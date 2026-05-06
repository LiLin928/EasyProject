namespace BusinessManager.Events;

using DotNetCore.CAP;
using Microsoft.Extensions.Logging;

/// <summary>
/// 任务告警处理器 - 处理任务重试耗尽后的告警通知
/// </summary>
public class TaskAlertHandler : ICapSubscribe
{
    /// <summary>
    /// 日志器（属性注入）
    /// </summary>
    public ILogger<TaskAlertHandler> _logger { get; set; } = null!;

    /// <summary>
    /// 处理任务告警事件
    /// </summary>
    [CapSubscribe("task.alert")]
    public async Task HandleTaskAlert(TaskAlertEvent alert)
    {
        _logger.LogError(
            "任务告警 - 任务名称: {TaskName}, 任务ID: {TaskId}, 错误: {ErrorMessage}, 重试次数: {RetryCount}/{MaxRetryCount}, 告警时间: {AlertTime}",
            alert.TaskName, alert.TaskId, alert.ErrorMessage, alert.RetryCount, alert.MaxRetryCount, alert.AlertTime
        );

        // 可扩展：发送邮件、短信、钉钉等通知
        // 目前仅记录日志，后续可集成通知模块
        await Task.CompletedTask;
    }
}

/// <summary>
/// 任务告警事件数据
/// </summary>
public class TaskAlertEvent
{
    /// <summary>
    /// 任务ID
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// 错误消息
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetryCount { get; set; }

    /// <summary>
    /// 告警时间
    /// </summary>
    public DateTime AlertTime { get; set; }

    /// <summary>
    /// 告警类型
    /// </summary>
    public string AlertType { get; set; } = string.Empty;
}