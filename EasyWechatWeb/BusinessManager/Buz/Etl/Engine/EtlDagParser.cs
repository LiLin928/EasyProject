using EasyWeChatModels.Models.Etl;
using Newtonsoft.Json;

namespace BusinessManager.Buz.Etl.Engine;

/// <summary>
/// DAG 解析器 - 使用 Kahn 算法进行拓扑排序
/// </summary>
public class EtlDagParser
{
    /// <summary>
    /// 解析 DAG 配置，生成执行计划
    /// </summary>
    /// <param name="dagConfigJson">DAG 配置 JSON 字符串</param>
    /// <returns>执行计划</returns>
    public EtlExecutionPlan Parse(string dagConfigJson)
    {
        if (string.IsNullOrEmpty(dagConfigJson))
        {
            throw new ArgumentException("DAG 配置不能为空", nameof(dagConfigJson));
        }

        // 解析 JSON
        var dagConfig = JsonConvert.DeserializeObject<DagConfig>(dagConfigJson);
        if (dagConfig == null || dagConfig.Nodes.Count == 0)
        {
            throw new ArgumentException("DAG 配置无效或没有节点", nameof(dagConfigJson));
        }

        return Parse(dagConfig);
    }

    /// <summary>
    /// 解析 DAG 配置对象，生成执行计划
    /// </summary>
    /// <param name="dagConfig">DAG 配置对象</param>
    /// <returns>执行计划</returns>
    public EtlExecutionPlan Parse(DagConfig dagConfig)
    {
        var plan = new EtlExecutionPlan();

        // 1. 构建节点依赖图（NodeId → 前置节点列表）
        var dependencyGraph = BuildDependencyGraph(dagConfig);

        // 2. 计算每个节点的入度
        var inDegrees = CalculateInDegrees(dagConfig.Nodes, dependencyGraph);

        // 3. Kahn 算法进行拓扑排序
        var (sequence, parallelGroups) = KahnTopologicalSort(dagConfig.Nodes, inDegrees, dependencyGraph);

        // 4. 构建上游链路缓存
        var upstreamCache = BuildUpstreamCache(dagConfig, dependencyGraph);

        // 5. 构建条件路由表
        var conditionalRoutes = BuildConditionalRoutes(dagConfig);

        plan.ExecutionSequence = sequence;
        plan.ParallelGroups = parallelGroups;
        plan.UpstreamCache = upstreamCache;
        plan.ConditionalRoutes = conditionalRoutes;

        return plan;
    }

    /// <summary>
    /// 构建节点依赖图（NodeId → 前置节点列表）
    /// </summary>
    private Dictionary<string, List<string>> BuildDependencyGraph(DagConfig dagConfig)
    {
        var graph = new Dictionary<string, List<string>>();

        // 初始化：每个节点都有空的前置节点列表
        foreach (var node in dagConfig.Nodes)
        {
            graph[node.Id] = new List<string>();
        }

        // 根据边构建依赖关系：边的目标节点依赖于源节点
        foreach (var edge in dagConfig.Edges)
        {
            if (graph.ContainsKey(edge.TargetNodeId))
            {
                graph[edge.TargetNodeId].Add(edge.SourceNodeId);
            }
        }

        return graph;
    }

    /// <summary>
    /// 计算每个节点的入度（前置节点数量）
    /// </summary>
    private Dictionary<string, int> CalculateInDegrees(
        List<DagNode> nodes,
        Dictionary<string, List<string>> dependencyGraph)
    {
        var inDegrees = new Dictionary<string, int>();

        foreach (var node in nodes)
        {
            inDegrees[node.Id] = dependencyGraph[node.Id].Count;
        }

        return inDegrees;
    }

    /// <summary>
    /// Kahn 算法拓扑排序
    /// </summary>
    /// <returns>执行序列 + 并行组列表</returns>
    private (List<string> sequence, List<List<string>> parallelGroups) KahnTopologicalSort(
        List<DagNode> nodes,
        Dictionary<string, int> inDegrees,
        Dictionary<string, List<string>> dependencyGraph)
    {
        var sequence = new List<string>();
        var parallelGroups = new List<List<string>>();

        // 复制入度，避免修改原数据
        var currentInDegrees = new Dictionary<string, int>(inDegrees);

        // 找到下游节点映射（NodeId → 下游节点列表）
        var downstreamGraph = BuildDownstreamGraph(nodes, dependencyGraph);

        // 队列：存储当前入度为 0 的节点
        var queue = new List<string>();

        // 初始：所有入度为 0 的节点加入队列
        foreach (var node in nodes)
        {
            if (currentInDegrees[node.Id] == 0)
            {
                queue.Add(node.Id);
            }
        }

        while (queue.Count > 0)
        {
            // 当前所有入度为 0 的节点构成一个并行组
            parallelGroups.Add(new List<string>(queue));

            // 按节点顺序排序（保证确定性）
            var currentBatch = queue.OrderBy(id => nodes.FindIndex(n => n.Id == id)).ToList();

            // 加入执行序列
            sequence.AddRange(currentBatch);

            // 清空队列，准备处理下一批
            queue.Clear();

            // 处理当前批次节点
            foreach (var nodeId in currentBatch)
            {
                // 更新下游节点入度
                if (downstreamGraph.ContainsKey(nodeId))
                {
                    foreach (var downstreamId in downstreamGraph[nodeId])
                    {
                        currentInDegrees[downstreamId]--;

                        // 如果下游节点入度变为 0，加入队列
                        if (currentInDegrees[downstreamId] == 0)
                        {
                            queue.Add(downstreamId);
                        }
                    }
                }
            }
        }

        // 检测是否有环
        if (sequence.Count != nodes.Count)
        {
            var unprocessed = nodes.Where(n => !sequence.Contains(n.Id)).Select(n => n.Id).ToList();
            throw new InvalidOperationException($"DAG 存在循环依赖，无法拓扑排序。未处理节点: {string.Join(", ", unprocessed)}");
        }

        return (sequence, parallelGroups);
    }

    /// <summary>
    /// 构建下游节点映射（NodeId → 下游节点列表）
    /// </summary>
    private Dictionary<string, List<string>> BuildDownstreamGraph(
        List<DagNode> nodes,
        Dictionary<string, List<string>> dependencyGraph)
    {
        var downstreamGraph = new Dictionary<string, List<string>>();

        // 初始化
        foreach (var node in nodes)
        {
            downstreamGraph[node.Id] = new List<string>();
        }

        // 反转依赖图
        foreach (var (nodeId, dependencies) in dependencyGraph)
        {
            foreach (var depId in dependencies)
            {
                if (downstreamGraph.ContainsKey(depId))
                {
                    downstreamGraph[depId].Add(nodeId);
                }
            }
        }

        return downstreamGraph;
    }

    /// <summary>
    /// 构建上游链路缓存（NodeId → 所有上游节点ID列表）
    /// </summary>
    private Dictionary<string, List<string>> BuildUpstreamCache(
        DagConfig dagConfig,
        Dictionary<string, List<string>> dependencyGraph)
    {
        var upstreamCache = new Dictionary<string, List<string>>();

        foreach (var node in dagConfig.Nodes)
        {
            // 递归获取所有上游节点
            var allUpstream = GetAllUpstreamNodes(node.Id, dependencyGraph, new HashSet<string>());
            upstreamCache[node.Id] = allUpstream.ToList();
        }

        return upstreamCache;
    }

    /// <summary>
    /// 递归获取所有上游节点（传递闭包）
    /// </summary>
    private HashSet<string> GetAllUpstreamNodes(
        string nodeId,
        Dictionary<string, List<string>> dependencyGraph,
        HashSet<string> visited)
    {
        var result = new HashSet<string>();

        if (!dependencyGraph.ContainsKey(nodeId))
        {
            return result;
        }

        foreach (var upstreamId in dependencyGraph[nodeId])
        {
            if (!visited.Contains(upstreamId))
            {
                visited.Add(upstreamId);
                result.Add(upstreamId);

                // 递归获取上游节点的上游
                var furtherUpstream = GetAllUpstreamNodes(upstreamId, dependencyGraph, visited);
                foreach (var id in furtherUpstream)
                {
                    result.Add(id);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 构建条件路由表（条件节点ID → {分支条件 → 目标节点ID})
    /// </summary>
    private Dictionary<string, Dictionary<string, string>> BuildConditionalRoutes(DagConfig dagConfig)
    {
        var conditionalRoutes = new Dictionary<string, Dictionary<string, string>>();

        // 找出所有条件节点
        var conditionNodes = dagConfig.Nodes.Where(n => n.Type == "condition").ToList();

        foreach (var conditionNode in conditionNodes)
        {
            var routes = new Dictionary<string, string>();

            // 找出从该条件节点出发的所有边
            var outgoingEdges = dagConfig.Edges
                .Where(e => e.SourceNodeId == conditionNode.Id)
                .ToList();

            foreach (var edge in outgoingEdges)
            {
                // 条件标签作为路由键
                var routeKey = edge.Condition?.Label ?? "default";
                routes[routeKey] = edge.TargetNodeId;
            }

            conditionalRoutes[conditionNode.Id] = routes;
        }

        return conditionalRoutes;
    }
}