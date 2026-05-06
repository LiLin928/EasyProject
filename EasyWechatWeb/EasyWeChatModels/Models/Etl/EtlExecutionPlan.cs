namespace EasyWeChatModels.Models.Etl;

/// <summary>
/// ETL 执行计划（由 DAG 解析器生成）
/// </summary>
public class EtlExecutionPlan
{
    /// <summary>
    /// 拓扑排序后的节点执行序列
    /// </summary>
    public List<string> ExecutionSequence { get; set; } = new();

    /// <summary>
    /// 并行执行组（同一组内的节点可并发执行）
    /// </summary>
    public List<List<string>> ParallelGroups { get; set; } = new();

    /// <summary>
    /// 上游链路缓存（NodeId → 所有上游节点ID列表）
    /// 用于变量访问控制：节点只能读取上游节点的输出变量
    /// </summary>
    public Dictionary<string, List<string>> UpstreamCache { get; set; } = new();

    /// <summary>
    /// 条件路由表（条件节点ID → {分支标签 → 目标节点ID})
    /// </summary>
    public Dictionary<string, Dictionary<string, string>> ConditionalRoutes { get; set; } = new();

    /// <summary>
    /// 总节点数
    /// </summary>
    public int TotalNodes => ExecutionSequence.Count;

    /// <summary>
    /// 并行组数量
    /// </summary>
    public int ParallelGroupCount => ParallelGroups.Count;

    /// <summary>
    /// 获取节点的上游节点列表
    /// </summary>
    public List<string> GetUpstreamNodes(string nodeId)
    {
        return UpstreamCache.TryGetValue(nodeId, out var upstream)
            ? upstream
            : new List<string>();
    }

    /// <summary>
    /// 获取节点的并行组索引（-1 表示不在任何并行组）
    /// </summary>
    public int GetParallelGroupIndex(string nodeId)
    {
        for (var i = 0; i < ParallelGroups.Count; i++)
        {
            if (ParallelGroups[i].Contains(nodeId))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 检查节点是否有条件路由
    /// </summary>
    public bool HasConditionalRoutes(string nodeId)
    {
        return ConditionalRoutes.ContainsKey(nodeId);
    }

    /// <summary>
    /// 获取条件节点的路由目标
    /// </summary>
    public string? GetConditionalTarget(string nodeId, string branchLabel)
    {
        if (!ConditionalRoutes.TryGetValue(nodeId, out var routes))
        {
            return null;
        }

        return routes.TryGetValue(branchLabel, out var target) ? target : null;
    }
}