namespace EasyWeChatModels.Dto;

/// <summary>
/// 微信手机号快速验证登录请求 DTO
/// </summary>
public class WxPhoneLoginDto
{
    /// <summary>
    /// 微信登录 code
    /// </summary>
    /// <example>wx_code_from_miniprogram</example>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 加密的手机号数据
    /// </summary>
    public string EncryptedData { get; set; } = string.Empty;

    /// <summary>
    /// 初始向量
    /// </summary>
    public string Iv { get; set; } = string.Empty;
}