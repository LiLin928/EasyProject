using CommonManager.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CommonManager.Cache;

/// <summary>
/// Token 黑名单缓存服务
/// </summary>
/// <remarks>
/// 提供 Token 黑名单管理功能，用于实现登出时使 Token 失效。
/// 支持将单个 Token 加入黑名单，以及使用户的所有 Token 失效。
///
/// 使用 Autofac 属性注入模式，_cache 和 _configuration 属性会自动赋值。
///
/// 使用场景：
/// - 用户登出时，将当前 Token 加入黑名单
/// - 修改密码后，使用户所有 Token 失效
/// - 强制用户下线
///
/// 存储方式：
/// - 单个 Token 黑名单：token_blacklist:{jti} -> "1"
/// - 用户 Token 失效标记：user_tokens:{userId}:invalidate -> timestamp
/// </remarks>
/// <example>
/// <code>
/// // 使用属性注入
/// public class AuthController : BaseController
/// {
///     public TokenBlacklistCache _tokenBlacklistCache { get; set; } = null!;
/// }
/// </code>
/// </example>
public class TokenBlacklistCache
{
    /// <summary>
    /// 缓存实例（Autofac 属性注入）
    /// </summary>
    public ICache _cache { get; set; } = null!;

    /// <summary>
    /// 配置实例（Autofac 属性注入）
    /// </summary>
    public IConfiguration _configuration { get; set; } = null!;

    /// <summary>
    /// Token 黑名单缓存键前缀
    /// </summary>
    private const string TokenKeyPrefix = "token_blacklist:";

    /// <summary>
    /// 用户 Token 失效标记键前缀
    /// </summary>
    private const string UserTokenKeyPrefix = "user_tokens:";

    /// <summary>
    /// 将 Token 加入黑名单
    /// </summary>
    /// <param name="jti">JWT ID（Token 的唯一标识）</param>
    /// <param name="expiresIn">过期时间（秒）</param>
    /// <remarks>
    /// 将单个 Token 的 JWT ID 加入黑名单。
    /// 过期时间应与 AccessToken 的有效期一致，过期后自动清理。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 获取 JWT ID
    /// var jti = User.FindFirst("jti")?.Value;
    ///
    /// // 将 Token 加入黑名单（有效期 1 小时）
    /// await _tokenBlacklistCache.AddToBlacklistAsync(jti, 3600);
    /// </code>
    /// </example>
    public async Task AddToBlacklistAsync(string jti, int expiresIn)
    {
        if (string.IsNullOrEmpty(jti))
        {
            return;
        }

        var key = TokenKeyPrefix + jti;
        await _cache.SetAsync(key, "1", expiresIn);
    }

    /// <summary>
    /// 检查 Token 是否在黑名单中
    /// </summary>
    /// <param name="jti">JWT ID</param>
    /// <returns>是否在黑名单中</returns>
    /// <remarks>
    /// 在验证 Token 时调用此方法，检查 Token 是否已被加入黑名单。
    /// 如果返回 true，应拒绝该 Token 的访问请求。
    /// </remarks>
    /// <example>
    /// <code>
    /// var jti = tokenPayload.Jti;
    /// if (await _tokenBlacklistCache.IsBlacklistedAsync(jti))
    /// {
    ///     // Token 已失效，拒绝访问
    ///     return Unauthorized();
    /// }
    /// </code>
    /// </example>
    public async Task<bool> IsBlacklistedAsync(string jti)
    {
        if (string.IsNullOrEmpty(jti))
        {
            return false;
        }

        var key = TokenKeyPrefix + jti;
        var value = await _cache.GetAsync(key);
        return !string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// 将用户的所有 Token 加入失效列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <remarks>
    /// 记录一个时间戳，表示该时间点之前的所有 Token 都已失效。
    /// 适用于修改密码、强制下线等场景。
    ///
    /// 注意：此方法不会删除已有的 Token 黑名单记录，
    /// 而是通过时间戳比对来判断 Token 是否失效。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 用户修改密码后，使所有 Token 失效
    /// await _tokenBlacklistCache.InvalidateUserTokensAsync(userId);
    /// </code>
    /// </example>
    public async Task InvalidateUserTokensAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        var key = $"{UserTokenKeyPrefix}{userId}:invalidate";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        // 存储 30 天，覆盖大部分 RefreshToken 有效期
        await _cache.SetAsync(key, timestamp, 86400 * 30);
    }

    /// <summary>
    /// 检查用户的 Token 是否已失效
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="tokenIssuedAt">Token 签发时间（Unix 时间戳，秒）</param>
    /// <returns>是否已失效</returns>
    /// <remarks>
    /// 比对 Token 的签发时间与用户 Token 失效时间戳。
    /// 如果 Token 签发时间早于失效时间戳，则 Token 已失效。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 获取 Token 签发时间
    /// var iat = tokenPayload.Iat;
    ///
    /// // 检查是否已失效
    /// if (await _tokenBlacklistCache.IsUserTokenInvalidatedAsync(userId, iat))
    /// {
    ///     // Token 已失效
    /// }
    /// </code>
    /// </example>
    public async Task<bool> IsUserTokenInvalidatedAsync(string userId, long tokenIssuedAt)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return false;
        }

        var key = $"{UserTokenKeyPrefix}{userId}:invalidate";
        var value = await _cache.GetAsync(key);

        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        if (long.TryParse(value, out var invalidateTimestamp))
        {
            return tokenIssuedAt < invalidateTimestamp;
        }

        return false;
    }

    /// <summary>
    /// 清除用户的 Token 失效标记
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <remarks>
    /// 用于清除用户的 Token 失效标记，允许之前的 Token 继续使用。
    /// 通常在管理员解除强制下线等场景使用。
    /// </remarks>
    public async Task ClearUserTokenInvalidationAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        var key = $"{UserTokenKeyPrefix}{userId}:invalidate";
        await _cache.RemoveAsync(key);
    }
}