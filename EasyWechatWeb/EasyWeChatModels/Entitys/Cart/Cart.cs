using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 购物车实体类
/// </summary>
/// <remarks>
/// 用于存储用户购物车商品信息
/// </remarks>
[SugarTable("Cart", "购物车表")]
public class Cart
{
    /// <summary>
    /// 购物车ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 用户ID
    /// </summary>
    /// <remarks>
    /// 所属用户的外键
    /// </remarks>
    [SugarColumn(ColumnDescription = "用户ID")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    /// <remarks>
    /// 商品的外键
    /// </remarks>
    [SugarColumn(ColumnDescription = "商品ID")]
    public Guid ProductId { get; set; }

    /// <summary>
    /// 商品数量
    /// </summary>
    /// <remarks>
    /// 购物车中该商品的数量
    /// </remarks>
    [SugarColumn(ColumnDescription = "商品数量")]
    public int Count { get; set; } = 1;

    /// <summary>
    /// 是否选中
    /// </summary>
    /// <remarks>
    /// 购物车中商品是否被选中用于结算：true-选中，false-未选中
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否选中")]
    public bool Selected { get; set; } = true;

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 购物车记录创建时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <remarks>
    /// 购物车记录最后更新时间，可为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}