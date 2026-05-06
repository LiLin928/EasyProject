using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 优惠券实体
/// </summary>
[SugarTable("Coupon", "优惠券表")]
public class Coupon
{
    /// <summary>
    /// 优惠券ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 优惠券名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "优惠券名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 类型：1-满减券，2-折扣券
    /// </summary>
    [SugarColumn(ColumnDescription = "类型：1-满减券，2-折扣券")]
    public int Type { get; set; }

    /// <summary>
    /// 优惠值（满减金额或折扣比例）
    /// </summary>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "优惠值")]
    public decimal Value { get; set; }

    /// <summary>
    /// 最低消费金额
    /// </summary>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "最低消费金额")]
    public decimal MinAmount { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    [SugarColumn(ColumnDescription = "开始时间")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    [SugarColumn(ColumnDescription = "结束时间")]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// 发放总数
    /// </summary>
    [SugarColumn(ColumnDescription = "发放总数")]
    public int TotalCount { get; set; }

    /// <summary>
    /// 已领取数量
    /// </summary>
    [SugarColumn(ColumnDescription = "已领取数量")]
    public int ClaimedCount { get; set; } = 0;

    /// <summary>
    /// 适用商品ID列表（JSON数组，空为全场通用）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "适用商品ID列表")]
    public string? ProductIds { get; set; }

    /// <summary>
    /// 状态：1-启用，0-禁用
    /// </summary>
    [SugarColumn(ColumnDescription = "状态：1-启用，0-禁用")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}