namespace EasyWeChatModels.Dto;

/// <summary>
/// 用户优惠券 DTO（微信端）
/// </summary>
public class WxUserCouponDto
{
    /// <summary>
    /// 用户优惠券ID
    /// </summary>
    public Guid Id { get; set; }

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
    /// 开始时间（Unix毫秒时间戳）
    /// </summary>
    public long StartTime { get; set; }

    /// <summary>
    /// 结束时间（Unix毫秒时间戳）
    /// </summary>
    public long EndTime { get; set; }

    /// <summary>
    /// 领取时间（Unix毫秒时间戳）
    /// </summary>
    public long ClaimTime { get; set; }

    /// <summary>
    /// 优惠描述
    /// </summary>
    public string Description
    {
        get
        {
            if (Type == 1)
            {
                return MinAmount > 0 ? $"满{MinAmount}减{Value}" : $"直减{Value}元";
            }
            else
            {
                return MinAmount > 0 ? $"满{MinAmount}打{Value * 10}折" : $"{Value * 10}折券";
            }
        }
    }

    /// <summary>
    /// 是否可用
    /// </summary>
    public bool IsUsable => Status == 1 && EndTime > DateTimeOffset.Now.ToUnixTimeMilliseconds();
}