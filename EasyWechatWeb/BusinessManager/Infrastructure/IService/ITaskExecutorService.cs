namespace BusinessManager.Infrastructure.IService;

using EasyWeChatModels.Dto;

/// <summary>
/// 任务执行服务接口
/// </summary>
public interface ITaskExecutorService
{
    /// <summary>
    /// 执行指定任务
    /// </summary>
    Task<TaskExecutionResult> ExecuteTaskAsync(Guid taskDefinitionId);

    /// <summary>
    /// 执行即时任务
    /// </summary>
    Task<TaskExecutionResult> ExecuteImmediateTaskAsync(string handlerType, string? handlerMethod = null, string? businessData = null);

    /// <summary>
    /// 调度任务
    /// </summary>
    Task ScheduleTaskAsync(Guid taskDefinitionId);
}