namespace EasyWeChatModels.Dto;

/// <summary>
/// 客户DTO
/// </summary>
public class CustomerDto
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
    public int Status { get; set; }

    /// <summary>
    /// 状态文本
    /// </summary>
    public string StatusText => Status == 1 ? "正常" : "禁用";

    /// <summary>
    /// 会员等级ID
    /// </summary>
    public Guid? LevelId { get; set; }

    /// <summary>
    /// 会员等级名称
    /// </summary>
    public string? LevelName { get; set; }

    /// <summary>
    /// 可用积分
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 累计消费金额
    /// </summary>
    public decimal TotalSpent { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; } = string.Empty;

    /// <summary>
    /// 更新时间
    /// </summary>
    public string? UpdateTime { get; set; }
}