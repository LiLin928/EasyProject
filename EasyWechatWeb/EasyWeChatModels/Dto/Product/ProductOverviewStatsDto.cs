namespace EasyWeChatModels.Dto;

/// <summary>
/// 商品概览统计DTO
/// </summary>
public class ProductOverviewStatsDto
{
    /// <summary>
    /// 今日统计
    /// </summary>
    public TodayStatsDto Today { get; set; } = new();

    /// <summary>
    /// 增长率统计
    /// </summary>
    public GrowthStatsDto Growth { get; set; } = new();
}