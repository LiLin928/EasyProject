using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 会员等级实体类
/// </summary>
/// <remarks>
/// 用于存储会员等级配置信息，支持等级升级规则和权益配置
/// </remarks>
[SugarTable("MemberLevel", "会员等级表")]
public class MemberLevel
{
    /// <summary>
    /// 等级ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 等级名称
    /// </summary>
    /// <remarks>
    /// 会员等级的显示名称，如：普通会员、银卡会员、金卡会员等
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "等级名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 最低消费金额
    /// </summary>
    /// <remarks>
    /// 达到此消费金额可升级到该等级
    /// </remarks>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "最低消费金额")]
    public decimal MinSpent { get; set; }

    /// <summary>
    /// 折扣比例（0-100）
    /// </summary>
    /// <remarks>
    /// 会员享受的折扣比例，如98表示98折，90表示9折
    /// </remarks>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "折扣比例(0-100)")]
    public decimal Discount { get; set; } = 100;

    /// <summary>
    /// 积分倍率
    /// </summary>
    /// <remarks>
    /// 消费获得积分的倍率，如1.5表示1.5倍积分
    /// </remarks>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "积分倍率")]
    public decimal PointsRate { get; set; } = 1;

    /// <summary>
    /// 等级图标
    /// </summary>
    /// <remarks>
    /// 等级图标的URL地址
    /// </remarks>
    [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "等级图标")]
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>
    /// 等级显示排序，数值越小越靠前
    /// </remarks>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 等级状态：1-启用，0-禁用
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态：1-启用，0-禁用")]
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