using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 用户优惠券实体
/// </summary>
[SugarTable("UserCoupon", "用户优惠券表")]
public class UserCoupon
{
    /// <summary>
    /// ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户ID")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 优惠券ID
    /// </summary>
    [SugarColumn(ColumnDescription = "优惠券ID")]
    public Guid CouponId { get; set; }

    /// <summary>
    /// 状态：1-未使用，2-已使用，3-已过期
    /// </summary>
    [SugarColumn(ColumnDescription = "状态：1-未使用，2-已使用，3-已过期")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 使用时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "使用时间")]
    public DateTime? UsedTime { get; set; }

    /// <summary>
    /// 关联订单ID
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "关联订单ID")]
    public Guid? OrderId { get; set; }

    /// <summary>
    /// 领取时间
    /// </summary>
    [SugarColumn(ColumnDescription = "领取时间")]
    public DateTime ClaimTime { get; set; } = DateTime.Now;
}