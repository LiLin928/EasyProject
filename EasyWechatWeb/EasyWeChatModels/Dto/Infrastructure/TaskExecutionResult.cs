namespace EasyWeChatModels.Dto;

/// <summary>
/// 任务执行结果
/// </summary>
public class TaskExecutionResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 执行消息
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 执行日志 ID
    /// </summary>
    public Guid? LogId { get; set; }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static TaskExecutionResult Success(string? message = null, Guid? logId = null)
    {
        return new TaskExecutionResult { IsSuccess = true, Message = message, LogId = logId };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static TaskExecutionResult Failed(string message, Guid? logId = null)
    {
        return new TaskExecutionResult { IsSuccess = false, Message = message, LogId = logId };
    }
}