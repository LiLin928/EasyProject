using BusinessManager.Basic.IService;
using BusinessManager.Basic.Service;
using CommonManager.Base;
using CommonManager.Cache;
using CommonManager.Dto;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EasyWeChatWeb.Controllers.Basic;

/// <summary>
/// 认证控制器
/// 提供用户登录、登出、Token刷新等认证相关功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : BaseController
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public IUserService _userService { get; set; } = null!;
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<AuthController> _logger { get; set; } = null!;
    /// <summary>
    /// Token 黑名单缓存
    /// </summary>
    public TokenBlacklistCache _tokenBlacklistCache { get; set; } = null!;
    /// <summary>
    /// 配置
    /// </summary>
    public IConfiguration _configuration { get; set; } = null!;

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="dto">登录请求参数，包含用户名和密码</param>
    /// <returns>登录结果，包含AccessToken和RefreshToken</returns>
    /// <response code="200">登录成功，返回用户信息和Token</response>
    /// <response code="401">用户名或密码错误</response>
    /// <remarks>
    /// 登录成功返回双Token：
    /// - AccessToken: 有效期60分钟，用于API访问
    /// - RefreshToken: 有效期7天，用于刷新Token
    ///
    /// AccessToken过期后，使用RefreshToken调用 /api/auth/refresh 接口获取新的Token
    /// </remarks>
    /// <example>
    /// POST /api/auth/login
    /// Content-Type: application/json
    /// {
    ///     "userName": "admin",
    ///     "password": "123456"
    /// }
    /// </example>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResultDto>), 200)]
    public async Task<ApiResponse<LoginResultDto>> Login([FromBody] LoginDto dto)
    {
        try
        {
            var result = await _userService.LoginAsync(dto);
            if (result == null)
            {
                return Error<LoginResultDto>("用户名或密码错误", 401);
            }
            return Success(result, "登录成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登录失败: {UserName}", dto.UserName);
            return Error<LoginResultDto>("登录失败");
        }
    }

    /// <summary>
    /// 刷新Token
    /// </summary>
    /// <param name="refreshToken">刷新令牌，登录时获取的RefreshToken</param>
    /// <returns>新的登录结果，包含新的AccessToken和RefreshToken</returns>
    /// <response code="200">刷新成功，返回新的Token</response>
    /// <response code="401">刷新令牌无效或已过期</response>
    /// <remarks>
    /// 当AccessToken过期时，使用RefreshToken获取新的Token。
    /// RefreshToken有效期为7天，过期后需要重新登录。
    /// 每次刷新会返回新的AccessToken和RefreshToken，旧的RefreshToken将失效。
    /// </remarks>
    /// <example>
    /// POST /api/auth/refresh
    /// Content-Type: application/json
    /// "your-refresh-token-string"
    /// </example>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<LoginResultDto>), 200)]
    public async Task<ApiResponse<LoginResultDto>> RefreshToken([FromBody] string refreshToken)
    {
        try
        {
            var result = await _userService.RefreshTokenAsync(refreshToken);
            if (result == null)
            {
                return Error<LoginResultDto>("刷新令牌无效或已过期", 401);
            }
            return Success(result, "刷新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刷新Token失败");
            return Error<LoginResultDto>("刷新失败");
        }
    }

    /// <summary>
    /// 用户登出
    /// </summary>
    /// <returns>登出结果</returns>
    /// <response code="200">登出成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 登出操作会执行以下操作：
    /// - 将当前 AccessToken 加入黑名单
    /// - 标记用户所有 Token 失效（可选）
    ///
    /// 客户端应在登出后删除本地存储的 Token。
    /// 此接口需要 Authorization 头携带有效的 AccessToken。
    /// </remarks>
    /// <example>
    /// POST /api/auth/logout
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<ApiResponse<object>> Logout()
    {
        try
        {
            // 获取当前用户的 Token 信息
            var jti = User.FindFirst("jti")?.Value ??
                      User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                         User.FindFirst("sub")?.Value ??
                         User.FindFirst("userId")?.Value;

            // 将当前 Token 加入黑名单
            if (!string.IsNullOrEmpty(jti))
            {
                var expiresIn = _configuration.GetValue<int>("JWTTokenOptions:AccessTokenExpiration", 60) * 60;
                await _tokenBlacklistCache.AddToBlacklistAsync(jti, expiresIn);
                _logger.LogInformation("Token 已加入黑名单: {Jti}", jti);
            }

            // 标记用户的所有 Token 失效（可选，增强安全性）
            if (!string.IsNullOrEmpty(userId))
            {
                await _tokenBlacklistCache.InvalidateUserTokensAsync(userId);
            }

            _logger.LogInformation("用户登出成功: {UserId}", userId);
            return Success("登出成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登出失败");
            return Error<object>("登出失败");
        }
    }
}

// JWT 注册声明名称常量
internal static class JwtRegisteredClaimNames
{
    public const string Jti = "jti";
}