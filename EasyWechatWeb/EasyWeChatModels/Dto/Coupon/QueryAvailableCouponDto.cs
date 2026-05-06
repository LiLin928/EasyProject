namespace EasyWeChatModels.Dto;

/// <summary>
/// 可用优惠券查询参数
/// </summary>
public class QueryAvailableCouponDto
{
    /// <summary>
    /// 订单金额
    /// </summary>
    public decimal OrderAmount { get; set; }

    /// <summary>
    /// 商品ID列表（用于筛选商品专属优惠券）
    /// </summary>
    public List<Guid>? ProductIds { get; set; }
}