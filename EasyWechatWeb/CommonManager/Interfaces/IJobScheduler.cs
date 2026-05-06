namespace CommonManager.Interfaces;

/// <summary>
/// 任务调度接口，用于管理和执行定时任务（Quartz 预留）
/// </summary>
/// <remarks>
/// 该接口定义了定时任务的管理方法，为后续集成 Quartz.NET 任务调度框架预留。
/// Quartz.NET 是一个功能强大的作业调度框架，支持 Cron 表达式、持久化、集群等特性。
///
/// 应用场景：
/// - 定时数据清理（如清理过期日志）
/// - 定时报表生成（如每日销售报表）
/// - 定时状态检查（如订单超时自动取消）
/// - 定时数据同步（如从外部系统同步数据）
/// - 定时消息发送（如定时推送通知）
///
/// 实现后可支持：
/// - Cron 表达式灵活配置执行时间
/// - 任务持久化，应用重启后自动恢复
/// - 集群部署，任务在多节点间协调
/// - 任务状态监控和管理
/// - 错过执行的任务补偿处理
///
/// 预留此接口便于后续集成 Quartz 框架，实现完整的任务调度能力。
/// </remarks>
/// <example>
/// <code>
/// // 注册定时任务（后续实现）
/// public class JobService : IJobService
/// {
///     private readonly IJobScheduler _scheduler;
///
///     public async Task InitializeJobs()
///     {
///         // 添加每日清理任务
///         await _scheduler.AddJobAsync&lt;CleanupJob&gt;("cleanup-job", "0 0 2 * * ?");  // 每天凌晨2点
///
///         // 添加订单超时检查任务
///         await _scheduler.AddJobAsync&lt;OrderTimeoutJob&gt;("order-check", "0 */5 * * * ?");  // 每5分钟
///     }
/// }
///
/// // 定义任务实现
/// public class CleanupJob : IJob
/// {
///     public async Task ExecuteAsync(JobContext context)
///     {
///         await CleanupExpiredLogs();
///     }
/// }
/// </code>
/// </example>
public interface IJobScheduler
{
    /// <summary>
    /// 添加定时任务到调度器
    /// </summary>
    /// <typeparam name="TJob">任务类型，必须实现 IJob 接口</typeparam>
    /// <param name="jobName">任务名称，用于唯一标识任务，如 "cleanup-job"、"report-generator"</param>
    /// <param name="cronExpression">Cron 表达式，定义任务的执行时间规则</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// Cron 表达式格式（6 或 7 位）：
    /// - 秒 分 时 日 月 周 [年]
    /// - 示例："0 0 12 * * ?" 表示每天中午12点执行
    ///
    /// 常用 Cron 表达式：
    /// - "0 0 2 * * ?"：每天凌晨 2 点
    /// - "0 */5 * * * ?"：每 5 分钟
    /// - "0 0 12 * * MON-FRI"：周一至周五中午 12 点
    /// - "0 0 0 1 * ?"：每月 1 号凌晨 0 点
    ///
    /// 特殊字符：
    /// - *：所有值
    /// - ?：不指定（日和周互斥）
    /// - -：范围，如 1-5
    /// - /：增量，如 0/15 表示从 0 开始每 15
    /// - ,：列表，如 1,3,5
    /// </remarks>
    /// <example>
    /// <code>
    /// // 添加每天凌晨 2 点执行的清理任务
    /// await _scheduler.AddJobAsync&lt;CleanupJob&gt;("cleanup-job", "0 0 2 * * ?");
    ///
    /// // 添加每小时执行的统计任务
    /// await _scheduler.AddJobAsync&lt;StatisticsJob&gt;("stats-job", "0 0 * * * ?");
    ///
    /// // 添加每 5 分钟执行的检查任务
    /// await _scheduler.AddJobAsync&lt;HealthCheckJob&gt;("health-check", "0 */5 * * * ?");
    /// </code>
    /// </example>
    Task AddJobAsync<TJob>(string jobName, string cronExpression) where TJob : IJob;

    /// <summary>
    /// 从调度器中删除定时任务
    /// </summary>
    /// <param name="jobName">任务名称</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// 删除任务后，任务将不再执行。
    /// 已在执行的任务不会被中断。
    /// 删除不存在的任务不会报错。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 删除不再需要的任务
    /// await _scheduler.RemoveJobAsync("cleanup-job");
    /// await _scheduler.RemoveJobAsync("temporary-task");
    /// </code>
    /// </example>
    Task RemoveJobAsync(string jobName);

    /// <summary>
    /// 暂停定时任务的执行
    /// </summary>
    /// <param name="jobName">任务名称</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// 暂停后任务停止执行，但保留在调度器中。
    /// 可通过 ResumeJobAsync 恢复执行。
    /// 暂停期间错过的任务，恢复后是否补偿取决于配置。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 暂停任务（如系统维护期间）
    /// await _scheduler.PauseJobAsync("cleanup-job");
    ///
    /// // 维护完成后恢复
    /// await _scheduler.ResumeJobAsync("cleanup-job");
    /// </code>
    /// </example>
    Task PauseJobAsync(string jobName);

    /// <summary>
    /// 恢复暂停的定时任务
    /// </summary>
    /// <param name="jobName">任务名称</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// 恢复后任务按原 Cron 表达式继续执行。
    /// 暂停期间错过的执行，Quartz 可配置是否补偿。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 恢复暂停的任务
    /// await _scheduler.ResumeJobAsync("cleanup-job");
    /// </code>
    /// </example>
    Task ResumeJobAsync(string jobName);

    /// <summary>
    /// 立即执行一次任务（不考虑 Cron 表达式）
    /// </summary>
    /// <param name="jobName">任务名称</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// 立即触发任务执行一次，不影响原有的调度计划。
    /// 常用于：
    /// - 手动触发执行
    /// - 测试任务是否正常工作
    /// - 补偿错过的执行
    /// </remarks>
    /// <example>
    /// <code>
    /// // 立即执行清理任务（手动触发）
    /// await _scheduler.TriggerJobAsync("cleanup-job");
    ///
    /// // 立即执行报表生成（临时需要）
    /// await _scheduler.TriggerJobAsync("report-generator");
    /// </code>
    /// </example>
    Task TriggerJobAsync(string jobName);
}

/// <summary>
/// 任务接口，所有定时任务必须实现此接口
/// </summary>
/// <remarks>
/// 实现 IJob 接口的类会被 Quartz 调度器识别为可执行的任务。
/// ExecuteAsync 方法包含具体的任务执行逻辑。
/// </remarks>
/// <example>
/// <code>
/// public class DailyReportJob : IJob
/// {
///     private readonly IReportService _reportService;
///
///     public DailyReportJob(IReportService reportService)
///     {
///         _reportService = reportService;
///     }
///
///     public async Task ExecuteAsync(JobContext context)
///     {
///         await _reportService.GenerateDailyReport();
///     }
/// }
/// </code>
/// </example>
public interface IJob
{
    /// <summary>
    /// 执行任务的具体逻辑
    /// </summary>
    /// <param name="context">任务上下文，包含任务名称和参数</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// ExecuteAsync 在任务触发时被调用。
    /// 任务执行时间过长可能影响下次执行，建议异步处理耗时操作。
    /// 异常处理：
    /// - 抛出的异常会被 Quartz 捕获并记录
    /// - 可配置重试策略
    /// </remarks>
    /// <example>
    /// <code>
    /// public async Task ExecuteAsync(JobContext context)
    /// {
    ///     var jobName = context.JobName;
    ///     var userId = context.Parameters.TryGetValue("userId", out var id) ? (int)id : 0;
    ///
    ///     try
    ///     {
    ///         // 执行任务逻辑
    ///         await ProcessAsync(userId);
    ///     }
    ///     catch (Exception ex)
    ///     {
    ///         // 记录错误但不中断调度
    ///         LogHelper.Error(ex, $"任务 {jobName} 执行失败");
    ///     }
    /// }
    /// </code>
    /// </example>
    Task ExecuteAsync(JobContext context);
}

/// <summary>
/// 任务上下文，包含任务执行时的相关信息
/// </summary>
/// <remarks>
/// JobContext 在任务执行时传入，提供任务名称和自定义参数。
/// 用于任务间传递数据和获取执行上下文。
/// </remarks>
/// <example>
/// <code>
/// // 任务执行时获取上下文
/// public async Task ExecuteAsync(JobContext context)
/// {
///     Console.WriteLine($"正在执行任务: {context.JobName}");
///
///     if (context.Parameters.TryGetValue("batchSize", out var size))
///     {
    ///         var batchSize = (int)size;
    ///         await ProcessBatchAsync(batchSize);
    ///     }
    /// }
///
/// // 触发任务时传递参数
    /// await _scheduler.TriggerJobAsync("process-job");  // 参数通过 JobDataMap 设置
/// </code>
/// </example>
public class JobContext
{
    /// <summary>
    /// 任务名称，标识当前执行的任务
    /// </summary>
    /// <remarks>
    /// 用于日志记录和任务区分。
    /// </remarks>
    public string JobName { get; set; } = string.Empty;

    /// <summary>
    /// 任务参数字典，包含任务执行时需要的自定义参数
    /// </summary>
    /// <remarks>
    /// 参数在任务添加或触发时设置，执行时读取。
    /// 常见参数类型：int、string、bool、DateTime 等。
    /// </remarks>
    public Dictionary<string, object> Parameters { get; set; } = new();
}