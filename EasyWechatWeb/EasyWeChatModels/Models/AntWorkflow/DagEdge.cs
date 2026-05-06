namespace EasyWeChatModels.Models;

/// <summary>
/// DAG 边（连线）
/// </summary>
public class DagEdge
{
    /// <summary>边ID（字符串）</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>源节点ID</summary>
    public string SourceNodeId { get; set; } = string.Empty;

    /// <summary>目标节点ID</summary>
    public string TargetNodeId { get; set; } = string.Empty;

    /// <summary>源端口</summary>
    public string? SourcePort { get; set; }

    /// <summary>目标端口</summary>
    public string? TargetPort { get; set; }

    /// <summary>边条件（条件分支用）</summary>
    public DagEdgeCondition? Condition { get; set; }
}