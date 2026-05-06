namespace EasyWeChatModels.Dto;

/// <summary>
/// 微信用户信息 DTO
/// </summary>
public class WxUserInfoDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 微信 OpenID
    /// </summary>
    public string? OpenId { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// 头像 URL
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// 可用积分
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 会员等级名称
    /// </summary>
    public string? LevelName { get; set; }
}