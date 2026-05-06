namespace EasyWeChatModels.Models.Etl;

/// <summary>
/// DAG 边模型（节点连接）
/// </summary>
public class DagEdge
{
    /// <summary>
    /// 边ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 源节点ID
    /// </summary>
    public string SourceNodeId { get; set; } = string.Empty;

    /// <summary>
    /// 目标节点ID
    /// </summary>
    public string TargetNodeId { get; set; } = string.Empty;

    /// <summary>
    /// 源端口ID
    /// </summary>
    public string? SourcePort { get; set; }

    /// <summary>
    /// 目标端口ID
    /// </summary>
    public string? TargetPort { get; set; }

    /// <summary>
    /// 边条件（用于条件分支）
    /// </summary>
    public EdgeCondition? Condition { get; set; }
}

/// <summary>
/// 边条件
/// </summary>
public class EdgeCondition
{
    /// <summary>条件表达式</summary>
    public string? Expression { get; set; }

    /// <summary>分支标签</summary>
    public string? Label { get; set; }
}