using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// ETL调度任务实体
/// </summary>
[SugarTable("EtlSchedule", "ETL调度任务表")]
public class EtlSchedule
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 调度名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "调度名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 关联的任务流ID
    /// </summary>
    [SugarColumn(ColumnDescription = "任务流ID")]
    public Guid PipelineId { get; set; }

    /// <summary>
    /// 任务流名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "任务流名称")]
    public string? PipelineName { get; set; }

    /// <summary>
    /// 调度类型：cron, interval, once
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "调度类型")]
    public string ScheduleType { get; set; } = "cron";

    /// <summary>
    /// Cron表达式
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "Cron表达式")]
    public string? CronExpression { get; set; }

    /// <summary>
    /// 间隔时间（秒）
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "间隔时间(秒)")]
    public int? IntervalSeconds { get; set; }

    /// <summary>
    /// 执行参数（JSON格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "执行参数")]
    public string? ExecuteParams { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用")]
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// 状态：active, paused, stopped
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "状态")]
    public string Status { get; set; } = "paused";

    /// <summary>
    /// 最后执行时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "最后执行时间")]
    public DateTime? LastExecuteTime { get; set; }

    /// <summary>
    /// 下次执行时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "下次执行时间")]
    public DateTime? NextExecuteTime { get; set; }

    /// <summary>
    /// 执行次数
    /// </summary>
    [SugarColumn(ColumnDescription = "执行次数")]
    public int ExecuteCount { get; set; } = 0;

    /// <summary>
    /// 成功次数
    /// </summary>
    [SugarColumn(ColumnDescription = "成功次数")]
    public int SuccessCount { get; set; } = 0;

    /// <summary>
    /// 失败次数
    /// </summary>
    [SugarColumn(ColumnDescription = "失败次数")]
    public int FailureCount { get; set; } = 0;

    /// <summary>
    /// 创建人ID
    /// </summary>
    [SugarColumn(ColumnDescription = "创建人ID")]
    public Guid CreatorId { get; set; }

    /// <summary>
    /// 创建人名称
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "创建人名称")]
    public string? CreatorName { get; set; }

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