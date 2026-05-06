namespace EasyWeChatModels.Dto;

/// <summary>
/// 库存统计DTO
/// </summary>
public class StockStatisticsDto
{
    /// <summary>
    /// 商品总数
    /// </summary>
    public int TotalProducts { get; set; }

    /// <summary>
    /// 总库存
    /// </summary>
    public int TotalStock { get; set; }

    /// <summary>
    /// 低库存商品数
    /// </summary>
    public int LowStockCount { get; set; }

    /// <summary>
    /// 缺货商品数
    /// </summary>
    public int OutOfStockCount { get; set; }

    /// <summary>
    /// 库存总价值
    /// </summary>
    public decimal TotalValue { get; set; }
}