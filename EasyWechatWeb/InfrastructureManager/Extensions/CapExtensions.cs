namespace InfrastructureManager.Extensions;

using DotNetCore.CAP;
using InfrastructureManager.CAP;
using InfrastructureManager.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// CAP 服务注册扩展
/// </summary>
public static class CapExtensions
{
    /// <summary>
    /// 注册 CAP 事件总线服务
    /// </summary>
    public static IServiceCollection AddCapService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration.GetSection("Cap").Get<EasyCapOptions>();

        if (options == null || !options.Enabled)
        {
            return services;
        }

        // 获取数据库连接字符串
        var connectionString = GetConnectionString(configuration);

        services.AddCap(cap =>
        {
            // 使用 MySQL 存储
            cap.UseMySql(connectionString);

            // Kafka 传输（生产环境）
            cap.UseKafka(k => k.Servers = options.Kafka.Servers);

            // 设置消费者组
            cap.DefaultGroupName = options.GroupId;

            // 重试配置
            cap.FailedRetryCount = options.Retry.MaxRetryCount;
            cap.FailedRetryInterval = options.Retry.RetryInterval;
        });

        // 注册事件总线封装
        services.AddScoped<IEventBus, EventBus>();

        return services;
    }

    private static string GetConnectionString(IConfiguration configuration)
    {
        // 尝试从多种配置格式获取连接字符串
        var defaultConnection = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(defaultConnection))
        {
            return defaultConnection;
        }

        // 尝试从 MasterSlaveConnectionStrings 获取
        var masterSlaveSection = configuration.GetSection("MasterSlaveConnectionStrings");
        if (masterSlaveSection.Exists())
        {
            var firstSlave = masterSlaveSection.GetChildren().FirstOrDefault();
            if (firstSlave != null)
            {
                var connStr = firstSlave["ConnectionString"];
                if (!string.IsNullOrEmpty(connStr))
                {
                    return connStr;
                }
            }
        }

        throw new InvalidOperationException("数据库连接字符串未配置，请检查 appsettings.json 中的 ConnectionStrings 或 MasterSlaveConnectionStrings 配置");
    }
}