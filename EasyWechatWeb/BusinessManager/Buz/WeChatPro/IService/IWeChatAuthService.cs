using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 微信认证服务接口
/// </summary>
public interface IWeChatAuthService
{
    /// <summary>
    /// 微信登录
    /// </summary>
    /// <param name="code">微信登录 code</param>
    /// <returns>登录结果</returns>
    Task<WxLoginResultDto?> LoginAsync(string code);

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户信息</returns>
    Task<WxUserInfoDto?> GetUserInfoAsync(Guid userId);

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="dto">更新信息</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateUserInfoAsync(Guid userId, UpdateWxUserDto dto);

    /// <summary>
    /// 绑定手机号
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="dto">绑定请求</param>
    /// <returns>绑定结果</returns>
    Task<BindPhoneResultDto> BindPhoneAsync(Guid userId, BindPhoneDto dto);

    /// <summary>
    /// 设置密码
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="dto">设置密码请求</param>
    /// <returns>是否成功</returns>
    Task<bool> SetPasswordAsync(Guid userId, SetPasswordDto dto);

    /// <summary>
    /// 手机号登录
    /// </summary>
    /// <param name="dto">登录请求</param>
    /// <returns>登录结果</returns>
    Task<WxLoginResultDto?> PhoneLoginAsync(PhoneLoginDto dto);

    /// <summary>
    /// 微信手机号快速验证登录
    /// </summary>
    /// <param name="dto">微信手机号登录请求参数</param>
    /// <returns>登录结果；失败返回 null</returns>
    Task<WxLoginResultDto?> WxPhoneLoginAsync(WxPhoneLoginDto dto);
}