using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 订单评价实体类
/// </summary>
/// <remarks>
/// 用于存储用户对订单的评价信息，包括多维度评分
/// </remarks>
[SugarTable("OrderReview", "订单评价表")]
public class OrderReview
{
    /// <summary>
    /// 评价ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

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
    /// 商品ID
    /// </summary>
    [SugarColumn(ColumnDescription = "商品ID")]
    public Guid ProductId { get; set; }

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

    #region 商品维度评分

    /// <summary>
    /// 商品质量评分
    /// </summary>
    [SugarColumn(ColumnDescription = "商品质量1-5")]
    public int ProductQuality { get; set; }

    /// <summary>
    /// 描述相符评分
    /// </summary>
    [SugarColumn(ColumnDescription = "描述相符1-5")]
    public int DescriptionMatch { get; set; }

    /// <summary>
    /// 性价比评分
    /// </summary>
    [SugarColumn(ColumnDescription = "性价比1-5")]
    public int CostPerformance { get; set; }

    #endregion

    #region 服务维度评分

    /// <summary>
    /// 发货速度评分
    /// </summary>
    [SugarColumn(ColumnDescription = "发货速度1-5")]
    public int ShippingSpeed { get; set; }

    /// <summary>
    /// 物流服务评分
    /// </summary>
    [SugarColumn(ColumnDescription = "物流服务1-5")]
    public int LogisticsService { get; set; }

    /// <summary>
    /// 客服态度评分
    /// </summary>
    [SugarColumn(ColumnDescription = "客服态度1-5")]
    public int CustomerService { get; set; }

    #endregion

    /// <summary>
    /// 综合评分
    /// </summary>
    [SugarColumn(DecimalDigits = 1, ColumnDescription = "综合评分")]
    public decimal OverallRating { get; set; }

    /// <summary>
    /// 评价文字
    /// </summary>
    [SugarColumn(ColumnDataType = "text", ColumnDescription = "评价文字")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 图片JSON
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "图片JSON")]
    public string? Images { get; set; }

    /// <summary>
    /// 视频JSON
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "视频JSON")]
    public string? Videos { get; set; }

    /// <summary>
    /// 标签JSON
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "标签JSON")]
    public string? Tags { get; set; }

    /// <summary>
    /// 是否匿名
    /// </summary>
    [SugarColumn(ColumnDescription = "是否匿名")]
    public bool IsAnonymous { get; set; } = false;

    /// <summary>
    /// 获得积分
    /// </summary>
    [SugarColumn(ColumnDescription = "获得积分")]
    public int PointsEarned { get; set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// normal-正常，hidden-隐藏
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态")]
    public string Status { get; set; } = "normal";

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