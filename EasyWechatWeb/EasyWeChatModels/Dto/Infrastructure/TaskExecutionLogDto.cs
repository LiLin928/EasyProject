namespace EasyWeChatModels.Dto;

/// <summary>
/// 任务执行日志 DTO
/// </summary>
public class TaskExecutionLogDto
{
    /// <summary>
    /// ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    public string JobName { get; set; } = string.Empty;

    /// <summary>
    /// 任务分组
    /// </summary>
    public string JobGroup { get; set; } = string.Empty;

    /// <summary>
    /// 执行状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态描述
    /// </summary>
    public string StatusText { get; set; } = string.Empty;

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 执行时长（毫秒）
    /// </summary>
    public long? Duration { get; set; }

    /// <summary>
    /// 执行结果
    /// </summary>
    public string? ResultMessage { get; set; }

    /// <summary>
    /// 异常信息
    /// </summary>
    public string? ExceptionMessage { get; set; }

    /// <summary>
    /// 触发类型
    /// </summary>
    public int TriggerType { get; set; }

    /// <summary>
    /// 执行实例 ID
    /// </summary>
    public string? InstanceId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}