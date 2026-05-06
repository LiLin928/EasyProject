namespace EasyWeChatModels.Dto;

/// <summary>
/// 今日统计DTO
/// </summary>
public class TodayStatsDto
{
    /// <summary>
    /// 销售数量
    /// </summary>
    public int SalesCount { get; set; }

    /// <summary>
    /// 销售金额
    /// </summary>
    public decimal SalesAmount { get; set; }

    /// <summary>
    /// 订单数量
    /// </summary>
    public int OrderCount { get; set; }
}