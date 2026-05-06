namespace CommonManager.Elasticsearch;

/// <summary>
/// 操作日志 Elasticsearch 文档模型
/// </summary>
/// <remarks>
/// 用于将操作日志索引到 Elasticsearch，便于全文搜索和分析。
/// 索引名称格式：operate-log-{yyyy.MM.dd}
/// </remarks>
public class OperateLogDocument
{
    /// <summary>
    /// 文档ID（对应 MySQL 的主键 GUID）
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 操作用户ID
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 操作用户名
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 操作模块
    /// </summary>
    public string? Module { get; set; }

    /// <summary>
    /// 操作动作
    /// </summary>
    public string? Action { get; set; }

    /// <summary>
    /// 请求方法（GET/POST/PUT/DELETE）
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// 请求地址
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    public string? Ip { get; set; }

    /// <summary>
    /// 操作地点
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// 请求参数（JSON格式）
    /// </summary>
    public string? Params { get; set; }

    /// <summary>
    /// 操作结果（JSON格式）
    /// </summary>
    public string? Result { get; set; }

    /// <summary>
    /// 状态（1=成功 0=失败）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMsg { get; set; }

    /// <summary>
    /// 执行时长（毫秒）
    /// </summary>
    public long? Duration { get; set; }

    /// <summary>
    /// 操作时间（用于索引时间分区和排序）
    /// </summary>
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 时间戳字段（ES 默认时间字段）
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;
}