namespace BusinessManager.Extensions;

using Autofac;
using BusinessManager.Infrastructure.Service;
using BusinessManager.Tasks;
using global::Quartz;
using global::Quartz.Impl;
using global::Quartz.Spi;
using InfrastructureManager.Jobs;
using InfrastructureManager.Options;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Quartz 服务注册扩展（使用 Autofac）
/// </summary>
public static class QuartzExtensions
{
    /// <summary>
    /// 注册 Quartz 任务调度服务到 Autofac 容器
    /// </summary>
    public static ContainerBuilder AddQuartzService(
        this ContainerBuilder containerBuilder,
        IConfiguration configuration)
    {
        var options = configuration.GetSection("Quartz").Get<EasyQuartzOptions>();

        if (options == null || !options.Enabled)
        {
            return containerBuilder;
        }

        // 1. 注册 AutofacJobFactory（让 Quartz 使用 Autofac 创建 Job）
        containerBuilder.RegisterType<InfrastructureManager.Quartz.AutofacJobFactory>()
            .As<IJobFactory>()
            .SingleInstance();

        // 2. 注册 IScheduler（单例）
        containerBuilder.Register(ctx =>
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().Result;

            // 绑定 Autofac JobFactory
            scheduler.JobFactory = ctx.Resolve<IJobFactory>();

            return scheduler;
        }).As<IScheduler>().SingleInstance();

        // 3. 注册 Job 类（每次执行创建新实例）
        containerBuilder.RegisterType<HelloWorldJob>().InstancePerDependency();
        containerBuilder.RegisterType<DynamicTaskJob>().InstancePerDependency();

        // 4. 注册 JobExecutionListener（全局监听器）
        containerBuilder.RegisterType<JobExecutionListener>()
            .As<IJobListener>()
            .SingleInstance();

        return containerBuilder;
    }

    /// <summary>
    /// 启动 Quartz 调度器
    /// </summary>
    public static async Task StartQuartzScheduler(this ILifetimeScope lifetimeScope)
    {
        var scheduler = lifetimeScope.Resolve<IScheduler>();
        await scheduler.Start();
    }
}