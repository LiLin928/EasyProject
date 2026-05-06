namespace BusinessManager.Infrastructure.Service;

using BusinessManager.Infrastructure.IService;
using BusinessManager.Tasks;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Enums;
using InfrastructureManager.Quartz;
using Microsoft.Extensions.Logging;

/// <summary>
/// 任务执行服务实现
/// </summary>
public class TaskExecutorService : ITaskExecutorService
{
    /// <summary>
    /// 任务定义服务（属性注入）
    /// </summary>
    public ITaskDefinitionService _taskService { get; set; } = null!;

    /// <summary>
    /// 事务执行器（属性注入）
    /// </summary>
    public TransactionalTaskExecutor _transactionalExecutor { get; set; } = null!;

    /// <summary>
    /// 任务执行器工厂（属性注入）
    /// </summary>
    public TaskExecutorFactory _executorFactory { get; set; } = null!;

    /// <summary>
    /// 日志器（属性注入）
    /// </summary>
    public ILogger<TaskExecutorService> _logger { get; set; } = null!;

    /// <summary>
    /// 执行指定任务
    /// </summary>
    public async Task<TaskExecutionResult> ExecuteTaskAsync(Guid taskDefinitionId)
    {
        var task = await _taskService.GetByIdAsync(taskDefinitionId);
        if (task == null)
        {
            return TaskExecutionResult.Failed("任务不存在");
        }

        _logger.LogInformation($"开始执行任务: {task.TaskName}");

        // 使用事务执行器执行
        return await _transactionalExecutor.ExecuteHandlerAsync(taskDefinitionId);
    }

    /// <summary>
    /// 执行即时任务（无需数据库记录）
    /// </summary>
    public async Task<TaskExecutionResult> ExecuteImmediateTaskAsync(string handlerType, string? handlerMethod = null, string? businessData = null)
    {
        _logger.LogInformation($"执行即时任务: {handlerType}.{handlerMethod ?? "ExecuteAsync"}");
        return await _executorFactory.ExecuteAsync(handlerType, handlerMethod, businessData);
    }

    /// <summary>
    /// 调度任务（更新状态和下次执行时间）
    /// </summary>
    public async Task ScheduleTaskAsync(Guid taskDefinitionId)
    {
        var task = await _taskService.GetByIdAsync(taskDefinitionId);
        if (task != null)
        {
            await _taskService.UpdateTaskStatusAsync(taskDefinitionId, TaskDefinitionStatus.Scheduled);
            var nextTime = _taskService.CalculateNextExecuteTime(task);
            await _taskService.UpdateNextExecuteTimeAsync(taskDefinitionId, nextTime);
            _logger.LogInformation($"任务已调度: {task.TaskName}, 下次执行: {nextTime}");
        }
    }
}