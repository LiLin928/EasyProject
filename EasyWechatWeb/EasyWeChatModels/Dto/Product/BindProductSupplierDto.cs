namespace EasyWeChatModels.Dto;

/// <summary>
/// 绑定商品供应商参数
/// </summary>
public class BindProductSupplierDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 供应商ID
    /// </summary>
    public Guid SupplierId { get; set; }

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
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}