namespace EasyWeChatModels.Dto;

/// <summary>
/// 增长率统计DTO
/// </summary>
public class GrowthStatsDto
{
    /// <summary>
    /// 销售数量增长率
    /// </summary>
    public decimal SalesCountGrowth { get; set; }

    /// <summary>
    /// 销售金额增长率
    /// </summary>
    public decimal SalesAmountGrowth { get; set; }

    /// <summary>
    /// 订单数量增长率
    /// </summary>
    public decimal OrderCountGrowth { get; set; }
}