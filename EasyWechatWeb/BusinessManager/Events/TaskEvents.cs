namespace BusinessManager.Events;

using BusinessManager.Infrastructure.IService;
using DotNetCore.CAP;
using EasyWeChatModels.Enums;
using Microsoft.Extensions.Logging;

/// <summary>
/// 任务完成事件数据
/// </summary>
public class TaskCompletedEvent
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
}

/// <summary>
/// 任务失败事件数据
/// </summary>
public class TaskFailedEvent
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
    public string Error { get; set; } = string.Empty;
}

/// <summary>
/// 任务事件消费者 - 处理任务完成和失败事件
/// </summary>
public class TaskEvents : ICapSubscribe
{
    /// <summary>
    /// 任务定义服务（属性注入）
    /// </summary>
    public ITaskDefinitionService _taskService { get; set; } = null!;

    /// <summary>
    /// 日志器（属性注入）
    /// </summary>
    public ILogger<TaskEvents> _logger { get; set; } = null!;

    /// <summary>
    /// 处理任务完成事件
    /// </summary>
    [CapSubscribe("task.completed")]
    public async Task OnTaskCompleted(TaskCompletedEvent evt)
    {
        _logger.LogInformation($"收到任务完成事件: {evt.TaskName}, TaskId: {evt.TaskId}");

        // 可扩展：发送通知、更新统计等
        // 目前仅记录日志，状态已在事务中更新
    }

    /// <summary>
    /// 处理任务失败事件
    /// </summary>
    [CapSubscribe("task.failed")]
    public async Task OnTaskFailed(TaskFailedEvent evt)
    {
        _logger.LogWarning($"收到任务失败事件: {evt.TaskName}, TaskId: {evt.TaskId}, Error: {evt.Error}");

        // 可扩展：发送告警通知、记录失败详情等
        // CAP 会自动重试，超过重试次数后进入失败队列
    }
}