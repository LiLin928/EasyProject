namespace EasyWeChatModels.Dto;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// 创建任务参数
/// </summary>
public class CreateTaskDto
{
    /// <summary>
    /// 任务名称
    /// </summary>
    [Required]
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// 任务分组
    /// </summary>
    public string TaskGroup { get; set; } = "Default";

    /// <summary>
    /// 任务类型（0:Cron, 1:即时, 2:周期）
    /// </summary>
    [Required]
    public int TaskType { get; set; }

    /// <summary>
    /// 周期类型（仅 Periodic 类型需要）
    /// </summary>
    public int? ScheduleType { get; set; }

    /// <summary>
    /// Cron 表达式（仅 Cron 类型需要）
    /// </summary>
    public string? CronExpression { get; set; }

    /// <summary>
    /// 指定执行时间（仅 Specific 类型需要）
    /// </summary>
    public DateTime? ExecuteTime { get; set; }

    /// <summary>
    /// 每月几号执行（仅 Monthly 类型需要）
    /// </summary>
    public int? DayOfMonth { get; set; }

    /// <summary>
    /// 执行时间-小时（Daily/Monthly 类型需要）
    /// </summary>
    public int? ExecuteHour { get; set; }

    /// <summary>
    /// 执行时间-分钟（Daily/Monthly 类型需要）
    /// </summary>
    public int? ExecuteMinute { get; set; }

    /// <summary>
    /// 执行器类型（0:反射, 1:API）
    /// </summary>
    [Required]
    public int ExecutorType { get; set; }

    /// <summary>
    /// 处理器类型（类名，仅反射执行时需要）
    /// </summary>
    public string? HandlerType { get; set; }

    /// <summary>
    /// 处理器方法名
    /// </summary>
    public string? HandlerMethod { get; set; }

    /// <summary>
    /// API 回调地址（仅API执行时需要）
    /// </summary>
    public string? ApiEndpoint { get; set; }

    /// <summary>
    /// API 方法
    /// </summary>
    public string? ApiMethod { get; set; }

    /// <summary>
    /// API 请求体（JSON）
    /// </summary>
    public string? ApiPayload { get; set; }

    /// <summary>
    /// 业务数据（JSON）
    /// </summary>
    public string? BusinessData { get; set; }

    /// <summary>
    /// 任务描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    public int TimeoutSeconds { get; set; } = 300;
}