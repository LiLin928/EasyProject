namespace EasyWeChatModels.Dto;

/// <summary>
/// 用户优惠券查询参数
/// </summary>
public class QueryUserCouponDto
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 优惠券ID
    /// </summary>
    public Guid? CouponId { get; set; }

    /// <summary>
    /// 状态：1-未使用，2-已使用，3-已过期
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 优惠券名称
    /// </summary>
    public string? CouponName { get; set; }
}