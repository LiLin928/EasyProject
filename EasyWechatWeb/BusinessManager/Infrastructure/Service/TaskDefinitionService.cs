namespace BusinessManager.Infrastructure.Service;

using BusinessManager.Infrastructure.IService;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Enums;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

/// <summary>
/// 任务定义服务实现 - 提供任务管理公共方法
/// </summary>
public class TaskDefinitionService : ITaskDefinitionService
{
    /// <summary>
    /// 数据库客户端（属性注入）
    /// </summary>
    public ISqlSugarClient _db { get; set; } = null!;

    /// <summary>
    /// 日志器（属性注入）
    /// </summary>
    public ILogger<TaskDefinitionService> _logger { get; set; } = null!;

    /// <summary>
    /// 获取待执行任务列表
    /// </summary>
    public async Task<List<TaskDefinitionDto>> GetPendingTasksAsync()
    {
        var list = await _db.Queryable<TaskDefinition>()
            .Where(x => x.Status == (int)TaskDefinitionStatus.Pending || x.Status == (int)TaskDefinitionStatus.Scheduled)
            .Where(x => x.TaskType != (int)TaskType.Immediate) // 排除即时任务
            .OrderBy(x => x.Priority)
            .ToListAsync();

        return ConvertToDtoList(list);
    }

    /// <summary>
    /// 获取即时任务列表
    /// </summary>
    public async Task<List<TaskDefinitionDto>> GetImmediateTasksAsync()
    {
        var list = await _db.Queryable<TaskDefinition>()
            .Where(x => x.TaskType == (int)TaskType.Immediate && x.Status == (int)TaskDefinitionStatus.Pending)
            .OrderBy(x => x.Priority)
            .ToListAsync();

        return ConvertToDtoList(list);
    }

    /// <summary>
    /// 根据 ID 获取任务
    /// </summary>
    public async Task<TaskDefinition?> GetByIdAsync(Guid id)
    {
        return await _db.Queryable<TaskDefinition>()
            .Where(x => x.Id == id)
            .FirstAsync();
    }

    /// <summary>
    /// 创建 Cron 任务
    /// </summary>
    public async Task<Guid> CreateCronTaskAsync(string name, string cron, string handlerType, string? description = null)
    {
        var task = new TaskDefinition
        {
            TaskName = name,
            TaskType = (int)TaskType.Cron,
            CronExpression = cron,
            HandlerType = handlerType,
            Description = description,
            Status = (int)TaskDefinitionStatus.Pending,
            NextExecuteTime = CalculateNextExecuteTimeFromCron(cron)
        };

        await _db.Insertable(task).ExecuteCommandAsync();
        return task.Id;
    }

    /// <summary>
    /// 创建即时任务（业务触发）
    /// </summary>
    public async Task<Guid> CreateImmediateTaskAsync(string name, string handlerType, string? apiEndpoint = null, string? businessData = null)
    {
        var task = new TaskDefinition
        {
            TaskName = name,
            TaskType = (int)TaskType.Immediate,
            HandlerType = handlerType,
            ApiEndpoint = apiEndpoint,
            BusinessData = businessData,
            Status = (int)TaskDefinitionStatus.Pending,
            NextExecuteTime = DateTime.Now // 即时任务立即执行
        };

        await _db.Insertable(task).ExecuteCommandAsync();
        return task.Id;
    }

    /// <summary>
    /// 创建周期任务（每天/每月）
    /// </summary>
    public async Task<Guid> CreatePeriodicTaskAsync(string name, ScheduleType scheduleType, int hour, int minute, string handlerType, int? dayOfMonth = null)
    {
        var task = new TaskDefinition
        {
            TaskName = name,
            TaskType = (int)TaskType.Periodic,
            ScheduleType = (int)scheduleType,
            ExecuteHour = hour,
            ExecuteMinute = minute,
            DayOfMonth = dayOfMonth,
            HandlerType = handlerType,
            Status = (int)TaskDefinitionStatus.Pending,
            NextExecuteTime = CalculateNextExecuteTimeFromPeriodic(scheduleType, hour, minute, dayOfMonth)
        };

        await _db.Insertable(task).ExecuteCommandAsync();
        return task.Id;
    }

    /// <summary>
    /// 创建指定时间任务
    /// </summary>
    public async Task<Guid> CreateScheduledTaskAsync(string name, DateTime executeTime, string handlerType)
    {
        var task = new TaskDefinition
        {
            TaskName = name,
            TaskType = (int)TaskType.Periodic,
            ScheduleType = (int)ScheduleType.Specific,
            ExecuteTime = executeTime,
            HandlerType = handlerType,
            Status = (int)TaskDefinitionStatus.Pending,
            NextExecuteTime = executeTime
        };

        await _db.Insertable(task).ExecuteCommandAsync();
        return task.Id;
    }

    /// <summary>
    /// 通过 DTO 创建任务
    /// </summary>
    public async Task<Guid> CreateTaskAsync(CreateTaskDto dto)
    {
        var task = dto.Adapt<TaskDefinition>();
        task.Id = Guid.NewGuid();
        task.CreateTime = DateTime.Now;
        task.Status = (int)TaskDefinitionStatus.Pending;
        task.NextExecuteTime = CalculateNextExecuteTime(task);

        await _db.Insertable(task).ExecuteCommandAsync();
        return task.Id;
    }

    /// <summary>
    /// 更新任务状态
    /// </summary>
    public async Task UpdateTaskStatusAsync(Guid taskId, TaskDefinitionStatus status)
    {
        await _db.Updateable<TaskDefinition>()
            .SetColumns(x => x.Status == (int)status)
            .SetColumns(x => x.UpdateTime == DateTime.Now)
            .Where(x => x.Id == taskId)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新下次执行时间
    /// </summary>
    public async Task UpdateNextExecuteTimeAsync(Guid taskId, DateTime? nextExecuteTime)
    {
        await _db.Updateable<TaskDefinition>()
            .SetColumns(x => x.NextExecuteTime == nextExecuteTime)
            .SetColumns(x => x.UpdateTime == DateTime.Now)
            .Where(x => x.Id == taskId)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 暂停任务
    /// </summary>
    public async Task PauseTaskAsync(Guid taskId)
    {
        await UpdateTaskStatusAsync(taskId, TaskDefinitionStatus.Paused);
        _logger.LogInformation($"任务已暂停: {taskId}");
    }

    /// <summary>
    /// 恢复任务
    /// </summary>
    public async Task ResumeTaskAsync(Guid taskId)
    {
        var task = await GetByIdAsync(taskId);
        if (task != null)
        {
            await UpdateTaskStatusAsync(taskId, TaskDefinitionStatus.Pending);
            await UpdateNextExecuteTimeAsync(taskId, CalculateNextExecuteTime(task));
            _logger.LogInformation($"任务已恢复: {taskId}");
        }
    }

    /// <summary>
    /// 取消任务
    /// </summary>
    public async Task CancelTaskAsync(Guid taskId)
    {
        await UpdateTaskStatusAsync(taskId, TaskDefinitionStatus.Completed);
        _logger.LogInformation($"任务已取消: {taskId}");
    }

    /// <summary>
    /// 手动触发任务
    /// </summary>
    public async Task TriggerTaskAsync(Guid taskId)
    {
        await UpdateTaskStatusAsync(taskId, TaskDefinitionStatus.Scheduled);
        await UpdateNextExecuteTimeAsync(taskId, DateTime.Now);
        _logger.LogInformation($"任务已手动触发: {taskId}");
    }

    /// <summary>
    /// 计算下次执行时间
    /// </summary>
    public DateTime? CalculateNextExecuteTime(TaskDefinition task)
    {
        return task.TaskType switch
        {
            (int)TaskType.Cron => CalculateNextExecuteTimeFromCron(task.CronExpression!),
            (int)TaskType.Immediate => DateTime.Now,
            (int)TaskType.Periodic => task.ScheduleType switch
            {
                (int)ScheduleType.Daily => CalculateNextExecuteTimeFromPeriodic(ScheduleType.Daily, task.ExecuteHour ?? 0, task.ExecuteMinute ?? 0, null),
                (int)ScheduleType.Monthly => CalculateNextExecuteTimeFromPeriodic(ScheduleType.Monthly, task.ExecuteHour ?? 0, task.ExecuteMinute ?? 0, task.DayOfMonth),
                (int)ScheduleType.Specific => task.ExecuteTime,
                _ => null
            },
            _ => null
        };
    }

    /// <summary>
    /// 清理过期任务
    /// </summary>
    public async Task<int> ClearExpiredTasksAsync(int days)
    {
        var cutoffDate = DateTime.Now.AddDays(-days);
        return await _db.Deleteable<TaskDefinition>()
            .Where(x => x.Status == (int)TaskDefinitionStatus.Completed && x.CreateTime < cutoffDate)
            .ExecuteCommandAsync();
    }

    #region 私有方法

    private List<TaskDefinitionDto> ConvertToDtoList(List<TaskDefinition> list)
    {
        var result = list.Adapt<List<TaskDefinitionDto>>();
        foreach (var item in result)
        {
            item.TaskTypeText = GetTaskTypeText(item.TaskType);
            item.StatusText = GetStatusText(item.Status);
        }
        return result;
    }

    private DateTime? CalculateNextExecuteTimeFromCron(string cronExpression)
    {
        try
        {
            // 简化实现：基于当前时间计算下次执行时间
            // 实际项目中应使用 Quartz 的 CronExpression 类
            var now = DateTime.Now;
            // 这里返回一个简单的估算值
            return now.AddHours(1); // 默认 1 小时后
        }
        catch
        {
            return null;
        }
    }

    private DateTime? CalculateNextExecuteTimeFromPeriodic(ScheduleType scheduleType, int hour, int minute, int? dayOfMonth)
    {
        var now = DateTime.Now;

        if (scheduleType == ScheduleType.Daily)
        {
            var todayExecute = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
            if (todayExecute > now)
            {
                return todayExecute;
            }
            return todayExecute.AddDays(1);
        }

        if (scheduleType == ScheduleType.Monthly && dayOfMonth.HasValue)
        {
            var thisMonthExecute = new DateTime(now.Year, now.Month, dayOfMonth.Value, hour, minute, 0);
            if (thisMonthExecute > now)
            {
                return thisMonthExecute;
            }
            // 下个月
            var nextMonth = now.Month == 12 ? 1 : now.Month + 1;
            var nextYear = now.Month == 12 ? now.Year + 1 : now.Year;
            return new DateTime(nextYear, nextMonth, dayOfMonth.Value, hour, minute, 0);
        }

        return null;
    }

    private string GetTaskTypeText(int taskType)
    {
        return taskType switch
        {
            (int)TaskType.Cron => "Cron定时",
            (int)TaskType.Immediate => "即时执行",
            (int)TaskType.Periodic => "周期任务",
            _ => "未知"
        };
    }

    private string GetStatusText(int status)
    {
        return status switch
        {
            (int)TaskDefinitionStatus.Pending => "待调度",
            (int)TaskDefinitionStatus.Scheduled => "已调度",
            (int)TaskDefinitionStatus.Paused => "暂停",
            (int)TaskDefinitionStatus.Completed => "已完成",
            (int)TaskDefinitionStatus.Failed => "失败",
            _ => "未知"
        };
    }

    #endregion
}