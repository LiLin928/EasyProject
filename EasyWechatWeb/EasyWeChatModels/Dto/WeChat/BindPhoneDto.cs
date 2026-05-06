namespace EasyWeChatModels.Dto;

/// <summary>
/// 绑定手机号请求 DTO
/// </summary>
public class BindPhoneDto
{
    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 验证码（可选，根据业务需求）
    /// </summary>
    public string? Code { get; set; }
}