namespace EasyWeChatModels.Models.Etl;

/// <summary>
/// DAG 配置模型（整个任务流的 DAG 定义）
/// </summary>
public class DagConfig
{
    /// <summary>
    /// 配置版本
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// 所有节点列表
    /// </summary>
    public List<DagNode> Nodes { get; set; } = new();

    /// <summary>
    /// 所有边列表
    /// </summary>
    public List<DagEdge> Edges { get; set; } = new();

    /// <summary>
    /// 全局配置
    /// </summary>
    public DagGlobalConfig? Global { get; set; }
}

/// <summary>
/// DAG 全局配置
/// </summary>
public class DagGlobalConfig
{
    /// <summary>全局超时时间（秒）</summary>
    public int? Timeout { get; set; }

    /// <summary>全局失败策略</summary>
    public string? FailureStrategy { get; set; }

    /// <summary>全局重试次数</summary>
    public int? RetryCount { get; set; }

    /// <summary>全局并发数</summary>
    public int? Concurrency { get; set; }
}