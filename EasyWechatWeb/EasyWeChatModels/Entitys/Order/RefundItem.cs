using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 售后商品明细实体类
/// </summary>
/// <remarks>
/// 用于存储售后申请中的商品明细信息
/// </remarks>
[SugarTable("RefundItem", "售后商品明细表")]
public class RefundItem
{
    /// <summary>
    /// 明细ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 售后记录ID
    /// </summary>
    [SugarColumn(ColumnDescription = "售后记录ID")]
    public Guid RefundId { get; set; }

    /// <summary>
    /// 订单商品ID
    /// </summary>
    [SugarColumn(ColumnDescription = "订单商品ID")]
    public Guid OrderItemId { get; set; }

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

    /// <summary>
    /// 金额
    /// </summary>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "金额")]
    public decimal Amount { get; set; }
}