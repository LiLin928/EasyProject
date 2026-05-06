using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 积分变动记录实体类
/// </summary>
/// <remarks>
/// 用于记录用户积分的变动历史，包括获得和消耗
/// </remarks>
[SugarTable("PointsRecord", "积分变动记录表")]
public class PointsRecord
{
    /// <summary>
    /// 记录ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 用户ID
    /// </summary>
    /// <remarks>
    /// 积分变动的用户ID
    /// </remarks>
    [SugarColumn(ColumnDescription = "用户ID")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 积分变动（正数为获得，负数为消耗）
    /// </summary>
    [SugarColumn(ColumnDescription = "积分变动")]
    public int Points { get; set; }

    /// <summary>
    /// 变动后余额
    /// </summary>
    /// <remarks>
    /// 变动后的积分余额
    /// </remarks>
    [SugarColumn(ColumnDescription = "变动后余额")]
    public int Balance { get; set; }

    /// <summary>
    /// 变动类型
    /// </summary>
    /// <remarks>
    /// 类型：review-评价奖励，order-订单奖励，exchange-积分兑换，refund-退款扣除，system-系统调整
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "变动类型")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 来源ID
    /// </summary>
    /// <remarks>
    /// 关联的来源对象ID，如订单ID、评价ID等
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "来源ID")]
    public Guid? SourceId { get; set; }

    /// <summary>
    /// 原因描述
    /// </summary>
    /// <remarks>
    /// 积分变动的原因说明
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "原因描述")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// 操作人ID
    /// </summary>
    /// <remarks>
    /// 系统调整时的操作人ID
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "操作人ID")]
    public Guid? OperatorId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}