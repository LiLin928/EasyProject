namespace EasyWeChatModels.Dto;

/// <summary>
/// 任务定义 DTO
/// </summary>
public class TaskDefinitionDto
{
    /// <summary>
    /// ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// 任务分组
    /// </summary>
    public string TaskGroup { get; set; } = "Default";

    /// <summary>
    /// 任务类型
    /// </summary>
    public int TaskType { get; set; }

    /// <summary>
    /// 任务类型描述
    /// </summary>
    public string TaskTypeText { get; set; } = string.Empty;

    /// <summary>
    /// 周期类型
    /// </summary>
    public int? ScheduleType { get; set; }

    /// <summary>
    /// Cron 表达式
    /// </summary>
    public string? CronExpression { get; set; }

    /// <summary>
    /// 指定执行时间
    /// </summary>
    public DateTime? ExecuteTime { get; set; }

    /// <summary>
    /// 每月几号执行
    /// </summary>
    public int? DayOfMonth { get; set; }

    /// <summary>
    /// 执行时间-小时
    /// </summary>
    public int? ExecuteHour { get; set; }

    /// <summary>
    /// 执行时间-分钟
    /// </summary>
    public int? ExecuteMinute { get; set; }

    /// <summary>
    /// 执行器类型（0:反射, 1:API）
    /// </summary>
    public int ExecutorType { get; set; }

    /// <summary>
    /// 处理器类型（仅反射执行时需要）
    /// </summary>
    public string? HandlerType { get; set; }

    /// <summary>
    /// 处理器方法名
    /// </summary>
    public string? HandlerMethod { get; set; }

    /// <summary>
    /// API 回调地址
    /// </summary>
    public string? ApiEndpoint { get; set; }

    /// <summary>
    /// API 方法
    /// </summary>
    public string? ApiMethod { get; set; }

    /// <summary>
    /// API 请求体
    /// </summary>
    public string? ApiPayload { get; set; }

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetries { get; set; }

    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    public int TimeoutSeconds { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态描述
    /// </summary>
    public string StatusText { get; set; } = string.Empty;

    /// <summary>
    /// 任务描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 下次执行时间
    /// </summary>
    public DateTime? NextExecuteTime { get; set; }

    /// <summary>
    /// 最后执行时间
    /// </summary>
    public DateTime? LastExecuteTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}