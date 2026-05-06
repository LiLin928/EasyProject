using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 订单项实体类
/// </summary>
/// <remarks>
/// 用于存储订单中的商品明细信息
/// </remarks>
[SugarTable("OrderItem", "订单项表")]
public class OrderItem
{
    /// <summary>
    /// 订单项ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 订单ID
    /// </summary>
    /// <remarks>
    /// 所属订单的外键
    /// </remarks>
    [SugarColumn(ColumnDescription = "订单ID")]
    public Guid OrderId { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    /// <remarks>
    /// 商品的外键
    /// </remarks>
    [SugarColumn(ColumnDescription = "商品ID")]
    public Guid ProductId { get; set; }

    /// <summary>
    /// 商品名称
    /// </summary>
    /// <remarks>
    /// 下单时的商品名称（快照），长度限制100字符
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "商品名称")]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 商品图片
    /// </summary>
    /// <remarks>
    /// 下单时的商品主图URL（快照），长度限制500字符
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "商品图片")]
    public string ProductImage { get; set; } = string.Empty;

    /// <summary>
    /// 商品单价
    /// </summary>
    /// <remarks>
    /// 下单时的商品单价（快照），保留两位小数
    /// </remarks>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "商品单价")]
    public decimal Price { get; set; }

    /// <summary>
    /// 商品数量
    /// </summary>
    /// <remarks>
    /// 购买的商品数量
    /// </remarks>
    [SugarColumn(ColumnDescription = "商品数量")]
    public int Count { get; set; }

    /// <summary>
    /// 小计金额
    /// </summary>
    /// <remarks>
    /// 商品单价 * 数量，保留两位小数
    /// </remarks>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "小计金额")]
    public decimal Subtotal { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 订单项创建时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}