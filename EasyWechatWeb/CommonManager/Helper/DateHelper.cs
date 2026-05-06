namespace CommonManager.Helper;

/// <summary>
/// 日期处理帮助类，提供时间戳转换和日期格式化方法
/// </summary>
/// <remarks>
/// 该类提供常用的日期时间操作方法，包括获取时间戳、时间戳与日期转换、日期格式化等。
/// 时间戳使用 UTC 时间，确保跨时区一致性。
/// </remarks>
/// <example>
/// <code>
/// // 获取当前时间戳
/// var timestamp = DateHelper.GetTimestamp();
///
/// // 时间戳转日期
/// var date = DateHelper.TimestampToDate(timestamp);
///
/// // 格式化日期
/// var formatted = DateHelper.FormatDate(date, "yyyy年MM月dd日");
/// </code>
/// </example>
public static class DateHelper
{
    /// <summary>
    /// 获取当前 UTC 时间的毫秒时间戳
    /// </summary>
    /// <returns>Unix 时间戳（毫秒）</returns>
    /// <remarks>
    /// 毫秒时间戳适用于需要高精度时间的场景，如 API 响应的 Timestamp 字段。
    /// 使用 UTC 时间，不受本地时区影响。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 获取毫秒时间戳
    /// var timestamp = DateHelper.GetTimestamp();
    /// Console.WriteLine($"时间戳: {timestamp}");
    ///
    /// // 用于 API 响应
    /// var response = new ApiResponse
    /// {
    ///     Timestamp = DateHelper.GetTimestamp()
    /// };
    /// </code>
    /// </example>
    public static long GetTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 获取当前 UTC 时间的秒时间戳
    /// </summary>
    /// <returns>Unix 时间戳（秒）</returns>
    /// <remarks>
    /// 秒时间戳适用于精度要求不高的场景，如缓存过期时间计算。
    /// 使用 UTC 时间，不受本地时区影响。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 获取秒时间戳
    /// var timestamp = DateHelper.GetTimestampSeconds();
    ///
    /// // 计算缓存过期时间
    /// var expireTime = DateHelper.GetTimestampSeconds() + 3600; // 1 小时后过期
    /// </code>
    /// </example>
    public static long GetTimestampSeconds()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    /// <summary>
    /// 将毫秒时间戳转换为本地日期时间
    /// </summary>
    /// <param name="timestamp">Unix 时间戳（毫秒）</param>
    /// <returns>转换后的本地 DateTime 对象</returns>
    /// <remarks>
    /// 将 UTC 时间戳转换为本地时区的日期时间。
    /// 适用于展示用户可见的时间，如创建时间、更新时间等。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 从 API 响应时间戳转换为可显示日期
    /// var timestamp = 1704067200000;
    /// var date = DateHelper.TimestampToDate(timestamp);
    /// Console.WriteLine($"日期: {date}");
    ///
    /// // 格式化显示
    /// var formatted = DateHelper.FormatDate(date);
    /// Console.WriteLine($"格式化日期: {formatted}");
    /// </code>
    /// </example>
    public static DateTime TimestampToDate(long timestamp)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
    }

    /// <summary>
    /// 将日期时间转换为毫秒时间戳
    /// </summary>
    /// <param name="date">DateTime 对象</param>
    /// <returns>Unix 时间戳（毫秒）</returns>
    /// <remarks>
    /// 将日期转换为 UTC 时间戳，用于存储或传输。
    /// 会考虑输入日期的时区信息。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 将日期转换为时间戳
    /// var date = DateTime.Now;
    /// var timestamp = DateHelper.DateToTimestamp(date);
    ///
    /// // 存储到数据库
    /// entity.CreateTime = DateHelper.DateToTimestamp(DateTime.Now);
    /// </code>
    /// </example>
    public static long DateToTimestamp(DateTime date)
    {
        return new DateTimeOffset(date).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 将日期格式化为指定格式的字符串
    /// </summary>
    /// <param name="date">要格式化的日期</param>
    /// <param name="format">格式字符串，默认为 "yyyy-MM-dd HH:mm:ss"</param>
    /// <returns>格式化后的日期字符串</returns>
    /// <remarks>
    /// 常用格式：
    /// - "yyyy-MM-dd HH:mm:ss"：完整日期时间（默认）
    /// - "yyyy-MM-dd"：仅日期
    /// - "HH:mm:ss"：仅时间
    /// - "yyyy年MM月dd日"：中文格式
    /// </remarks>
    /// <example>
    /// <code>
    /// var date = DateTime.Now;
    ///
    /// // 默认格式
    /// var defaultFormat = DateHelper.FormatDate(date);  // 2024-01-15 10:30:00
    ///
    /// // 仅日期
    /// var dateOnly = DateHelper.FormatDate(date, "yyyy-MM-dd");  // 2024-01-15
    ///
    /// // 仅时间
    /// var timeOnly = DateHelper.FormatDate(date, "HH:mm:ss");  // 10:30:00
    ///
    /// // 中文格式
    /// var chinese = DateHelper.FormatDate(date, "yyyy年MM月dd日");  // 2024年01月15日
    /// </code>
    /// </example>
    public static string FormatDate(DateTime date, string format = "yyyy-MM-dd HH:mm:ss")
    {
        return date.ToString(format);
    }

    /// <summary>
    /// 获取当前日期时间的格式化字符串
    /// </summary>
    /// <param name="format">格式字符串，默认为 "yyyy-MM-dd HH:mm:ss"</param>
    /// <returns>当前日期的格式化字符串</returns>
    /// <remarks>
    /// 快捷方法，直接获取当前时间并格式化。
    /// 使用本地时间（DateTime.Now）。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 获取当前完整时间
    /// var now = DateHelper.GetNow();  // 2024-01-15 10:30:00
    ///
    /// // 获取当前日期
    /// var today = DateHelper.GetNow("yyyy-MM-dd");  // 2024-01-15
    ///
    /// // 用于日志记录
    /// LogHelper.Info($"[{DateHelper.GetNow()}] 用户登录");
    /// </code>
    /// </example>
    public static string GetNow(string format = "yyyy-MM-dd HH:mm:ss")
    {
        return FormatDate(DateTime.Now, format);
    }
}