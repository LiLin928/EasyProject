namespace EasyWeChatModels.Dto;

/// <summary>
/// 添加会员等级参数
/// </summary>
public class AddMemberLevelDto
{
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
    public decimal Discount { get; set; } = 100;

    /// <summary>
    /// 积分倍率
    /// </summary>
    public decimal PointsRate { get; set; } = 1;

    /// <summary>
    /// 等级图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; } = 1;
}