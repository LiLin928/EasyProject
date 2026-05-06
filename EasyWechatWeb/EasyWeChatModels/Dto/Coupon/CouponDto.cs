namespace EasyWeChatModels.Dto;

/// <summary>
/// 优惠券DTO
/// </summary>
public class CouponDto
{
    /// <summary>
    /// 优惠券ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 优惠券名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 类型：1-满减券，2-折扣券
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 类型文本
    /// </summary>
    public string TypeText => Type == 1 ? "满减券" : "折扣券";

    /// <summary>
    /// 优惠值（满减金额或折扣比例）
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// 最低消费金额
    /// </summary>
    public decimal MinAmount { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public string StartTime { get; set; } = string.Empty;

    /// <summary>
    /// 结束时间
    /// </summary>
    public string EndTime { get; set; } = string.Empty;

    /// <summary>
    /// 发放总数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 已领取数量
    /// </summary>
    public int ClaimedCount { get; set; }

    /// <summary>
    /// 剩余数量
    /// </summary>
    public int RemainCount => TotalCount - ClaimedCount;

    /// <summary>
    /// 适用商品ID列表
    /// </summary>
    public List<Guid>? ProductIds { get; set; }

    /// <summary>
    /// 状态：1-启用，0-禁用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态文本
    /// </summary>
    public string StatusText => Status == 1 ? "启用" : "禁用";

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; } = string.Empty;
}