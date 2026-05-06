using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 商品供应商关联实体类
/// </summary>
/// <remarks>
/// 用于记录商品与供应商的关联关系，包括采购价格、最小起订量等信息
/// </remarks>
[SugarTable("ProductSupplier", "商品供应商关联表")]
public class ProductSupplier
{
    /// <summary>
    /// 关联ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 商品ID
    /// </summary>
    [SugarColumn(ColumnDescription = "商品ID")]
    public Guid ProductId { get; set; }

    /// <summary>
    /// 供应商ID
    /// </summary>
    [SugarColumn(ColumnDescription = "供应商ID")]
    public Guid SupplierId { get; set; }

    /// <summary>
    /// SKU码
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "SKU码")]
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// 采购价格
    /// </summary>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "采购价格")]
    public decimal PurchasePrice { get; set; }

    /// <summary>
    /// 最小起订量
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "最小起订量")]
    public int? MinOrderQty { get; set; }

    /// <summary>
    /// 是否默认供应商
    /// </summary>
    [SugarColumn(ColumnDescription = "是否默认供应商")]
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "备注")]
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}