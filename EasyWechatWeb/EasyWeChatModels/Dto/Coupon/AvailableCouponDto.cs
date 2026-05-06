namespace EasyWeChatModels.Dto;

/// <summary>
/// 可用优惠券结果 DTO
/// </summary>
public class AvailableCouponDto
{
    /// <summary>
    /// 用户优惠券ID
    /// </summary>
    public Guid UserCouponId { get; set; }

    /// <summary>
    /// 优惠券ID
    /// </summary>
    public Guid CouponId { get; set; }

    /// <summary>
    /// 优惠券名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 类型：1-满减券，2-折扣券
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 优惠值
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// 最低消费金额
    /// </summary>
    public decimal MinAmount { get; set; }

    /// <summary>
    /// 优惠金额（根据订单金额计算）
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// 优惠描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
}