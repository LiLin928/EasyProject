namespace BusinessManager.Infrastructure.Service;

using Autofac;
using BusinessManager.Infrastructure.IService;
using BusinessManager.Tasks;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Enums;
using global::Quartz;
using global::Quartz.Spi;
using Microsoft.Extensions.Logging;

/// <summary>
/// 任务执行监听器 - 自动记录执行日志到数据库
/// </summary>
/// <remarks>
/// 只对非动态任务（如 HelloWorldJob）记录日志。
/// DynamicTaskJob 已通过 TransactionalTaskExecutor 记录日志，此处跳过以避免重复记录。
/// </remarks>
public class JobExecutionListener : IJobListener
{
    private readonly ILifetimeScope _lifetimeScope;
    private readonly ILogger<JobExecutionListener>? _logger;

    public string Name => "JobExecutionListener";

    public JobExecutionListener(
        ILifetimeScope lifetimeScope,
        ILogger<JobExecutionListener>? logger = null)
    {
        _lifetimeScope = lifetimeScope;
        _logger = logger;
    }

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("任务被 vetoed: {JobName}", context.JobDetail.Key.Name);
        return Task.CompletedTask;
    }

    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        // DynamicTaskJob 由 TransactionalTaskExecutor 处理日志，跳过以避免重复
        if (context.JobDetail.JobType == typeof(DynamicTaskJob))
        {
            _logger?.LogInformation("动态任务 {JobName} 将由 TransactionalTaskExecutor 记录日志，跳过 JobListener 日志", context.JobDetail.Key.Name);
            return Task.CompletedTask;
        }

        var log = new TaskExecutionLog
        {
            Id = Guid.NewGuid(),
            JobName = context.JobDetail.Key.Name,
            JobGroup = context.JobDetail.Key.Group,
            Status = (int)TaskExecutionStatus.Running,
            StartTime = context.FireTimeUtc.LocalDateTime,
            TriggerType = context.Trigger is ISimpleTrigger ? (int)TriggerType.Manual : (int)TriggerType.Cron,
            InstanceId = context.Scheduler.SchedulerInstanceId
        };

        // 存储到 JobDetail 的 JobDataMap 中，以便后续使用
        context.JobDetail.JobDataMap["ExecutionLogId"] = log.Id.ToString();
        context.JobDetail.JobDataMap["StartTime"] = log.StartTime.ToString("O");

        _logger?.LogInformation("任务开始执行: {JobName} [{JobGroup}]", log.JobName, log.JobGroup);

        // 写入数据库
        using var scope = _lifetimeScope.BeginLifetimeScope();
        var logService = scope.ResolveOptional<ITaskExecutionLogService>();
        if (logService != null)
        {
            try
            {
                logService.LogExecutionAsync(log).Wait();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "写入任务执行日志失败");
            }
        }
        else
        {
            _logger?.LogWarning("ITaskExecutionLogService 未注册，无法写入执行日志");
        }

        return Task.CompletedTask;
    }

    public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
    {
        // DynamicTaskJob 由 TransactionalTaskExecutor 处理日志，跳过以避免重复
        if (context.JobDetail.JobType == typeof(DynamicTaskJob))
        {
            if (jobException != null)
            {
                _logger?.LogError("动态任务执行失败: {JobName} - {Message}", context.JobDetail.Key.Name, jobException.Message);
            }
            else
            {
                _logger?.LogInformation("动态任务执行完成: {JobName}", context.JobDetail.Key.Name);
            }
            return Task.CompletedTask;
        }

        var startTimeStr = context.JobDetail.JobDataMap.GetString("StartTime");
        var startTime = DateTime.TryParse(startTimeStr, out var parsedStartTime) ? parsedStartTime : DateTime.Now;
        var endTime = DateTime.Now;
        var duration = (long)(endTime - startTime).TotalMilliseconds;

        var logIdStr = context.JobDetail.JobDataMap.GetString("ExecutionLogId");
        var logId = Guid.TryParse(logIdStr, out var parsedId) ? parsedId : Guid.NewGuid();

        var status = jobException != null ? (int)TaskExecutionStatus.Failed : (int)TaskExecutionStatus.Success;

        if (jobException != null)
        {
            _logger?.LogError("任务执行失败: {JobName} - {Message}", context.JobDetail.Key.Name, jobException.Message);
        }
        else
        {
            _logger?.LogInformation("任务执行完成: {JobName}, 耗时: {Duration}ms", context.JobDetail.Key.Name, duration);
        }

        // 更新数据库日志
        using var scope = _lifetimeScope.BeginLifetimeScope();
        var logService = scope.ResolveOptional<ITaskExecutionLogService>();
        if (logService != null)
        {
            try
            {
                logService.UpdateExecutionResultAsync(
                    logId,
                    status,
                    jobException == null ? "执行成功" : null,
                    jobException?.Message,
                    jobException?.StackTrace
                ).Wait();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "更新任务执行日志失败");
            }
        }

        return Task.CompletedTask;
    }
}