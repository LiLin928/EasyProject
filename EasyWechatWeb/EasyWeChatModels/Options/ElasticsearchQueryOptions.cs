namespace EasyWeChatModels.Options;

/// <summary>
/// Elasticsearch 日志查询配置选项
/// </summary>
public class ElasticsearchQueryOptions
{
    /// <summary>
    /// 环境配置字典，键为环境名称（如 Development、Production）
    /// </summary>
    public Dictionary<string, ElasticsearchEnvironmentConfig> Environments { get; set; } = new();

    /// <summary>
    /// 默认环境名称
    /// </summary>
    public string DefaultEnvironment { get; set; } = "Development";
}

/// <summary>
/// 单个 Elasticsearch 环境配置
/// </summary>
public class ElasticsearchEnvironmentConfig
{
    /// <summary>
    /// Elasticsearch 服务地址，如 http://localhost:9200
    /// </summary>
    public string Url { get; set; } = "";

    /// <summary>
    /// 索引前缀，配合日期生成完整索引名
    /// </summary>
    public string IndexPrefix { get; set; } = "easywechat-logs";

    /// <summary>
    /// 默认查询超时时间（毫秒）
    /// </summary>
    public int DefaultTimeout { get; set; } = 30000;
}