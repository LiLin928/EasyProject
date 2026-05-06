namespace EasyWeChatModels.Dto;

/// <summary>
/// 微信登录请求 DTO
/// </summary>
public class WxLoginDto
{
    /// <summary>
    /// 微信登录 code
    /// </summary>
    public string Code { get; set; } = string.Empty;
}