namespace EasyWeChatModels.Entitys;

using SqlSugar;

/// <summary>
/// 任务定义实体 - 存储动态任务配置
/// </summary>
[SugarTable("TaskDefinition", "任务定义表")]
public class TaskDefinition
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
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// 任务分组
    /// </summary>
    [SugarColumn(ColumnDescription = "任务分组", Length = 50)]
    public string TaskGroup { get; set; } = "Default";

    /// <summary>
    /// 任务类型（0:Cron, 1:即时, 2:周期）
    /// </summary>
    [SugarColumn(ColumnDescription = "任务类型")]
    public int TaskType { get; set; }

    /// <summary>
    /// 周期类型（0:每天, 1:每月, 2:指定时间）
    /// </summary>
    [SugarColumn(ColumnDescription = "周期类型", IsNullable = true)]
    public int? ScheduleType { get; set; }

    /// <summary>
    /// Cron 表达式
    /// </summary>
    [SugarColumn(ColumnDescription = "Cron表达式", Length = 100, IsNullable = true)]
    public string? CronExpression { get; set; }

    /// <summary>
    /// 指定执行时间
    /// </summary>
    [SugarColumn(ColumnDescription = "指定执行时间", IsNullable = true)]
    public DateTime? ExecuteTime { get; set; }

    /// <summary>
    /// 每月几号执行（1-31）
    /// </summary>
    [SugarColumn(ColumnDescription = "每月几号", IsNullable = true)]
    public int? DayOfMonth { get; set; }

    /// <summary>
    /// 每天执行时间（小时）
    /// </summary>
    [SugarColumn(ColumnDescription = "执行时间-小时", IsNullable = true)]
    public int? ExecuteHour { get; set; }

    /// <summary>
    /// 每天执行时间（分钟）
    /// </summary>
    [SugarColumn(ColumnDescription = "执行时间-分钟", IsNullable = true)]
    public int? ExecuteMinute { get; set; }

    /// <summary>
    /// 执行器类型（0:反射, 1:API）
    /// </summary>
    [SugarColumn(ColumnDescription = "执行器类型")]
    public int ExecutorType { get; set; }

    /// <summary>
    /// 处理器类型（类名，仅反射执行时需要）
    /// </summary>
    [SugarColumn(ColumnDescription = "处理器类型", Length = 200, IsNullable = true)]
    public string? HandlerType { get; set; } = string.Empty;

    /// <summary>
    /// 处理器方法名
    /// </summary>
    [SugarColumn(ColumnDescription = "处理器方法名", Length = 100, IsNullable = true)]
    public string? HandlerMethod { get; set; }

    /// <summary>
    /// API 回调地址
    /// </summary>
    [SugarColumn(ColumnDescription = "API回调地址", Length = 200, IsNullable = true)]
    public string? ApiEndpoint { get; set; }

    /// <summary>
    /// API 方法（GET/POST）
    /// </summary>
    [SugarColumn(ColumnDescription = "API方法", Length = 10, IsNullable = true)]
    public string? ApiMethod { get; set; }

    /// <summary>
    /// API 请求体（JSON）
    /// </summary>
    [SugarColumn(ColumnDescription = "API请求体", ColumnDataType = "text", IsNullable = true)]
    public string? ApiPayload { get; set; }

    /// <summary>
    /// 状态（0:待调度, 1:已调度, 2:暂停, 3:已完成, 4:失败）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; }

    /// <summary>
    /// 优先级
    /// </summary>
    [SugarColumn(ColumnDescription = "优先级")]
    public int Priority { get; set; }

    /// <summary>
    /// 最大重试次数
    /// </summary>
    [SugarColumn(ColumnDescription = "最大重试次数")]
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    [SugarColumn(ColumnDescription = "超时时间(秒)")]
    public int TimeoutSeconds { get; set; } = 300;

    /// <summary>
    /// 业务数据（JSON）
    /// </summary>
    [SugarColumn(ColumnDescription = "业务数据", ColumnDataType = "text", IsNullable = true)]
    public string? BusinessData { get; set; }

    /// <summary>
    /// 任务描述
    /// </summary>
    [SugarColumn(ColumnDescription = "任务描述", Length = 500, IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间", IsNullable = true)]
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 最后执行时间
    /// </summary>
    [SugarColumn(ColumnDescription = "最后执行时间", IsNullable = true)]
    public DateTime? LastExecuteTime { get; set; }

    /// <summary>
    /// 下次执行时间
    /// </summary>
    [SugarColumn(ColumnDescription = "下次执行时间", IsNullable = true)]
    public DateTime? NextExecuteTime { get; set; }
}