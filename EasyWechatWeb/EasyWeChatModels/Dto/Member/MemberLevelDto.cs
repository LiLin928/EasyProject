namespace EasyWeChatModels.Dto;

/// <summary>
/// 会员等级DTO
/// </summary>
public class MemberLevelDto
{
    /// <summary>
    /// 等级ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 等级名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 最低消费金额
    /// </summary>
    public decimal MinSpent { get; set; }

    /// <summary>
    /// 折扣比例
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// 积分倍率
    /// </summary>
    public decimal PointsRate { get; set; }

    /// <summary>
    /// 等级图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; } = string.Empty;
}