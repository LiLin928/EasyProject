using Serilog;

namespace CommonManager.Helper;

/// <summary>
/// 日志封装帮助类，提供统一的日志记录方法
/// </summary>
/// <remarks>
/// 该类基于 Serilog 实现日志记录，封装了常用的日志级别和方法。
/// 支持信息、警告、错误、调试等不同级别的日志输出。
/// 需在应用启动时通过 SerilogConfiguration.Configure() 配置日志。
/// </remarks>
/// <example>
/// <code>
/// // 配置 Serilog（在 Program.cs 中）
/// SerilogConfiguration.Configure();
///
/// // 记录信息日志
/// LogHelper.Info("用户登录成功");
///
/// // 记录错误日志
/// LogHelper.Error(ex, "数据库连接失败");
///
/// // 记录 SQL 日志
/// LogHelper.MySqlInfo("SELECT * FROM Users");
/// </code>
/// </example>
public static class LogHelper
{
    /// <summary>
    /// 记录信息级别日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <remarks>
    /// 信息级别用于记录常规操作日志，如用户登录、接口调用等。
    /// 通常在生产环境中保持启用。
    /// </remarks>
    /// <example>
    /// <code>
    /// LogHelper.Info("系统启动完成");
    /// LogHelper.Info("用户张三登录成功");
    /// </code>
    /// </example>
    public static void Info(string message) => Log.Information(message);

    /// <summary>
    /// 记录信息级别日志（带参数）
    /// </summary>
    /// <param name="message">日志消息模板，支持 {0}、{1} 等占位符</param>
    /// <param name="args">参数数组，用于填充消息模板中的占位符</param>
    /// <remarks>
    /// 使用参数化日志可以减少字符串拼接开销，便于日志分析。
    /// </remarks>
    /// <example>
    /// <code>
    /// LogHelper.Info("用户 {0} 执行操作 {1}", userId, action);
    /// LogHelper.Info("处理请求耗时 {0}ms", elapsedMilliseconds);
    /// </code>
    /// </example>
    public static void Info(string message, params object[] args) => Log.Information(message, args);

    /// <summary>
    /// 记录警告级别日志
    /// </summary>
    /// <param name="message">警告消息</param>
    /// <remarks>
    /// 警告级别用于记录可能的问题但不影响正常运行的情况。
    /// 如缓存失效、配置缺失、性能下降等。
    /// </remarks>
    /// <example>
    /// <code>
    /// LogHelper.Warning("Redis 连接超时，使用本地缓存");
    /// LogHelper.Warning("配置项 Theme:Default 未设置，使用默认值");
    /// </code>
    /// </example>
    public static void Warning(string message) => Log.Warning(message);

    /// <summary>
    /// 记录警告级别日志（带参数）
    /// </summary>
    /// <param name="message">警告消息模板</param>
    /// <param name="args">参数数组</param>
    /// <remarks>
    /// 使用参数化方式记录警告，便于分析和追踪问题。
    /// </remarks>
    /// <example>
    /// <code>
    /// LogHelper.Warning("用户 {0} 尝试访问受限资源 {1}", userId, resourceId);
    /// </code>
    /// </example>
    public static void Warning(string message, params object[] args) => Log.Warning(message, args);

    /// <summary>
    /// 记录错误级别日志
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <remarks>
    /// 错误级别用于记录运行时错误和异常情况。
    /// 应及时处理和排查此类问题。
    /// </remarks>
    /// <example>
    /// <code>
    /// LogHelper.Error("支付服务不可用");
    /// LogHelper.Error("用户认证失败");
    /// </code>
    /// </example>
    public static void Error(string message) => Log.Error(message);

    /// <summary>
    /// 记录错误级别日志（带异常对象）
    /// </summary>
    /// <param name="ex">异常对象</param>
    /// <param name="message">错误消息</param>
    /// <remarks>
    /// 记录异常详细信息，包括堆栈跟踪，便于问题定位。
    /// 这是最常用的错误日志记录方式。
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     await ProcessOrderAsync(order);
    /// }
    /// catch (Exception ex)
    /// {
    ///     LogHelper.Error(ex, "订单处理失败，订单号: {0}", order.OrderNo);
    /// }
    /// </code>
    /// </example>
    public static void Error(Exception ex, string message) => Log.Error(ex, message);

    /// <summary>
    /// 记录错误级别日志（带参数）
    /// </summary>
    /// <param name="message">错误消息模板</param>
    /// <param name="args">参数数组</param>
    /// <remarks>
    /// 使用参数化方式记录错误消息。
    /// </remarks>
    /// <example>
    /// <code>
    /// LogHelper.Error("API 调用失败: {0}, 状态码: {1}", url, statusCode);
    /// </code>
    /// </example>
    public static void Error(string message, params object[] args) => Log.Error(message, args);

    /// <summary>
    /// 记录调试级别日志
    /// </summary>
    /// <param name="message">调试消息</param>
    /// <remarks>
    /// 调试级别用于开发调试，记录详细的程序运行信息。
    /// 通常在生产环境中禁用，仅用于开发和测试环境。
    /// </remarks>
    /// <example>
    /// <code>
    /// LogHelper.Debug("进入方法 GetUserById，参数: userId={0}", userId);
    /// LogHelper.Debug("SQL 执行结果: {0} 条记录", count);
    /// </code>
    /// </example>
    public static void Debug(string message) => Log.Debug(message);

    /// <summary>
    /// 记录 MySQL SQL 信息日志
    /// </summary>
    /// <param name="sql">SQL 语句</param>
    /// <remarks>
    /// 专门用于记录 SQL 执行日志，添加 "MySQL" 来源标记。
    /// 便于过滤和分析数据库相关日志。
    /// </remarks>
    /// <example>
    /// <code>
    /// LogHelper.MySqlInfo("SELECT * FROM Users WHERE Status = 'Active'");
    /// LogHelper.MySqlInfo("INSERT INTO Orders (OrderNo, Amount) VALUES ('123', 100.00)");
    /// </code>
    /// </example>
    public static void MySqlInfo(string sql) => Log.ForContext("Source", "MySQL").Information(sql);

    /// <summary>
    /// 记录 MySQL SQL 错误日志
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <remarks>
    /// 专门用于记录 SQL 执行错误，添加 "MySQL" 来源标记。
    /// 用于记录数据库连接失败、SQL 执行错误等问题。
    /// </remarks>
    /// <example>
    /// <code>
    /// LogHelper.MySqlError("SQL 执行失败: 连接超时");
    /// LogHelper.MySqlError("数据库连接失败: Connection refused");
    /// </code>
    /// </example>
    public static void MySqlError(string message) => Log.ForContext("Source", "MySQL").Error(message);
}