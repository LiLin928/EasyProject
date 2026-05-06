namespace EasyWeChatModels.Dto;

/// <summary>
/// 新增优惠券参数
/// </summary>
public class AddCouponDto
{
    /// <summary>
    /// 优惠券名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 类型：1-满减券，2-折扣券
    /// </summary>
    public int Type { get; set; } = 1;

    /// <summary>
    /// 优惠值（满减金额或折扣比例，折扣券填0.1-1之间的小数）
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// 最低消费金额
    /// </summary>
    public decimal MinAmount { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// 发放总数
    /// </summary>
    public int TotalCount { get; set; } = 100;

    /// <summary>
    /// 适用商品ID列表（空为全场通用）
    /// </summary>
    public List<Guid>? ProductIds { get; set; }

    /// <summary>
    /// 状态：1-启用，0-禁用
    /// </summary>
    public int Status { get; set; } = 1;
}