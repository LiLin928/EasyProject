using BusinessManager.Buz.Etl.Log;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models.Etl;
using SqlSugar;

namespace BusinessManager.Buz.Etl.Engine;

/// <summary>
/// ETL 执行上下文（变量池 + 链路追踪 + 状态管理）
/// </summary>
public class EtlExecutionContext
{
    /// <summary>
    /// 执行记录ID
    /// </summary>
    public Guid ExecutionId { get; set; }

    /// <summary>
    /// 任务流ID
    /// </summary>
    public Guid PipelineId { get; set; }

    /// <summary>
    /// 任务流DAG配置
    /// </summary>
    public DagConfig DagConfig { get; set; } = null!;

    /// <summary>
    /// 执行计划（由 DAG 解析器生成）
    /// </summary>
    public EtlExecutionPlan ExecutionPlan { get; set; } = null!;

    /// <summary>
    /// 全局变量池（NodeId → {VarName → Value}）
    /// 每个节点的输出变量隔离存储
    /// </summary>
    public Dictionary<string, Dictionary<string, object>> VariablePool { get; } = new();

    /// <summary>
    /// 执行参数（用户传入的参数）
    /// </summary>
    public Dictionary<string, object> ExecuteParams { get; set; } = new();

    /// <summary>
    /// 节点执行状态（NodeId → 执行结果）
    /// </summary>
    public Dictionary<string, EtlNodeResult> NodeResults { get; } = new();

    /// <summary>
    /// 节点执行记录实体（NodeId → EtlNodeExecution）
    /// 用于持久化到数据库
    /// </summary>
    public Dictionary<string, EtlNodeExecution> NodeExecutions { get; } = new();

    /// <summary>
    /// 数据库连接（用于节点执行器访问数据库）
    /// </summary>
    public ISqlSugarClient Db { get; set; } = null!;

    /// <summary>
    /// 日志服务（可选）
    /// </summary>
    public IEtlExecutionLogService? Logger { get; set; }

    /// <summary>
    /// 执行开始时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 当前执行状态
    /// </summary>
    public string Status { get; set; } = "running";

    /// <summary>
    /// 已完成节点数
    /// </summary>
    public int CompletedNodes { get; set; } = 0;

    /// <summary>
    /// 总节点数
    /// </summary>
    public int TotalNodes { get; set; } = 0;

    /// <summary>
    /// 是否取消执行
    /// </summary>
    public bool IsCancelled { get; set; } = false;

    /// <summary>
    /// 取消原因
    /// </summary>
    public string? CancelReason { get; set; }

    // ========== 变量访问方法 ==========

    /// <summary>
    /// 获取上游链路中所有节点的输出变量
    /// 只能访问上游节点的变量，实现变量作用域隔离
    /// </summary>
    /// <param name="nodeId">当前节点ID</param>
    /// <returns>合并后的上游变量字典</returns>
    public Dictionary<string, object> GetUpstreamVariables(string nodeId)
    {
        var result = new Dictionary<string, object>();

        // 添加执行参数（全局可用）
        foreach (var (key, value) in ExecuteParams)
        {
            result[key] = value;
        }

        // 获取上游节点列表
        var upstreamNodes = ExecutionPlan.GetUpstreamNodes(nodeId);

        // 合并上游节点的输出变量
        foreach (var upstreamId in upstreamNodes)
        {
            if (VariablePool.TryGetValue(upstreamId, out var nodeVariables))
            {
                foreach (var (key, value) in nodeVariables)
                {
                    // 如果变量名重复，后执行的节点覆盖前面的
                    result[key] = value;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 获取特定上游节点的输出变量
    /// </summary>
    /// <param name="nodeId">当前节点ID</param>
    /// <param name="upstreamNodeId">上游节点ID</param>
    /// <returns>上游节点的输出变量，如果不存在返回空字典</returns>
    public Dictionary<string, object> GetNodeOutputVariables(string nodeId, string upstreamNodeId)
    {
        // 检查是否为上游节点
        var upstreamNodes = ExecutionPlan.GetUpstreamNodes(nodeId);
        if (!upstreamNodes.Contains(upstreamNodeId))
        {
            throw new InvalidOperationException($"节点 {upstreamNodeId} 不是节点 {nodeId} 的上游节点，无法访问其变量");
        }

        if (VariablePool.TryGetValue(upstreamNodeId, out var variables))
        {
            return variables;
        }

        return new Dictionary<string, object>();
    }

    /// <summary>
    /// 获取特定变量值（从上游节点中查找）
    /// </summary>
    /// <param name="nodeId">当前节点ID</param>
    /// <param name="variableName">变量名</param>
    /// <returns>变量值，如果不存在返回 null</returns>
    public object? GetVariable(string nodeId, string variableName)
    {
        // 先检查执行参数
        if (ExecuteParams.TryGetValue(variableName, out var paramValue))
        {
            return paramValue;
        }

        // 从上游节点中查找
        var upstreamNodes = ExecutionPlan.GetUpstreamNodes(nodeId);
        foreach (var upstreamId in upstreamNodes)
        {
            if (VariablePool.TryGetValue(upstreamId, out var nodeVariables))
            {
                if (nodeVariables.TryGetValue(variableName, out var value))
                {
                    return value;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 设置节点输出变量
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <param name="outputs">输出变量字典</param>
    public void SetOutputVariables(string nodeId, Dictionary<string, object> outputs)
    {
        if (!VariablePool.ContainsKey(nodeId))
        {
            VariablePool[nodeId] = new Dictionary<string, object>();
        }

        foreach (var (key, value) in outputs)
        {
            VariablePool[nodeId][key] = value;
        }
    }

    /// <summary>
    /// 设置单个输出变量
    /// </summary>
    public void SetOutputVariable(string nodeId, string variableName, object value)
    {
        if (!VariablePool.ContainsKey(nodeId))
        {
            VariablePool[nodeId] = new Dictionary<string, object>();
        }

        VariablePool[nodeId][variableName] = value;
    }

    // ========== 状态管理方法 ==========

    /// <summary>
    /// 判断上游节点是否都已完成
    /// </summary>
    /// <param name="nodeId">当前节点ID</param>
    /// <returns>是否全部完成</returns>
    public bool IsUpstreamCompleted(string nodeId)
    {
        var upstreamNodes = ExecutionPlan.GetUpstreamNodes(nodeId);

        foreach (var upstreamId in upstreamNodes)
        {
            if (!NodeResults.TryGetValue(upstreamId, out var result) || !result.Success)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 设置节点执行结果
    /// </summary>
    public void SetNodeResult(string nodeId, EtlNodeResult result)
    {
        NodeResults[nodeId] = result;

        if (result.Success)
        {
            CompletedNodes++;
        }
    }

    /// <summary>
    /// 获取节点执行结果
    /// </summary>
    public EtlNodeResult? GetNodeResult(string nodeId)
    {
        return NodeResults.TryGetValue(nodeId, out var result) ? result : null;
    }

    /// <summary>
    /// 计算执行进度（百分比）
    /// </summary>
    public int CalculateProgress()
    {
        if (TotalNodes == 0) return 0;
        return (int)((double)CompletedNodes / TotalNodes * 100);
    }

    /// <summary>
    /// 获取当前执行的节点（根据执行序列）
    /// </summary>
    public string? GetCurrentNodeId()
    {
        // 找到第一个未完成的节点
        foreach (var nodeId in ExecutionPlan.ExecutionSequence)
        {
            if (!NodeResults.ContainsKey(nodeId))
            {
                return nodeId;
            }
        }

        return null; // 所有节点已完成
    }

    /// <summary>
    /// 获取当前执行的节点名称
    /// </summary>
    public string? GetCurrentNodeName()
    {
        var currentNodeId = GetCurrentNodeId();
        if (currentNodeId == null) return null;

        var node = DagConfig.Nodes.FirstOrDefault(n => n.Id == currentNodeId);
        return node?.Name;
    }

    // ========== 失败处理方法 ==========

    /// <summary>
    /// 获取失败的节点列表
    /// </summary>
    public List<string> GetFailedNodes()
    {
        return NodeResults
            .Where(kv => !kv.Value.Success)
            .Select(kv => kv.Key)
            .ToList();
    }

    /// <summary>
    /// 获取节点的失败策略
    /// </summary>
    public string GetNodeFailureStrategy(string nodeId)
    {
        var node = DagConfig.Nodes.FirstOrDefault(n => n.Id == nodeId);
        if (node == null) return "stop";

        // 节点级别的失败策略
        if (!string.IsNullOrEmpty(node.FailureStrategy))
        {
            return node.FailureStrategy;
        }

        // 全局失败策略
        if (DagConfig.Global?.FailureStrategy != null)
        {
            return DagConfig.Global.FailureStrategy;
        }

        // 默认策略
        return "stop";
    }

    /// <summary>
    /// 获取节点的重试配置
    /// </summary>
    public (int retryTimes, int retryInterval) GetNodeRetryConfig(string nodeId)
    {
        var node = DagConfig.Nodes.FirstOrDefault(n => n.Id == nodeId);
        if (node == null) return (0, 0);

        // 节点级别配置
        if (node.RetryTimes.HasValue && node.RetryTimes.Value > 0)
        {
            return (node.RetryTimes.Value, node.RetryInterval ?? 5);
        }

        // 全局配置
        if (DagConfig.Global != null && DagConfig.Global.RetryCount.HasValue && DagConfig.Global.RetryCount.Value > 0)
        {
            return (DagConfig.Global.RetryCount.Value, 5);
        }

        return (0, 0);
    }

    // ========== 执行记录管理方法 ==========

    /// <summary>
    /// 创建节点执行记录实体
    /// </summary>
    public EtlNodeExecution CreateNodeExecution(string nodeId, DagNode node)
    {
        var execution = new EtlNodeExecution
        {
            Id = Guid.NewGuid(),
            ExecutionId = ExecutionId,
            NodeId = nodeId,
            NodeName = node.Name,
            NodeType = node.Type,
            NodeConfig = node.Config,
            Status = "running",
            StartTime = DateTime.Now,
            RetryCount = 0
        };

        NodeExecutions[nodeId] = execution;
        return execution;
    }

    /// <summary>
    /// 更新节点执行记录（成功）
    /// </summary>
    public void UpdateNodeExecutionSuccess(string nodeId, EtlNodeResult result)
    {
        if (!NodeExecutions.TryGetValue(nodeId, out var execution))
        {
            return;
        }

        execution.Status = "success";
        execution.EndTime = DateTime.Now;
        execution.Duration = result.Duration ?? (long)(DateTime.Now - (execution.StartTime ?? DateTime.Now)).TotalMilliseconds;
        execution.OutputData = result.Outputs != null
            ? Newtonsoft.Json.JsonConvert.SerializeObject(result.Outputs)
            : null;
        execution.ProcessedRows = result.ProcessedRows;
        execution.RetryCount = result.RetryCount;
    }

    /// <summary>
    /// 更新节点执行记录（失败）
    /// </summary>
    public void UpdateNodeExecutionFailure(string nodeId, EtlNodeResult result)
    {
        if (!NodeExecutions.TryGetValue(nodeId, out var execution))
        {
            return;
        }

        execution.Status = "failure";
        execution.EndTime = DateTime.Now;
        execution.Duration = result.Duration ?? (long)(DateTime.Now - (execution.StartTime ?? DateTime.Now)).TotalMilliseconds;
        execution.ErrorMessage = result.ErrorMessage;
        execution.RetryCount = result.RetryCount;
    }

    /// <summary>
    /// 更新节点执行记录（重试）
    /// </summary>
    public void UpdateNodeExecutionRetry(string nodeId, int retryCount)
    {
        if (!NodeExecutions.TryGetValue(nodeId, out var execution))
        {
            return;
        }

        execution.RetryCount = retryCount;
        execution.Status = "retrying";
    }

    // ========== 取消执行方法 ==========

    /// <summary>
    /// 取消执行
    /// </summary>
    public void Cancel(string reason)
    {
        IsCancelled = true;
        CancelReason = reason;
        Status = "cancelled";
    }

    /// <summary>
    /// 检查是否应该继续执行
    /// </summary>
    public bool ShouldContinue()
    {
        return !IsCancelled && Status == "running";
    }

    // ========== 日志记录方法 ==========

    /// <summary>
    /// 记录节点日志
    /// </summary>
    public async Task LogAsync(string? nodeId, string? nodeName, string level,
        string message, string? detailData = null, string? step = null)
    {
        if (Logger != null)
        {
            await Logger.LogAsync(ExecutionId, nodeId, nodeName, level, message, detailData, step);
        }
    }

    /// <summary>
    /// 记录信息日志
    /// </summary>
    public async Task LogInfoAsync(string? nodeId, string? nodeName, string message,
        string? detailData = null, string? step = null)
    {
        if (Logger != null)
        {
            await Logger.LogInfoAsync(ExecutionId, nodeId, nodeName, message, detailData, step);
        }
    }

    /// <summary>
    /// 记录错误日志
    /// </summary>
    public async Task LogErrorAsync(string? nodeId, string? nodeName, string message,
        string? detailData = null, string? step = null)
    {
        if (Logger != null)
        {
            await Logger.LogErrorAsync(ExecutionId, nodeId, nodeName, message, detailData, step);
        }
    }

    // ========== 静态工厂方法 ==========

    /// <summary>
    /// 创建执行上下文（从执行记录初始化）
    /// </summary>
    public static EtlExecutionContext Create(
        EtlExecution execution,
        DagConfig dagConfig,
        EtlExecutionPlan plan,
        ISqlSugarClient db)
    {
        var context = new EtlExecutionContext
        {
            ExecutionId = execution.Id,
            PipelineId = execution.PipelineId,
            DagConfig = dagConfig,
            ExecutionPlan = plan,
            Db = db,
            StartTime = execution.StartTime ?? DateTime.Now,
            Status = execution.Status ?? "running",
            TotalNodes = plan.TotalNodes
        };

        // 解析执行参数
        if (!string.IsNullOrEmpty(execution.ExecuteParams))
        {
            try
            {
                context.ExecuteParams = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(execution.ExecuteParams)
                    ?? new Dictionary<string, object>();
            }
            catch
            {
                context.ExecuteParams = new Dictionary<string, object>();
            }
        }

        return context;
    }

    /// <summary>
    /// 创建空执行上下文（用于测试）
    /// </summary>
    public static EtlExecutionContext CreateEmpty()
    {
        return new EtlExecutionContext
        {
            ExecutionId = Guid.NewGuid(),
            PipelineId = Guid.NewGuid(),
            DagConfig = new DagConfig { Nodes = new List<DagNode>(), Edges = new List<DagEdge>() },
            ExecutionPlan = new EtlExecutionPlan(),
            StartTime = DateTime.Now,
            TotalNodes = 0
        };
    }
}