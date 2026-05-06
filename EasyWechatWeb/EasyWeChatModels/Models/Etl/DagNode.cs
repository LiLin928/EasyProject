namespace EasyWeChatModels.Models.Etl;

/// <summary>
/// DAG 节点模型
/// </summary>
public class DagNode
{
    /// <summary>
    /// 节点ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 节点名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 节点类型：datasource, sql, transform, output, api, file, script, condition, parallel, notification, subflow
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 节点配置（JSON字符串，根据节点类型解析为具体配置类）
    /// </summary>
    public string? Config { get; set; }

    /// <summary>
    /// 节点位置（前端设计器坐标）
    /// </summary>
    public NodePosition? Position { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int? RetryTimes { get; set; }

    /// <summary>
    /// 重试间隔（秒）
    /// </summary>
    public int? RetryInterval { get; set; }

    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    public int? Timeout { get; set; }

    /// <summary>
    /// 失败策略：stop, skip, retry
    /// </summary>
    public string? FailureStrategy { get; set; }

    /// <summary>
    /// 是否跳过失败
    /// </summary>
    public bool? SkipOnFailure { get; set; }
}

/// <summary>
/// 节点位置
/// </summary>
public class NodePosition
{
    /// <summary>X坐标</summary>
    public double X { get; set; }

    /// <summary>Y坐标</summary>
    public double Y { get; set; }
}