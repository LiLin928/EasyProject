// 文件：EasyWeChatModels/Dto/Infrastructure/UpdateTaskDto.cs
using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 任务更新参数 DTO
/// </summary>
public class UpdateTaskDto
{
    /// <summary>
    /// 任务ID（编辑时必填）
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 2)]
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
    /// 周期类型（0:每天, 1:每月, 2:指定时间）
    /// </summary>
    public int? ScheduleType { get; set; }

    /// <summary>
    /// Cron表达式
    /// </summary>
    public string? CronExpression { get; set; }

    /// <summary>
    /// 指定执行时间
    /// </summary>
    public DateTime? ExecuteTime { get; set; }

    /// <summary>
    /// 每月几号执行（1-31）
    /// </summary>
    public int? DayOfMonth { get; set; }

    /// <summary>
    /// 执行时间-小时（0-23）
    /// </summary>
    public int? ExecuteHour { get; set; }

    /// <summary>
    /// 执行时间-分钟（0-59）
    /// </summary>
    public int? ExecuteMinute { get; set; }

    /// <summary>
    /// 执行器类型（0:反射, 1:API）
    /// </summary>
    [Required]
    public int ExecutorType { get; set; }

    /// <summary>
    /// 处理器类名
    /// </summary>
    public string? HandlerType { get; set; }

    /// <summary>
    /// 处理器方法名
    /// </summary>
    public string? HandlerMethod { get; set; }

    /// <summary>
    /// API地址
    /// </summary>
    public string? ApiEndpoint { get; set; }

    /// <summary>
    /// API请求方法（GET/POST）
    /// </summary>
    public string? ApiMethod { get; set; }

    /// <summary>
    /// API请求体（JSON）
    /// </summary>
    public string? ApiPayload { get; set; }

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    public int TimeoutSeconds { get; set; } = 300;

    /// <summary>
    /// 任务描述
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }
}