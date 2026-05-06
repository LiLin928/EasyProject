namespace EasyWeChatModels.Dto;

/// <summary>
/// 库存调整参数
/// </summary>
public class StockAdjustDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 变动类型：in, out, adjust
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 变动数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 供应商ID
    /// </summary>
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// 采购价格
    /// </summary>
    public decimal? PurchasePrice { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}