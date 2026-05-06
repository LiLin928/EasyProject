using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 换货商品实体类
/// </summary>
/// <remarks>
/// 用于存储换货申请中的新商品信息
/// </remarks>
[SugarTable("ExchangeItem", "换货商品表")]
public class ExchangeItem
{
    /// <summary>
    /// 换货商品ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 售后记录ID
    /// </summary>
    [SugarColumn(ColumnDescription = "售后记录ID")]
    public Guid RefundId { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    [SugarColumn(ColumnDescription = "商品ID")]
    public Guid ProductId { get; set; }

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
    /// 商品单价
    /// </summary>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "商品单价")]
    public decimal Price { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [SugarColumn(ColumnDescription = "数量")]
    public int Quantity { get; set; }
}