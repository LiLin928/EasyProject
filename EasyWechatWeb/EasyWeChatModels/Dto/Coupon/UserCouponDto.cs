namespace EasyWeChatModels.Dto;

/// <summary>
/// 用户优惠券DTO
/// </summary>
public class UserCouponDto
{
    /// <summary>
    /// 用户优惠券ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 优惠券ID
    /// </summary>
    public Guid CouponId { get; set; }

    /// <summary>
    /// 优惠券名称
    /// </summary>
    public string CouponName { get; set; } = string.Empty;

    /// <summary>
    /// 类型：1-满减券，2-折扣券
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 类型文本
    /// </summary>
    public string TypeText => Type == 1 ? "满减券" : "折扣券";

    /// <summary>
    /// 优惠值
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// 最低消费金额
    /// </summary>
    public decimal MinAmount { get; set; }

    /// <summary>
    /// 状态：1-未使用，2-已使用，3-已过期
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态文本
    /// </summary>
    public string StatusText => Status switch
    {
        1 => "未使用",
        2 => "已使用",
        3 => "已过期",
        _ => "未知"
    };

    /// <summary>
    /// 使用时间
    /// </summary>
    public string? UsedTime { get; set; }

    /// <summary>
    /// 关联订单ID
    /// </summary>
    public Guid? OrderId { get; set; }

    /// <summary>
    /// 关联订单编号
    /// </summary>
    public string? OrderNo { get; set; }

    /// <summary>
    /// 领取时间
    /// </summary>
    public string ClaimTime { get; set; } = string.Empty;
}