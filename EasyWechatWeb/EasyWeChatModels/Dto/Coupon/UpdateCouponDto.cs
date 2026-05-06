namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新优惠券参数
/// </summary>
public class UpdateCouponDto
{
    /// <summary>
    /// 优惠券ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 优惠券名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 优惠值
    /// </summary>
    public decimal? Value { get; set; }

    /// <summary>
    /// 最低消费金额
    /// </summary>
    public decimal? MinAmount { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 发放总数
    /// </summary>
    public int? TotalCount { get; set; }

    /// <summary>
    /// 适用商品ID列表
    /// </summary>
    public List<Guid>? ProductIds { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}