namespace BusinessManager.Infrastructure.IService;

using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Enums;

/// <summary>
/// 任务定义服务接口 - 任务管理公共方法
/// </summary>
public interface ITaskDefinitionService
{
    /// <summary>
    /// 获取待执行任务列表
    /// </summary>
    Task<List<TaskDefinitionDto>> GetPendingTasksAsync();

    /// <summary>
    /// 获取即时任务列表
    /// </summary>
    Task<List<TaskDefinitionDto>> GetImmediateTasksAsync();

    /// <summary>
    /// 根据 ID 获取任务
    /// </summary>
    Task<TaskDefinition?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建 Cron 任务
    /// </summary>
    Task<Guid> CreateCronTaskAsync(string name, string cron, string handlerType, string? description = null);

    /// <summary>
    /// 创建即时任务（业务触发）
    /// </summary>
    Task<Guid> CreateImmediateTaskAsync(string name, string handlerType, string? apiEndpoint = null, string? businessData = null);

    /// <summary>
    /// 创建周期任务（每天/每月）
    /// </summary>
    Task<Guid> CreatePeriodicTaskAsync(string name, ScheduleType scheduleType, int hour, int minute, string handlerType, int? dayOfMonth = null);

    /// <summary>
    /// 创建指定时间任务
    /// </summary>
    Task<Guid> CreateScheduledTaskAsync(string name, DateTime executeTime, string handlerType);

    /// <summary>
    /// 通过 DTO 创建任务
    /// </summary>
    Task<Guid> CreateTaskAsync(CreateTaskDto dto);

    /// <summary>
    /// 更新任务状态
    /// </summary>
    Task UpdateTaskStatusAsync(Guid taskId, TaskDefinitionStatus status);

    /// <summary>
    /// 更新下次执行时间
    /// </summary>
    Task UpdateNextExecuteTimeAsync(Guid taskId, DateTime? nextExecuteTime);

    /// <summary>
    /// 暂停任务
    /// </summary>
    Task PauseTaskAsync(Guid taskId);

    /// <summary>
    /// 恢复任务
    /// </summary>
    Task ResumeTaskAsync(Guid taskId);

    /// <summary>
    /// 取消任务
    /// </summary>
    Task CancelTaskAsync(Guid taskId);

    /// <summary>
    /// 手动触发任务
    /// </summary>
    Task TriggerTaskAsync(Guid taskId);

    /// <summary>
    /// 计算下次执行时间
    /// </summary>
    DateTime? CalculateNextExecuteTime(TaskDefinition task);

    /// <summary>
    /// 清理过期任务
    /// </summary>
    Task<int> ClearExpiredTasksAsync(int days);
}