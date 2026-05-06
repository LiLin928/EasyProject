namespace EasyWeChatModels.Dto;

/// <summary>
/// 库存变动记录DTO
/// </summary>
public class StockRecordDto
{
    /// <summary>
    /// 记录ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// SKU码
    /// </summary>
    public string? SkuCode { get; set; }

    /// <summary>
    /// 商品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 商品图片
    /// </summary>
    public string? ProductImage { get; set; }

    /// <summary>
    /// 变动类型
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 变动数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 变动前库存
    /// </summary>
    public int BeforeStock { get; set; }

    /// <summary>
    /// 变动后库存
    /// </summary>
    public int AfterStock { get; set; }

    /// <summary>
    /// 供应商名称
    /// </summary>
    public string? SupplierName { get; set; }

    /// <summary>
    /// 采购价格
    /// </summary>
    public decimal? PurchasePrice { get; set; }

    /// <summary>
    /// 操作人
    /// </summary>
    public string? Operator { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; } = string.Empty;
}