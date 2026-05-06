namespace EasyWeChatModels.Dto.Infrastructure;

using EasyWeChatModels.Enums.Infrastructure;

/// <summary>
/// 任务补偿事件数据 - 用于重试失败后的撤销操作
/// </summary>
public class TaskCompensationDto
{
    /// <summary>
    /// 任务定义ID
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// 执行日志ID
    /// </summary>
    public Guid LogId { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// 错误消息
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// 补偿类型
    /// </summary>
    public CompensationType CompensationType { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetryCount { get; set; }

    /// <summary>
    /// 补偿时间
    /// </summary>
    public DateTime CompensationTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 补偿结果
    /// </summary>
    public string? CompensationResult { get; set; }

    /// <summary>
    /// 业务数据（用于补偿操作）
    /// </summary>
    public string? BusinessData { get; set; }
}