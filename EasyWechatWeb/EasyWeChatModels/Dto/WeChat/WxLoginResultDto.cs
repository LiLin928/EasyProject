namespace EasyWeChatModels.Dto;

/// <summary>
/// 微信登录结果 DTO
/// </summary>
public class WxLoginResultDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// JWT Token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 用户信息
    /// </summary>
    public WxUserInfoDto UserInfo { get; set; } = new();
}