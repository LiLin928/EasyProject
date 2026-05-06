using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CommonManager.Dto;
using EasyWeChatModels.Options;
using Microsoft.IdentityModel.Tokens;

namespace CommonManager.Helper;

/// <summary>
/// JWT Token 帮助类，提供 Token 的生成、验证和刷新功能
/// </summary>
/// <remarks>
/// 该类实现了双 Token 机制（Access Token + Refresh Token），提高安全性：
/// - Access Token：短期有效的访问令牌，用于接口认证
/// - Refresh Token：长期有效的刷新令牌，用于获取新的 Access Token
///
/// Token 中包含用户基本信息（UserId、UserName、RealName），便于在接口中获取当前用户信息。
/// </remarks>
/// <example>
/// <code>
/// // 用户登录时生成 Token
/// var jwtOptions = _config.GetSection&lt;JWTTokenOptions&gt;("JWTTokenOptions");
/// var loginResult = CustomerJWTHelper.GetLoginToken(userId, userName, realName, jwtOptions);
///
/// // 验证 Token
/// var principal = CustomerJWTHelper.ValidateToken(token, jwtOptions);
/// if (principal != null)
/// {
///     var userId = CustomerJWTHelper.GetUserId(principal);
/// }
///
/// // 刷新 Token
/// var newToken = CustomerJWTHelper.RefreshAccessToken(refreshToken, jwtOptions, ValidateUserExists);
/// </code>
/// </example>
public static class CustomerJWTHelper
{
    /// <summary>
    /// 生成登录 Token（双 Token 机制）
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="userName">用户名（登录账号）</param>
    /// <param name="realName">用户真实姓名</param>
    /// <param name="options">JWT 配置选项</param>
    /// <param name="roles">用户角色列表（可选）</param>
    /// <returns>包含 Access Token 和 Refresh Token 的登录结果</returns>
    public static LoginResultDto GetLoginToken(string userId, string userName, string realName, JWTTokenOptions options, List<string>? roles = null)
    {
        var accessToken = GenerateAccessToken(userId, userName, realName, options, roles);
        var refreshToken = GenerateRefreshToken(userId, userName, options);

        return new LoginResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = options.AccessTokenExpiration * 60
        };
    }

    /// <summary>
    /// 生成访问令牌（Access Token）
    /// </summary>
    private static string GenerateAccessToken(string userId, string userName, string realName, JWTTokenOptions options, List<string>? roles = null)
    {
        var claims = new List<Claim>
        {
            new("UserId", userId),
            new("UserName", userName),
            new("RealName", realName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // 添加角色声明
        if (roles != null && roles.Count > 0)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        return GenerateToken(claims, options, options.AccessTokenExpiration);
    }

    /// <summary>
    /// 生成刷新令牌（Refresh Token）
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="userName">用户名</param>
    /// <param name="options">JWT 配置选项</param>
    /// <returns>Refresh Token 字符串</returns>
    /// <remarks>
    /// Refresh Token 包含的声明：
    /// - UserId：用户 ID
    /// - UserName：用户名
    /// - TokenType：令牌类型标识（"Refresh"）
    /// - jti：JWT ID（唯一标识）
    ///
    /// 通过 TokenType 声明区分 Access Token 和 Refresh Token。
    /// </remarks>
    private static string GenerateRefreshToken(string userId, string userName, JWTTokenOptions options)
    {
        var claims = new List<Claim>
        {
            new("UserId", userId),
            new("UserName", userName),
            new("TokenType", "Refresh"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        return GenerateToken(claims, options, options.RefreshTokenExpiration);
    }

    /// <summary>
    /// 生成 JWT Token
    /// </summary>
    /// <param name="claims">声明列表</param>
    /// <param name="options">JWT 配置选项</param>
    /// <param name="expirationMinutes">过期时间（分钟）</param>
    /// <returns>JWT Token 字符串</returns>
    /// <remarks>
    /// 使用 HMAC SHA256 算法签名。
    /// SecurityKey 至少需要 32 个字符以保证安全性。
    /// </remarks>
    private static string GenerateToken(List<Claim> claims, JWTTokenOptions options, int expirationMinutes)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecurityKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 验证 Token 并返回声明主体
    /// </summary>
    /// <param name="token">要验证的 JWT Token 字符串</param>
    /// <param name="options">JWT 配置选项</param>
    /// <returns>验证成功返回 ClaimsPrincipal，验证失败返回 null</returns>
    /// <remarks>
    /// 验证内容包括：
    /// - 签发者（Issuer）验证
    /// - 受众（Audience）验证
    /// - 生命周期（Lifetime）验证
    /// - 签名密钥验证
    ///
    /// ClockSkew 设置为 0，即精确验证过期时间，不预留缓冲时间。
    /// 验证失败返回 null，不抛出异常，便于业务逻辑处理。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 在中间件或过滤器中验证 Token
    /// var principal = CustomerJWTHelper.ValidateToken(token, jwtOptions);
    /// if (principal == null)
    /// {
    ///     // Token 无效或已过期
    ///     return Unauthorized();
    /// }
    ///
    /// // 获取用户信息
    /// var userId = CustomerJWTHelper.GetUserId(principal);
    /// </code>
    /// </example>
    public static ClaimsPrincipal? ValidateToken(string token, JWTTokenOptions options)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(options.SecurityKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = options.Issuer,
                ValidAudience = options.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 从 ClaimsPrincipal 中获取用户 ID
    /// </summary>
    /// <param name="principal">ClaimsPrincipal 对象（从 Token 验证后获得）</param>
    /// <returns>用户 ID 字符串，不存在则返回空字符串</returns>
    /// <remarks>
    /// 通常在 Controller 或 Service 中使用，获取当前登录用户的 ID。
    /// 返回字符串格式，可根据需要转换为 Guid 或 int。
    /// </remarks>
    /// <example>
    /// <code>
    /// [Authorize]
    /// public ApiResponse&lt;User&gt; GetProfile()
    /// {
    ///     var userIdStr = CustomerJWTHelper.GetUserId(User);
    ///     var userId = Guid.Parse(userIdStr);
    ///     var user = _userService.GetByIdAsync(userId);
    ///     return Success(user);
    /// }
    /// </code>
    /// </example>
    public static string GetUserId(ClaimsPrincipal principal)
    {
        return principal.FindFirst("UserId")?.Value ?? string.Empty;
    }

    /// <summary>
    /// 从 ClaimsPrincipal 中获取用户名
    /// </summary>
    /// <param name="principal">ClaimsPrincipal 对象</param>
    /// <returns>用户名，不存在则返回空字符串</returns>
    /// <remarks>
    /// 获取 Token 中存储的 UserName 声明值。
    /// </remarks>
    /// <example>
    /// <code>
    /// var userName = CustomerJWTHelper.GetUserName(User);
    /// Console.WriteLine($"当前用户: {userName}");
    /// </code>
    /// </example>
    public static string GetUserName(ClaimsPrincipal principal)
    {
        return principal.FindFirst("UserName")?.Value ?? string.Empty;
    }

    /// <summary>
    /// 使用 Refresh Token 刷新 Access Token
    /// </summary>
    /// <param name="refreshToken">Refresh Token 字符串</param>
    /// <param name="options">JWT 配置选项</param>
    /// <param name="validateUser">用户验证函数，用于检查用户是否仍然有效（如是否被禁用）</param>
    /// <returns>刷新成功返回新的登录结果，失败返回 null</returns>
    /// <remarks>
    /// 刷新流程：
    /// 1. 验证 Refresh Token 是否有效
    /// 2. 检查 TokenType 是否为 "Refresh"
    /// 3. 调用 validateUser 函数检查用户状态
    /// 4. 生成新的 Access Token
    ///
    /// validateUser 函数应由业务层提供，用于检查用户是否被禁用、注销等。
    /// </remarks>
    /// <example>
    /// <code>
    /// [HttpPost("refresh")]
    /// [AllowAnonymous]
    /// public async Task&lt;ApiResponse&lt;LoginResultDto&gt;&gt; RefreshToken(RefreshTokenDto dto)
    /// {
    ///     var jwtOptions = _config.GetSection&lt;JWTTokenOptions&gt;("JWTTokenOptions");
    ///
    ///     var result = CustomerJWTHelper.RefreshAccessToken(
    ///         dto.RefreshToken,
    ///         jwtOptions,
    ///         userId => _userService.ExistsAsync(u => u.Id == userId && u.Status == "Active")
    ///     );
    ///
    ///     if (result == null)
    ///         return Error&lt;LoginResultDto&gt;("Token 无效或已过期", 401);
    ///
    ///     return Success(result);
    /// }
    /// </code>
    /// </example>
    public static LoginResultDto? RefreshAccessToken(string refreshToken, JWTTokenOptions options, Func<string, bool> validateUser)
    {
        var principal = ValidateToken(refreshToken, options);
        if (principal == null) return null;

        var tokenType = principal.FindFirst("TokenType")?.Value;
        if (tokenType != "Refresh") return null;

        var userId = GetUserId(principal);
        var userName = GetUserName(principal);

        if (!validateUser(userId)) return null;

        var newAccessToken = GenerateAccessToken(userId, userName, userName, options);

        return new LoginResultDto
        {
            AccessToken = newAccessToken,
            RefreshToken = refreshToken,
            ExpiresIn = options.AccessTokenExpiration * 60
        };
    }
}