namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新微信用户 DTO
/// </summary>
public class UpdateWxUserDto
{
    /// <summary>
    /// 昵称
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// 头像 URL
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public int? Gender { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }
}