using Autofac;
using global::Quartz;
using global::Quartz.Spi;

namespace InfrastructureManager.Quartz;

/// <summary>
/// Autofac Job 工厂 - 让 Quartz 使用 Autofac 创建 Job
/// </summary>
/// <remarks>
/// 通过 Autofac 解析 Job 实例，支持构造函数注入。
/// Job 必须用 InstancePerDependency() 注册（Quartz 每次执行都会创建新实例）。
/// </remarks>
public class AutofacJobFactory : IJobFactory
{
    private readonly ILifetimeScope _container;

    public AutofacJobFactory(ILifetimeScope container)
    {
        _container = container;
    }

    /// <summary>
    /// 通过 Autofac 解析 Job 实例
    /// </summary>
    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        var jobType = bundle.JobDetail.JobType;
        return (IJob)_container.Resolve(jobType);
    }

    /// <summary>
    /// 返回 Job（Autofac 会自动管理生命周期）
    /// </summary>
    public void ReturnJob(IJob job)
    {
        // Autofac 会自动释放，无需手动处理
    }
}