namespace InfrastructureManager.Options;

/// <summary>
/// CAP 事件总线配置选项
/// </summary>
public class EasyCapOptions
{
    /// <summary>
    /// 是否启用事件总线（默认禁用）
    /// </summary>
    /// <remarks>
    /// 默认值设为 false，避免配置缺失时意外启用导致错误
    /// </remarks>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// 消费者组 ID
    /// </summary>
    public string GroupId { get; set; } = "EasyWeChatWeb";

    /// <summary>
    /// Kafka 配置
    /// </summary>
    public KafkaConfig Kafka { get; set; } = new();

    /// <summary>
    /// 重试配置
    /// </summary>
    public RetryConfig Retry { get; set; } = new();
}

/// <summary>
/// Kafka 配置
/// </summary>
public class KafkaConfig
{
    /// <summary>
    /// Kafka 服务器地址
    /// </summary>
    public string Servers { get; set; } = "localhost:9092";

    /// <summary>
    /// 主题前缀
    /// </summary>
    public string TopicPrefix { get; set; } = "easywechat.";
}

/// <summary>
/// 重试配置
/// </summary>
public class RetryConfig
{
    /// <summary>
    /// 重试间隔（秒）
    /// </summary>
    public int RetryInterval { get; set; } = 60;

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;
}