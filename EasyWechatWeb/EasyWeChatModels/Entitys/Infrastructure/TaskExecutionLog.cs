namespace EasyWeChatModels.Entitys;

using SqlSugar;

/// <summary>
/// 任务执行日志实体
/// </summary>
[SugarTable("TaskExecutionLog", "任务执行日志表")]
public class TaskExecutionLog
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 任务名称
    /// </summary>
    [SugarColumn(ColumnDescription = "任务名称", Length = 100)]
    public string JobName { get; set; } = string.Empty;

    /// <summary>
    /// 任务分组
    /// </summary>
    [SugarColumn(ColumnDescription = "任务分组", Length = 50)]
    public string JobGroup { get; set; } = string.Empty;

    /// <summary>
    /// 执行状态（0:执行中, 1:成功, 2:失败, 3:取消）
    /// </summary>
    [SugarColumn(ColumnDescription = "执行状态")]
    public int Status { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    [SugarColumn(ColumnDescription = "开始时间")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    [SugarColumn(ColumnDescription = "结束时间", IsNullable = true)]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 执行时长（毫秒）
    /// </summary>
    [SugarColumn(ColumnDescription = "执行时长(ms)", IsNullable = true)]
    public long? Duration { get; set; }

    /// <summary>
    /// 执行结果描述
    /// </summary>
    [SugarColumn(ColumnDescription = "执行结果", Length = 500, IsNullable = true)]
    public string? ResultMessage { get; set; }

    /// <summary>
    /// 异常信息
    /// </summary>
    [SugarColumn(ColumnDescription = "异常信息", ColumnDataType = "text", IsNullable = true)]
    public string? ExceptionMessage { get; set; }

    /// <summary>
    /// 异常堆栈
    /// </summary>
    [SugarColumn(ColumnDescription = "异常堆栈", ColumnDataType = "text", IsNullable = true)]
    public string? ExceptionStackTrace { get; set; }

    /// <summary>
    /// 触发类型（0:Cron触发, 1:手动触发）
    /// </summary>
    [SugarColumn(ColumnDescription = "触发类型")]
    public int TriggerType { get; set; }

    /// <summary>
    /// 执行实例 ID（用于多实例部署区分）
    /// </summary>
    [SugarColumn(ColumnDescription = "执行实例ID", Length = 100, IsNullable = true)]
    public string? InstanceId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}