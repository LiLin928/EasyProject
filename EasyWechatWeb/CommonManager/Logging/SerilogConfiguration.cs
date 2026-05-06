using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace CommonManager.Logging;

/// <summary>
/// Serilog 配置类，提供日志系统的初始化和配置方法
/// </summary>
/// <remarks>
/// Serilog 是一个结构化日志框架，支持多种输出目标（Sink）。
/// 该类提供默认的日志配置，包括：
/// - 控制台输出：便于开发调试
/// - 文件输出：按天滚动，最大 10MB，保留 30 天
/// - 日志级别控制：可过滤 Microsoft 和 System 框架日志
///
/// 使用前需在应用启动时调用 Configure 方法初始化日志系统。
/// </remarks>
/// <example>
/// <code>
/// // 在 Program.cs 中配置 Serilog
/// SerilogConfiguration.Configure();
///
/// // 或使用自定义配置
/// SerilogConfiguration.Configure(LogEventLevel.Information, "logs/app-.txt");
///
/// // 配置后使用 LogHelper 记录日志
/// LogHelper.Info("应用启动成功");
/// LogHelper.Error(ex, "数据库连接失败");
///
/// // 在应用退出时关闭日志
/// Log.CloseAndFlush();
/// </code>
/// </example>
public static class SerilogConfiguration
{
    /// <summary>
    /// 配置 Serilog 日志系统（使用默认配置）
    /// </summary>
    /// <param name="logPath">日志文件路径模板，默认为 "logs/log-.txt"
    /// 支持滚动间隔占位符，如 "log-.txt" 会生成 log-20240115.txt</param>
    /// <remarks>
    /// 默认配置说明：
    /// - 最低日志级别：Debug（记录所有级别日志）
    /// - Microsoft/System 框架日志：Warning 级别（减少框架日志噪音）
    /// - 控制台输出：带时间戳和级别的格式化输出
    /// - 文件输出：
    ///   - 滚动间隔：按天滚动（RollingInterval.Day）
    ///   - 文件大小限制：10MB
    ///   - 保留文件数：30 天
    ///   - 输出格式：带时区的时间戳 + 级别 + 消息
    /// </remarks>
    /// <example>
    /// <code>
    /// // 使用默认配置
    /// SerilogConfiguration.Configure();
    ///
    /// // 使用自定义路径
    /// SerilogConfiguration.Configure("logs/myapp-.txt");
    ///
    /// // 使用相对路径（相对于应用根目录）
    /// SerilogConfiguration.Configure("logs/app/log-.txt");
    /// </code>
    /// </example>
    public static void Configure(string logPath = "logs/log-.txt")
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                fileSizeLimitBytes: 10 * 1024 * 1024, // 10MB
                retainedFileCountLimit: 30
            )
            .CreateLogger();
    }

    /// <summary>
    /// 配置 Serilog 日志系统（使用自定义最低日志级别）
    /// </summary>
    /// <param name="minimumLevel">最低日志级别，低于此级别的日志不会被记录</param>
    /// <param name="logPath">日志文件路径模板，默认为 "logs/log-.txt"</param>
    /// <remarks>
    /// 日志级别说明（从低到高）：
    /// - Verbose：详细调试信息，通常仅用于开发调试
    /// - Debug：调试信息，用于开发环境问题排查
    /// - Information：常规信息，记录重要业务操作（生产环境推荐）
    /// - Warning：警告信息，记录可能的问题但不影响运行
    /// - Error：错误信息，记录运行时错误和异常
    /// - Fatal：致命错误，记录导致应用崩溃的严重错误
    ///
    /// 生产环境推荐使用 Information 级别，减少日志量同时保留关键信息。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 生产环境配置（仅记录 Information 及以上级别）
    /// SerilogConfiguration.Configure(LogEventLevel.Information);
    ///
    /// // 开发环境配置（记录所有级别）
    /// SerilogConfiguration.Configure(LogEventLevel.Debug);
    ///
    /// // 仅记录错误和致命错误（最小日志量）
    /// SerilogConfiguration.Configure(LogEventLevel.Error);
    ///
    /// // 自定义路径和级别
    /// SerilogConfiguration.Configure(LogEventLevel.Warning, "logs/warnings-.txt");
    /// </code>
    /// </example>
    public static void Configure(LogEventLevel minimumLevel, string logPath = "logs/log-.txt")
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(minimumLevel)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                fileSizeLimitBytes: 10 * 1024 * 1024,
                retainedFileCountLimit: 30
            )
            .CreateLogger();
    }

    /// <summary>
    /// 配置 Serilog 日志系统（包含 Elasticsearch Sink）
    /// </summary>
    /// <param name="logPath">日志文件路径模板，默认为 "logs/log-.txt"</param>
    /// <param name="esUrl">Elasticsearch 服务地址，如 "http://localhost:9200"</param>
    /// <param name="indexFormat">ES 索引格式，默认为 "easywechat-logs-{0:yyyy.MM.dd}"</param>
    /// <remarks>
    /// 配置说明：
    /// - Console Sink: 开发调试，输出到控制台
    /// - File Sink: 按天滚动文件，最大 10MB，保留 30 天
    /// - Elasticsearch Sink: 按天分索引存储日志，便于 Kibana 分析
    ///
    /// IndexFormat 使用 {0:yyyy.MM.dd} 占位符，自动按日期创建索引：
    /// - easywechat-logs-2026.04.07 → 今天的日志索引
    /// - easywechat-logs-2026.04.08 → 明天的日志索引
    /// </remarks>
    /// <example>
    /// <code>
    /// // 启用 Elasticsearch 日志
    /// SerilogConfiguration.ConfigureWithElasticsearch(
    ///     logPath: "logs/app-.txt",
    ///     esUrl: "http://localhost:9200",
    ///     indexFormat: "myapp-logs-{0:yyyy.MM.dd}"
    /// );
    /// </code>
    /// </example>
    public static void ConfigureWithElasticsearch(
        string logPath = "logs/log-.txt",
        string? esUrl = null,
        string? indexFormat = null)
    {
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            // 添加 HTTP 请求上下文 Enricher
            .Enrich.With<HttpRequestEnricher>()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                fileSizeLimitBytes: 10 * 1024 * 1024,
                retainedFileCountLimit: 30
            );

        // Elasticsearch Sink（可选）
        if (!string.IsNullOrEmpty(esUrl))
        {
            loggerConfig.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(esUrl))
            {
                IndexFormat = indexFormat ?? "easywechat-logs-{0:yyyy.MM.dd}",
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                NumberOfShards = 1,
                NumberOfReplicas = 0
            });
        }

        Log.Logger = loggerConfig.CreateLogger();
    }
}