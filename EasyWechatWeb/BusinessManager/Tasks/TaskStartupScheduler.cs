namespace BusinessManager.Tasks;

using BusinessManager.Infrastructure.IService;
using BusinessManager.Infrastructure.Service;
using global::Quartz;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz.Impl.Matchers;


/// <summary>
/// 任务启动调度器 - 程序启动时检查并调度数据库中的待执行任务
/// </summary>
/// <remarks>
/// IHostedService 使用 Autofac 构造函数注入
/// </remarks>
public class TaskStartupScheduler : IHostedService
{
    private readonly ITaskDefinitionService _taskService;
    private readonly ITaskExecutorService _executorService;
    private readonly IScheduler _scheduler;
    private readonly IJobListener _jobListener;
    private readonly ILogger<TaskStartupScheduler> _logger;

    public TaskStartupScheduler(
        ITaskDefinitionService taskService,
        ITaskExecutorService executorService,
        IScheduler scheduler,
        IJobListener jobListener,
        ILogger<TaskStartupScheduler> logger)
    {
        _taskService = taskService;
        _executorService = executorService;
        _scheduler = scheduler;
        _jobListener = jobListener;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("开始检查数据库中的待执行任务...");

        try
        {
            // 添加全局 JobListener
            _scheduler.ListenerManager.AddJobListener(_jobListener);

            // 启动 Quartz 调度器
            if (!_scheduler.IsStarted)
            {
                await _scheduler.Start(cancellationToken);
                _logger.LogInformation("Quartz 调度器已启动");
            }

            // 1. 检查待调度的周期任务
            var pendingTasks = await _taskService.GetPendingTasksAsync();
            foreach (var task in pendingTasks)
            {
                try
                {
                    await ScheduleTaskAsync(task, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "调度任务失败: {TaskName}, Cron表达式: {CronExpression}", task.TaskName, task.CronExpression);
                }
            }

            _logger.LogInformation($"已调度 {pendingTasks.Count} 个周期任务");

            // 2. 检查即时任务并立即执行
            var immediateTasks = await _taskService.GetImmediateTasksAsync();
            foreach (var task in immediateTasks)
            {
                _logger.LogInformation($"执行即时任务: {task.TaskName}");
                await _executorService.ExecuteTaskAsync(task.Id);
            }

            _logger.LogInformation($"已执行 {immediateTasks.Count} 个即时任务");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "任务启动检查失败");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("任务启动调度器停止");
        return _scheduler.Shutdown(cancellationToken);
    }

    private async Task ScheduleTaskAsync(EasyWeChatModels.Dto.TaskDefinitionDto task, CancellationToken cancellationToken)
    {
        var jobKey = new JobKey(task.TaskName, task.TaskGroup);

        // 检查任务是否已存在
        if (await _scheduler.CheckExists(jobKey, cancellationToken))
        {
            _logger.LogInformation($"任务已存在: {task.TaskName}");
            return;
        }

        // 创建动态任务
        var job = JobBuilder.Create<DynamicTaskJob>()
            .WithIdentity(jobKey)
            .WithDescription(task.Description)
            .UsingJobData("TaskDefinitionId", task.Id.ToString())
            .Build();

        // 根据任务类型创建触发器
        ITrigger trigger;
        if (task.TaskType == (int)EasyWeChatModels.Enums.TaskType.Cron && !string.IsNullOrEmpty(task.CronExpression))
        {
            // Quartz 需要 6 位 Cron 表达式（秒 分 时 日 月 周）
            // 前端可能生成 5 位表达式，需要转换
            var quartzCron = ConvertToQuartzCron(task.CronExpression);

            _logger.LogInformation($"任务 {task.TaskName} 原始Cron: {task.CronExpression}, 转换后: {quartzCron}");

            // 验证 Cron 表达式有效性
            try
            {
                var cronExpression = new CronExpression(quartzCron);
                var testNextTime = cronExpression.GetNextValidTimeAfter(DateTime.UtcNow);
                if (testNextTime == null)
                {
                    _logger.LogError($"任务 {task.TaskName} 的 Cron 表达式 '{quartzCron}' 无法计算出下次执行时间，表达式可能无效");
                    return;
                }
                _logger.LogInformation($"任务 {task.TaskName} 验证通过，下次触发时间(UTC): {testNextTime}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"任务 {task.TaskName} 的 Cron 表达式 '{quartzCron}' 无效");
                return;
            }

            trigger = TriggerBuilder.Create()
                .WithIdentity($"{task.TaskName}-trigger", task.TaskGroup)
                .WithCronSchedule(quartzCron)
                .Build();
        }
        else if (task.TaskType == (int)EasyWeChatModels.Enums.TaskType.Periodic && task.NextExecuteTime.HasValue)
        {
            trigger = TriggerBuilder.Create()
                .WithIdentity($"{task.TaskName}-trigger", task.TaskGroup)
                .StartAt(task.NextExecuteTime.Value)
                .Build();
        }
        else
        {
            _logger.LogWarning($"任务 {task.TaskName} 缺少有效的调度配置");
            return;
        }

        // 先调度任务
        await _scheduler.ScheduleJob(job, trigger, cancellationToken);

        // 调度后获取 Trigger 并计算下次触发时间
        var triggerKey = new TriggerKey($"{task.TaskName}-trigger", task.TaskGroup);
        var scheduledTrigger = await _scheduler.GetTrigger(triggerKey, cancellationToken);
        var nextFireTime = scheduledTrigger?.GetNextFireTimeUtc();
        _logger.LogInformation($"任务 {task.TaskName} Quartz下次触发时间(UTC): {nextFireTime?.ToString() ?? "null"}");
        _logger.LogInformation($"任务 {task.TaskName} Quartz下次触发时间(Local): {nextFireTime?.LocalDateTime ?? DateTime.MinValue}");

        // 验证调度结果
        var scheduledJobs = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(task.TaskGroup));
        var scheduledTriggers = await _scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(task.TaskGroup));
        _logger.LogInformation($"调度后验证 - JobGroup '{task.TaskGroup}' 中的 Job 数量: {scheduledJobs.Count}, Trigger 数量: {scheduledTriggers.Count}");

        var triggerState = await _scheduler.GetTriggerState(new TriggerKey($"{task.TaskName}-trigger", task.TaskGroup));
        _logger.LogInformation($"任务 {task.TaskName} Trigger 状态: {triggerState}");

        await _taskService.UpdateTaskStatusAsync(task.Id, EasyWeChatModels.Enums.TaskDefinitionStatus.Scheduled);

        _logger.LogInformation($"任务已调度完成: {task.TaskName}");
    }

    /// <summary>
    /// 将标准 5 位 Cron 表达式转换为 Quartz 6 位格式
    /// </summary>
    /// <remarks>
    /// 标准 Cron: 分 时 日 月 周
    /// Quartz Cron: 秒 分 时 日 月 周 [年]
    ///
    /// 转换规则：
    /// - 在开头添加秒字段（默认为 0）
    /// - 如果日字段是 * 且周字段是 *，Quartz 要求周用 ?
    /// - 如果日字段是数字且周字段是 *，周字段改为 ?
    /// - 如果周字段有值且日字段是 *，日字段改为 ?
    ///
    /// 常见表达式示例：
    /// - 每分钟：标准 `* * * * *` → Quartz `0 * * * * ?`
    /// - 每小时：标准 `0 * * * *` → Quartz `0 0 * * * ?`
    /// - 每天凌晨2点：标准 `0 2 * * *` → Quartz `0 0 2 * * ?`
    /// - 每周一上午10点：标准 `0 10 * * 1` → Quartz `0 0 10 ? * 1`
    ///
    /// 注意：表达式 `0 0/1 * * *`（第0分钟，每隔1小时）= 每小时执行，不是每分钟！
    /// </remarks>
    private string ConvertToQuartzCron(string cron)
    {
        var parts = cron.Trim().Split(' ');

        // 已经是 6 位或 7 位，验证并返回
        if (parts.Length >= 6)
        {
            // 验证是否为有效的 Quartz 格式
            _logger.LogInformation($"表达式 '{cron}' 已是 Quartz 格式（{parts.Length}位），直接使用");
            return cron;
        }

        // 5 位表达式转换
        if (parts.Length == 5)
        {
            var minute = parts[0];
            var hour = parts[1];
            var day = parts[2];
            var month = parts[3];
            var weekday = parts[4];

            _logger.LogInformation($"5位表达式解析: 分={minute}, 时={hour}, 日={day}, 月={month}, 周={weekday}");

            // Quartz 冲突规则：日和周不能同时为 *
            // 如果日是 * 且周是 *，周改为 ?
            if (day == "*" && weekday == "*")
            {
                weekday = "?";
            }
            // 如果日有值且周是 *，周改为 ?
            else if (day != "*" && day != "?" && weekday == "*")
            {
                weekday = "?";
            }
            // 如果周有值且日是 *，日改为 ?
            else if (weekday != "*" && weekday != "?" && day == "*")
            {
                day = "?";
            }

            var quartzCron = $"0 {minute} {hour} {day} {month} {weekday}";
            _logger.LogInformation($"转换结果: '{quartzCron}'（秒=0, 分={minute}, 时={hour}, 日={day}, 月={month}, 周={weekday})");

            return quartzCron;
        }

        // 其他情况（少于5位），尝试补秒字段
        _logger.LogWarning($"表达式 '{cron}' 格式异常（{parts.Length}位），尝试补秒字段");
        return "0 " + cron;
    }
}

/// <summary>
/// 动态任务执行器 - 根据数据库配置执行任务
/// </summary>
/// <remarks>
/// Quartz Job 使用构造函数注入，因为 Quartz DI JobFactory 不支持 Autofac 属性注入
/// </remarks>
public class DynamicTaskJob : IJob
{
    private readonly ITaskExecutorService _executorService;
    private readonly ILogger<DynamicTaskJob> _logger;

    public DynamicTaskJob(
        ITaskExecutorService executorService,
        ILogger<DynamicTaskJob> logger)
    {
        _executorService = executorService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var taskIdStr = context.JobDetail.JobDataMap.GetString("TaskDefinitionId");
        if (string.IsNullOrEmpty(taskIdStr) || !Guid.TryParse(taskIdStr, out Guid taskId))
        {
            _logger.LogWarning("动态任务缺少 TaskDefinitionId");
            return;
        }

        _logger.LogInformation("开始执行动态任务: {TaskId}", taskId);
        await _executorService.ExecuteTaskAsync(taskId);
    }
}