namespace InfrastructureManager.Jobs;

using global::Quartz;

/// <summary>
/// Quartz 任务基类接口
/// </summary>
public interface IJobBase : IJob
{
    /// <summary>
    /// 任务名称
    /// </summary>
    string JobName { get; }

    /// <summary>
    /// 任务分组
    /// </summary>
    string JobGroup { get; }

    /// <summary>
    /// Cron 表达式
    /// </summary>
    string CronExpression { get; }

    /// <summary>
    /// 任务描述
    /// </summary>
    string Description { get; }
}