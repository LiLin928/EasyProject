namespace EasyWeChatModels.Dto;

/// <summary>
/// 创建客户参数
/// </summary>
public class AddCustomerDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    public string Nickname { get; set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 会员等级ID
    /// </summary>
    public Guid? LevelId { get; set; }
}