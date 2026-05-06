namespace BusinessManager.Extensions;

using BusinessManager.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 任务调度扩展方法
/// </summary>
public static class TaskSchedulingExtensions
{
    /// <summary>
    /// 注册 CAP 失败消息监控服务
    /// </summary>
    /// <remarks>
    /// BackgroundService 必须使用构造函数注入，因为 AddHostedService 不支持 Autofac 属性注入。
    /// 定期检查 CAP 失败消息并执行补偿操作。
    /// 注意：只有 CAP 启用时才注册此服务，否则会因为 CAP 数据表不存在而报错。
    /// </remarks>
    public static IServiceCollection AddCapFailedMessageMonitor(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var capSection = configuration.GetSection("Cap");
        var enabledValue = capSection.GetValue<bool?>("Enabled");
        bool isCapEnabled = enabledValue ?? false;

        if (isCapEnabled)
        {
            services.AddHostedService<CapFailedMessageMonitorService>();
        }

        return services;
    }
}