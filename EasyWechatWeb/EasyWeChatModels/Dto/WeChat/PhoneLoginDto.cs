namespace EasyWeChatModels.Dto;

/// <summary>
/// 手机号登录请求 DTO
/// </summary>
public class PhoneLoginDto
{
    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;
}