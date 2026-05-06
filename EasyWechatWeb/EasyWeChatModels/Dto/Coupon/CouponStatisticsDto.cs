namespace EasyWeChatModels.Dto;

/// <summary>
/// 优惠券统计DTO
/// </summary>
public class CouponStatisticsDto
{
    /// <summary>
    /// 优惠券总数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 启用数量
    /// </summary>
    public int ActiveCount { get; set; }

    /// <summary>
    /// 禁用数量
    /// </summary>
    public int InactiveCount { get; set; }

    /// <summary>
    /// 已领取总数
    /// </summary>
    public int TotalClaimedCount { get; set; }

    /// <summary>
    /// 未使用数量
    /// </summary>
    public int UnusedCount { get; set; }

    /// <summary>
    /// 已使用数量
    /// </summary>
    public int UsedCount { get; set; }

    /// <summary>
    /// 已过期数量
    /// </summary>
    public int ExpiredCount { get; set; }
}