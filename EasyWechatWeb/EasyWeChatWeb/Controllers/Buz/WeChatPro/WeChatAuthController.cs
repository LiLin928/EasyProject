using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.WeChatPro;

/// <summary>
/// 微信认证控制器
/// 提供微信登录、用户信息获取和更新功能
/// </summary>
[ApiController]
[Route("api/wechat/auth")]
public class WeChatAuthController : BaseController
{
    /// <summary>
    /// 微信认证服务接口
    /// </summary>
    public IWeChatAuthService _authService { get; set; } = null!;
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatAuthController> _logger { get; set; } = null!;

    /// <summary>
    /// 微信登录
    /// </summary>
    /// <param name="dto">微信登录请求参数，包含微信登录 code</param>
    /// <returns>登录结果，包含 Token 和用户信息</returns>
    /// <response code="200">登录成功，返回 Token 和用户信息</response>
    /// <response code="400">登录失败，code 无效或过期</response>
    /// <remarks>
    /// 通过微信小程序登录获取的 code 进行登录。
    /// 登录成功后返回 JWT Token，用于后续接口认证。
    /// </remarks>
    /// <example>
    /// POST /api/wechat/auth/login
    /// Content-Type: application/json
    /// {
    ///     "code": "wx_login_code_from_wechat"
    /// }
    /// </example>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<WxLoginResultDto>), 200)]
    public async Task<ApiResponse<WxLoginResultDto>> Login([FromBody] WxLoginDto dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto.Code);
            if (result == null)
            {
                return Error<WxLoginResultDto>("登录失败，code 无效或已过期", 400);
            }
            return Success(result, "登录成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "微信登录失败: {Code}", dto.Code);
            return Error<WxLoginResultDto>("登录失败");
        }
    }

    /// <summary>
    /// 手机号登录
    /// </summary>
    /// <param name="dto">登录请求参数</param>
    /// <returns>登录结果，包含 Token 和用户信息</returns>
    /// <response code="200">登录成功</response>
    /// <response code="400">手机号或密码错误</response>
    /// <remarks>
    /// 通过手机号和密码进行登录，需要用户先绑定手机号并设置密码。
    /// </remarks>
    /// <example>
    /// POST /api/wechat/auth/phone-login
    /// Content-Type: application/json
    /// {
    ///     "phone": "13800138000",
    ///     "password": "123456"
    /// }
    /// </example>
    [HttpPost("phone-login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<WxLoginResultDto>), 200)]
    public async Task<ApiResponse<WxLoginResultDto>> PhoneLogin([FromBody] PhoneLoginDto dto)
    {
        try
        {
            var result = await _authService.PhoneLoginAsync(dto);
            if (result == null)
            {
                return Error<WxLoginResultDto>("手机号或密码错误", 400);
            }
            return Success(result, "登录成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "手机号登录失败: {Phone}", dto.Phone);
            return Error<WxLoginResultDto>("登录失败");
        }
    }

    /// <summary>
    /// 微信手机号快速验证登录
    /// </summary>
    /// <param name="dto">登录请求参数</param>
    /// <returns>登录结果，包含 Token 和用户信息</returns>
    /// <response code="200">登录成功</response>
    /// <response code="400">微信授权数据无效或已过期</response>
    /// <remarks>
    /// 通过微信小程序 getPhoneNumber 按钮获取的加密数据进行登录。
    /// 后端使用 SessionKey 解密手机号，根据手机号查找或创建用户。
    /// </remarks>
    /// <example>
    /// POST /api/wechat/auth/wx-phone-login
    /// Content-Type: application/json
    /// {
    ///     "code": "wx_login_code",
    ///     "encryptedData": "encrypted_phone_data",
    ///     "iv": "initial_vector"
    /// }
    /// </example>
    [HttpPost("wx-phone-login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<WxLoginResultDto>), 200)]
    public async Task<ApiResponse<WxLoginResultDto>> WxPhoneLogin([FromBody] WxPhoneLoginDto dto)
    {
        try
        {
            var result = await _authService.WxPhoneLoginAsync(dto);
            if (result == null)
            {
                return Error<WxLoginResultDto>("微信授权数据无效或已过期", 400);
            }
            return Success(result, "登录成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "微信手机号登录失败");
            return Error<WxLoginResultDto>("登录失败");
        }
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <returns>当前登录用户的详细信息</returns>
    /// <response code="200">成功获取用户信息</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">用户不存在</response>
    /// <remarks>
    /// 获取当前登录用户的详细信息，包括昵称、头像、手机号、积分、会员等级等。
    /// 需要在请求头中携带有效的 JWT Token。
    /// </remarks>
    /// <example>
    /// GET /api/wechat/auth/userinfo
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("userinfo")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<WxUserInfoDto>), 200)]
    public async Task<ApiResponse<WxUserInfoDto>> GetUserInfo()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<WxUserInfoDto>("请先登录", 401);
            }

            var result = await _authService.GetUserInfoAsync(userId);
            if (result == null)
            {
                return Error<WxUserInfoDto>("用户不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户信息失败");
            return Error<WxUserInfoDto>("获取用户信息失败");
        }
    }

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="dto">更新用户信息请求参数，包含昵称、头像、性别、手机号等</param>
    /// <returns>更新结果</returns>
    /// <response code="200">更新成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 更新当前登录用户的个人信息。
    /// 只更新请求中提供的字段，未提供的字段保持不变。
    /// </remarks>
    /// <example>
    /// PUT /api/wechat/auth/userinfo
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "nickname": "新昵称",
    ///     "avatarUrl": "https://example.com/avatar.jpg",
    ///     "gender": 1,
    ///     "phone": "13800138000"
    /// }
    /// </example>
    [HttpPut("userinfo")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> UpdateUserInfo([FromBody] UpdateWxUserDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<bool>("请先登录", 401);
            }

            var result = await _authService.UpdateUserInfoAsync(userId, dto);
            return Success(result, "更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用户信息失败");
            return Error<bool>("更新用户信息失败");
        }
    }

    /// <summary>
    /// 绑定手机号
    /// </summary>
    /// <param name="dto">绑定请求参数</param>
    /// <returns>绑定结果</returns>
    /// <response code="200">绑定成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">绑定失败，手机号已被占用</response>
    /// <remarks>
    /// 为当前登录用户绑定手机号，绑定后可使用手机号登录。
    /// </remarks>
    /// <example>
    /// POST /api/wechat/auth/bind-phone
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "phone": "13800138000",
    ///     "code": "123456"  // 可选，验证码
    /// }
    /// </example>
    [HttpPost("bind-phone")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<BindPhoneResultDto>), 200)]
    public async Task<ApiResponse<BindPhoneResultDto>> BindPhone([FromBody] BindPhoneDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<BindPhoneResultDto>("请先登录", 401);
            }

            var result = await _authService.BindPhoneAsync(userId, dto);
            if (!result.Success)
            {
                return Error<BindPhoneResultDto>(result.Message ?? "绑定失败", 400);
            }
            return Success(result, "绑定成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "绑定手机号失败");
            return Error<BindPhoneResultDto>("绑定手机号失败");
        }
    }

    /// <summary>
    /// 设置密码
    /// </summary>
    /// <param name="dto">设置密码请求参数</param>
    /// <returns>设置结果</returns>
    /// <response code="200">设置成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">设置失败，密码不符合要求</response>
    /// <remarks>
    /// 为当前登录用户设置密码，设置后可使用手机号+密码登录。
    /// 密码长度不能少于6位，需要确认密码一致。
    /// </remarks>
    /// <example>
    /// POST /api/wechat/auth/set-password
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "password": "123456",
    ///     "confirmPassword": "123456"
    /// }
    /// </example>
    [HttpPost("set-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> SetPassword([FromBody] SetPasswordDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<bool>("请先登录", 401);
            }

            var result = await _authService.SetPasswordAsync(userId, dto);
            return Success(result, "密码设置成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<bool>(ex.Message, 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "设置密码失败");
            return Error<bool>("设置密码失败");
        }
    }
}