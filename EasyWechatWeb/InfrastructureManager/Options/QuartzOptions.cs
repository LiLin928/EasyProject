namespace InfrastructureManager.Options;

/// <summary>
/// Quartz 配置选项
/// </summary>
public class EasyQuartzOptions
{
    /// <summary>
    /// 是否启用任务调度（默认禁用）
    /// </summary>
    /// <remarks>
    /// 默认值设为 false，避免配置缺失时意外启用导致错误
    /// </remarks>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// 调度器名称
    /// </summary>
    public string SchedulerName { get; set; } = "EasyWeChatScheduler";

    /// <summary>
    /// 线程池大小
    /// </summary>
    public int ThreadCount { get; set; } = 10;

    /// <summary>
    /// 任务配置列表
    /// </summary>
    public List<JobConfig> Jobs { get; set; } = new();
}

/// <summary>
/// 单个任务配置
/// </summary>
public class JobConfig
{
    /// <summary>
    /// 任务名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 任务分组
    /// </summary>
    public string Group { get; set; } = string.Empty;

    /// <summary>
    /// Cron 表达式
    /// </summary>
    public string CronExpression { get; set; } = string.Empty;

    /// <summary>
    /// 任务描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; } = true;
}