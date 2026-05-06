namespace EasyWeChatModels.Dto;

/// <summary>
/// 库存DTO
/// </summary>
public class StockDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// SKU码
    /// </summary>
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 商品图片
    /// </summary>
    public string? ProductImage { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// 库存数量
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// 库存预警阈值
    /// </summary>
    public int AlertThreshold { get; set; }

    /// <summary>
    /// 是否低库存
    /// </summary>
    public bool IsLowStock { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public string? UpdateTime { get; set; }
}