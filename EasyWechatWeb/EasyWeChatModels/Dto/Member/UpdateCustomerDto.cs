namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新客户参数
/// </summary>
public class UpdateCustomerDto
{
    /// <summary>
    /// 客户ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

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
    public int? Status { get; set; }

    /// <summary>
    /// 会员等级ID
    /// </summary>
    public Guid? LevelId { get; set; }
}