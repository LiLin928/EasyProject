namespace EasyWeChatModels.Dto;

/// <summary>
/// 商品供应商关联DTO
/// </summary>
public class ProductSupplierDto
{
    /// <summary>
    /// 关联ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 供应商ID
    /// </summary>
    public Guid SupplierId { get; set; }

    /// <summary>
    /// 供应商名称
    /// </summary>
    public string SupplierName { get; set; } = string.Empty;

    /// <summary>
    /// 商品信息
    /// </summary>
    public ProductDto? Product { get; set; }

    /// <summary>
    /// SKU码
    /// </summary>
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// 采购价格
    /// </summary>
    public decimal PurchasePrice { get; set; }

    /// <summary>
    /// 最小起订量
    /// </summary>
    public int? MinOrderQty { get; set; }

    /// <summary>
    /// 是否默认供应商
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}