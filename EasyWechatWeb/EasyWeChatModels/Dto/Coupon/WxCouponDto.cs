namespace EasyWeChatModels.Dto;

/// <summary>
/// 微信优惠券 DTO
/// </summary>
public class WxCouponDto
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
    /// 开始时间（Unix毫秒时间戳）
    /// </summary>
    public long StartTime { get; set; }

    /// <summary>
    /// 结束时间（Unix毫秒时间戳）
    /// </summary>
    public long EndTime { get; set; }

    /// <summary>
    /// 是否可领取
    /// </summary>
    public bool CanClaim { get; set; }

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
}