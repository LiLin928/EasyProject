namespace EasyWeChatModels.Dto;

/// <summary>
/// 绑定手机号结果 DTO
/// </summary>
public class BindPhoneResultDto
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 更新后的用户信息
    /// </summary>
    public WxUserInfoDto? UserInfo { get; set; }
}