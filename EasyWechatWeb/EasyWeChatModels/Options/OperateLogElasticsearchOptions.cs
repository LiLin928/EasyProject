namespace EasyWeChatModels.Options;

/// <summary>
/// 操作日志 Elasticsearch 存储配置选项
/// </summary>
public class OperateLogElasticsearchOptions
{
    /// <summary>
    /// 是否启用 ES 存储（默认 false）
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// ES 服务地址
    /// </summary>
    public string Url { get; set; } = "";

    /// <summary>
    /// 索引前缀，配合日期生成完整索引名
    /// 格式：{IndexPrefix}-{yyyy.MM.dd}
    /// </summary>
    public string IndexPrefix { get; set; } = "operate-log";

    /// <summary>
    /// 索引写入超时时间（毫秒）
    /// </summary>
    public int WriteTimeout { get; set; } = 5000;

    /// <summary>
    /// 是否异步写入（不阻塞主流程）
    /// </summary>
    public bool AsyncWrite { get; set; } = true;
}