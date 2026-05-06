namespace EasyWeChatModels.Dto;

/// <summary>
/// 商品销量统计DTO
/// </summary>
public class ProductSalesStatsDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 商品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 商品图片
    /// </summary>
    public string? ProductImage { get; set; }

    /// <summary>
    /// SKU码
    /// </summary>
    public string SkuCode { get; set; } = string.Empty;

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

    /// <summary>
    /// 平均价格
    /// </summary>
    public decimal AvgPrice { get; set; }
}