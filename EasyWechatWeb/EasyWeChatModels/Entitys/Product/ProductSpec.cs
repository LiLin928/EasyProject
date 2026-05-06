using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 商品规格组实体
/// </summary>
[SugarTable("ProductSpec", "商品规格组表")]
public class ProductSpec
{
    /// <summary>
    /// 规格ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 商品ID
    /// </summary>
    [SugarColumn(ColumnDescription = "商品ID")]
    public Guid ProductId { get; set; }

    /// <summary>
    /// 规格名称（如：颜色、尺寸）
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "规格名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 是否必选
    /// </summary>
    [SugarColumn(ColumnDescription = "是否必选")]
    public bool Required { get; set; } = true;

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

/// <summary>
/// 规格选项实体
/// </summary>
[SugarTable("ProductSpecOption", "规格选项表")]
public class ProductSpecOption
{
    /// <summary>
    /// 选项ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 规格ID
    /// </summary>
    [SugarColumn(ColumnDescription = "规格ID")]
    public Guid SpecId { get; set; }

    /// <summary>
    /// 选项名称（如：红色、XL）
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "选项名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 选项值
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "选项值")]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 价格调整（加价或减价）
    /// </summary>
    [SugarColumn(DecimalDigits = 2, IsNullable = true, ColumnDescription = "价格调整")]
    public decimal? PriceAdjust { get; set; }

    /// <summary>
    /// 库存（可选）
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "库存")]
    public int? Stock { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}

/// <summary>
/// 商品SKU实体
/// </summary>
[SugarTable("ProductSku", "商品SKU表")]
public class ProductSku
{
    /// <summary>
    /// SKU ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 商品ID
    /// </summary>
    [SugarColumn(ColumnDescription = "商品ID")]
    public Guid ProductId { get; set; }

    /// <summary>
    /// SKU编码
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "SKU编码")]
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// 规格组合（JSON格式，如：[{"specId":"xxx","optionId":"xxx"}]）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", ColumnDescription = "规格组合")]
    public string SpecCombination { get; set; } = string.Empty;

    /// <summary>
    /// 价格
    /// </summary>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "价格")]
    public decimal Price { get; set; }

    /// <summary>
    /// 库存
    /// </summary>
    [SugarColumn(ColumnDescription = "库存")]
    public int Stock { get; set; } = 0;

    /// <summary>
    /// 图片
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "图片")]
    public string? Image { get; set; }

    /// <summary>
    /// 状态：1-启用，0-禁用
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}