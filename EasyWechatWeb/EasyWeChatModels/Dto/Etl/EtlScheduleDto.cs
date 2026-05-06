namespace EasyWeChatModels.Dto;

/// <summary>
/// ETL调度任务DTO
/// </summary>
public class EtlScheduleDto
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 关联的任务流ID
    /// </summary>
    public Guid PipelineId { get; set; }

    /// <summary>
    /// 任务流名称
    /// </summary>
    public string? PipelineName { get; set; }

    /// <summary>
    /// 调度类型
    /// </summary>
    public string ScheduleType { get; set; } = "cron";

    /// <summary>
    /// Cron表达式
    /// </summary>
    public string? CronExpression { get; set; }

    /// <summary>
    /// Cron描述（中文）
    /// </summary>
    public string? CronDescription { get; set; }

    /// <summary>
    /// 间隔时间（秒）
    /// </summary>
    public int? IntervalSeconds { get; set; }

    /// <summary>
    /// 执行参数
    /// </summary>
    public string? ExecuteParams { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; set; } = "paused";

    /// <summary>
    /// 最后执行时间
    /// </summary>
    public string? LastExecuteTime { get; set; }

    /// <summary>
    /// 下次执行时间
    /// </summary>
    public string? NextExecuteTime { get; set; }

    /// <summary>
    /// 执行次数
    /// </summary>
    public int ExecuteCount { get; set; }

    /// <summary>
    /// 成功次数
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 失败次数
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// 创建人名称
    /// </summary>
    public string? CreatorName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string? CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public string? UpdateTime { get; set; }
}

/// <summary>
/// 查询调度参数
/// </summary>
public class QueryEtlScheduleDto
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 名称关键字
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 任务流ID
    /// </summary>
    public Guid? PipelineId { get; set; }

    /// <summary>
    /// 状态筛选
    /// </summary>
    public string? Status { get; set; }
}

/// <summary>
/// 创建调度参数
/// </summary>
public class CreateEtlScheduleDto
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 任务流ID
    /// </summary>
    public Guid PipelineId { get; set; }

    /// <summary>
    /// 调度类型
    /// </summary>
    public string ScheduleType { get; set; } = "cron";

    /// <summary>
    /// Cron表达式
    /// </summary>
    public string? CronExpression { get; set; }

    /// <summary>
    /// 间隔时间（秒）
    /// </summary>
    public int? IntervalSeconds { get; set; }

    /// <summary>
    /// 执行参数
    /// </summary>
    public string? ExecuteParams { get; set; }
}

/// <summary>
/// 更新调度参数
/// </summary>
public class UpdateEtlScheduleDto
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 调度类型
    /// </summary>
    public string? ScheduleType { get; set; }

    /// <summary>
    /// Cron表达式
    /// </summary>
    public string? CronExpression { get; set; }

    /// <summary>
    /// 间隔时间（秒）
    /// </summary>
    public int? IntervalSeconds { get; set; }

    /// <summary>
    /// 执行参数（JSON格式）
    /// </summary>
    public string? ExecuteParams { get; set; }
}