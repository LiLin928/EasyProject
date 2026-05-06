using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 商品评价实体类
/// </summary>
/// <remarks>
/// 用于存储用户对商品的评价信息，包括评分、评价内容、图片等
/// </remarks>
[SugarTable("ProductReview", "商品评价表")]
public class ProductReview
{
    /// <summary>
    /// 评价ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

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
    /// 订单ID
    /// </summary>
    [SugarColumn(ColumnDescription = "订单ID")]
    public Guid OrderId { get; set; }

    /// <summary>
    /// 订单编号
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "订单编号")]
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户ID")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "用户昵称")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 用户头像
    /// </summary>
    [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "用户头像")]
    public string? UserAvatar { get; set; }

    /// <summary>
    /// 评分
    /// </summary>
    /// <remarks>
    /// 评分范围1-5分
    /// </remarks>
    [SugarColumn(ColumnDescription = "评分1-5")]
    public int Rating { get; set; }

    /// <summary>
    /// 评价内容
    /// </summary>
    [SugarColumn(ColumnDataType = "text", ColumnDescription = "评价内容")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 评价图片JSON
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "评价图片JSON")]
    public string? Images { get; set; }

    /// <summary>
    /// 商家回复
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "商家回复")]
    public string? Reply { get; set; }

    /// <summary>
    /// 回复时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "回复时间")]
    public DateTime? ReplyTime { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// pending-待审核，approved-已通过，rejected-已拒绝
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态：pending-待审核，approved-已通过，rejected-已拒绝")]
    public string Status { get; set; } = "pending";

    /// <summary>
    /// 是否匿名
    /// </summary>
    [SugarColumn(ColumnDescription = "是否匿名")]
    public bool IsAnonymous { get; set; } = false;

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}