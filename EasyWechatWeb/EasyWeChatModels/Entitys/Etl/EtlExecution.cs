using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// ETL执行记录实体
/// </summary>
[SugarTable("EtlExecution", "ETL执行记录表")]
public class EtlExecution
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 任务流ID
    /// </summary>
    [SugarColumn(ColumnDescription = "任务流ID")]
    public Guid PipelineId { get; set; }

    /// <summary>
    /// 任务流名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "任务流名称")]
    public string? PipelineName { get; set; }

    /// <summary>
    /// 调度ID（可选，手动执行时为空）
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "调度ID")]
    public Guid? ScheduleId { get; set; }

    /// <summary>
    /// 执行状态：pending, running, success, failure, cancelled
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "执行状态")]
    public string Status { get; set; } = "pending";

    /// <summary>
    /// 执行参数（JSON格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "执行参数")]
    public string? ExecuteParams { get; set; }

    /// <summary>
    /// 执行结果（JSON格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "执行结果")]
    public string? Result { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "错误信息")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "开始时间")]
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "结束时间")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 执行耗时（毫秒）
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "执行耗时(ms)")]
    public long? Duration { get; set; }

    /// <summary>
    /// 进度百分比
    /// </summary>
    [SugarColumn(ColumnDescription = "进度百分比")]
    public int Progress { get; set; } = 0;

    /// <summary>
    /// 已完成节点数
    /// </summary>
    [SugarColumn(ColumnDescription = "已完成节点数")]
    public int CompletedNodes { get; set; } = 0;

    /// <summary>
    /// 总节点数
    /// </summary>
    [SugarColumn(ColumnDescription = "总节点数")]
    public int TotalNodes { get; set; } = 0;

    /// <summary>
    /// 当前执行节点ID
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "当前节点ID")]
    public string? CurrentNodeId { get; set; }

    /// <summary>
    /// 当前执行节点名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "当前节点名称")]
    public string? CurrentNodeName { get; set; }

    /// <summary>
    /// 触发类型：manual, schedule
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "触发类型")]
    public string TriggerType { get; set; } = "manual";

    /// <summary>
    /// 触发人ID
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "触发人ID")]
    public Guid? TriggerUserId { get; set; }

    /// <summary>
    /// 触发人名称
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "触发人名称")]
    public string? TriggerUserName { get; set; }

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