using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 库存变动记录实体类
/// </summary>
/// <remarks>
/// 用于记录商品的库存变动，包括入库、出库、调整等操作
/// </remarks>
[SugarTable("StockRecord", "库存变动记录表")]
public class StockRecord
{
    /// <summary>
    /// 记录ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 商品ID
    /// </summary>
    [SugarColumn(ColumnDescription = "商品ID")]
    public Guid ProductId { get; set; }

    /// <summary>
    /// SKU码
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "SKU码")]
    public string? SkuCode { get; set; }

    /// <summary>
    /// 商品名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "商品名称")]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 商品图片
    /// </summary>
    [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "商品图片")]
    public string? ProductImage { get; set; }

    /// <summary>
    /// 变动类型
    /// </summary>
    /// <remarks>
    /// in-入库，out-出库，adjust-调整
    /// </remarks>
    [SugarColumn(ColumnDescription = "变动类型：in-入库，out-出库，adjust-调整")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 变动数量
    /// </summary>
    [SugarColumn(ColumnDescription = "变动数量")]
    public int Quantity { get; set; }

    /// <summary>
    /// 变动前库存
    /// </summary>
    [SugarColumn(ColumnDescription = "变动前库存")]
    public int BeforeStock { get; set; }

    /// <summary>
    /// 变动后库存
    /// </summary>
    [SugarColumn(ColumnDescription = "变动后库存")]
    public int AfterStock { get; set; }

    /// <summary>
    /// 供应商ID
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "供应商ID")]
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// 供应商名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "供应商名称")]
    public string? SupplierName { get; set; }

    /// <summary>
    /// 采购价格
    /// </summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = true, ColumnDescription = "采购价格")]
    public decimal? PurchasePrice { get; set; }

    /// <summary>
    /// 操作人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "操作人")]
    public string? Operator { get; set; }

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