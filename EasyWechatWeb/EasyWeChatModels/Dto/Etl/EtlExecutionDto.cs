namespace EasyWeChatModels.Dto;

/// <summary>
/// ETL执行记录DTO
/// </summary>
public class EtlExecutionDto
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 任务流ID
    /// </summary>
    public Guid PipelineId { get; set; }

    /// <summary>
    /// 任务流名称
    /// </summary>
    public string? PipelineName { get; set; }

    /// <summary>
    /// 调度ID
    /// </summary>
    public Guid? ScheduleId { get; set; }

    /// <summary>
    /// 执行状态
    /// </summary>
    public string Status { get; set; } = "pending";

    /// <summary>
    /// 执行参数
    /// </summary>
    public string? ExecuteParams { get; set; }

    /// <summary>
    /// 执行结果
    /// </summary>
    public string? Result { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public string? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public string? EndTime { get; set; }

    /// <summary>
    /// 执行耗时（毫秒）
    /// </summary>
    public long? Duration { get; set; }

    /// <summary>
    /// 进度百分比
    /// </summary>
    public int Progress { get; set; }

    /// <summary>
    /// 已完成节点数
    /// </summary>
    public int CompletedNodes { get; set; }

    /// <summary>
    /// 总节点数
    /// </summary>
    public int TotalNodes { get; set; }

    /// <summary>
    /// 当前节点ID
    /// </summary>
    public string? CurrentNodeId { get; set; }

    /// <summary>
    /// 当前节点名称
    /// </summary>
    public string? CurrentNodeName { get; set; }

    /// <summary>
    /// 触发类型
    /// </summary>
    public string TriggerType { get; set; } = string.Empty;

    /// <summary>
    /// 触发人名称
    /// </summary>
    public string? TriggerUserName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string? CreateTime { get; set; }
}

/// <summary>
/// 查询执行记录参数
/// </summary>
public class QueryEtlExecutionDto
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
    /// 任务流ID
    /// </summary>
    public Guid? PipelineId { get; set; }

    /// <summary>
    /// 调度ID
    /// </summary>
    public Guid? ScheduleId { get; set; }

    /// <summary>
    /// 状态筛选
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 开始时间（别名，用于前端兼容）
    /// </summary>
    public string? DateStart { get; set; }

    /// <summary>
    /// 结束时间（别名，用于前端兼容）
    /// </summary>
    public string? DateEnd { get; set; }

    /// <summary>
    /// 开始时间（原始字段）
    /// </summary>
    public string? StartTimeBegin { get; set; }

    /// <summary>
    /// 结束时间（原始字段）
    /// </summary>
    public string? StartTimeEnd { get; set; }
}

/// <summary>
/// 执行统计DTO
/// </summary>
public class EtlExecutionStatisticsDto
{
    /// <summary>
    /// 总执行次数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 成功次数
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 失败次数
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// 运行中次数
    /// </summary>
    public int RunningCount { get; set; }

    /// <summary>
    /// 待执行次数
    /// </summary>
    public int PendingCount { get; set; }

    /// <summary>
    /// 平均耗时（毫秒）
    /// </summary>
    public long AvgDuration { get; set; }

    /// <summary>
    /// 成功率
    /// </summary>
    public double SuccessRate { get; set; }
}

/// <summary>
/// 今日统计DTO
/// </summary>
public class EtlTodayStatisticsDto
{
    /// <summary>
    /// 总数
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 运行中
    /// </summary>
    public int Running { get; set; }

    /// <summary>
    /// 成功
    /// </summary>
    public int Success { get; set; }

    /// <summary>
    /// 失败
    /// </summary>
    public int Failure { get; set; }

    /// <summary>
    /// 待执行
    /// </summary>
    public int Pending { get; set; }
}