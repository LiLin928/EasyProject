namespace EasyWeChatModels.Dto;

/// <summary>
/// 订单统计DTO
/// </summary>
public class OrderStatisticsDto
{
    /// <summary>
    /// 今日订单数
    /// </summary>
    public int TodayCount { get; set; }

    /// <summary>
    /// 今日订单金额
    /// </summary>
    public decimal TodayAmount { get; set; }

    /// <summary>
    /// 待付款订单数
    /// </summary>
    public int PendingPaymentCount { get; set; }

    /// <summary>
    /// 待发货订单数
    /// </summary>
    public int PendingShipCount { get; set; }

    /// <summary>
    /// 待收货订单数
    /// </summary>
    public int PendingReceiveCount { get; set; }

    /// <summary>
    /// 售后中订单数
    /// </summary>
    public int RefundingCount { get; set; }
}