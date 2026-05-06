using BusinessManager.Buz.Etl.Executor;
using BusinessManager.Buz.Etl.Log;
using EasyWeChatModels.Dto.Etl;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models.Etl;
using Mapster;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SqlSugar;

namespace BusinessManager.Buz.Etl.Engine;

/// <summary>
/// ETL 执行引擎实现
/// 核心调度器：解析 DAG → 创建上下文 → 调度节点 → 处理失败 → 更新状态
/// </summary>
public class EtlExecutionEngine : IEtlExecutionEngine
{
    /// <summary>
    /// 数据库连接（属性注入）
    /// </summary>
    public ISqlSugarClient _db { get; set; } = null!;

    /// <summary>
    /// 日志（属性注入）
    /// </summary>
    public ILogger<EtlExecutionEngine> _logger { get; set; } = null!;

    /// <summary>
    /// ETL 执行日志服务（属性注入）
    /// </summary>
    public IEtlExecutionLogService? _etlLogService { get; set; }

    /// <summary>
    /// DAG 解析器
    /// </summary>
    private readonly EtlDagParser _dagParser = new();

    /// <summary>
    /// 执行器工厂
    /// </summary>
    private readonly EtlExecutorFactory _executorFactory;

    /// <summary>
    /// 构造函数（初始化执行器工厂）
    /// </summary>
    public EtlExecutionEngine()
    {
        _executorFactory = EtlExecutorFactory.CreateDefault();
    }

    /// <summary>
    /// 同步执行
    /// </summary>
    public async Task<EtlExecutionResultDto> ExecuteSyncAsync(Guid executionId)
    {
        // 1. 加载执行记录
        var execution = await LoadExecutionAsync(executionId);
        if (execution == null)
        {
            return CreateErrorResult("执行记录不存在", executionId);
        }

        // 2. 检查状态
        if (execution.Status != "pending")
        {
            return CreateErrorResult($"执行状态不正确: {execution.Status}", executionId);
        }

        // 3. 执行流程
        return await ExecuteInternalAsync(execution);
    }

    /// <summary>
    /// 异步执行（后台任务）
    /// </summary>
    public async Task<Guid> ExecuteAsyncAsync(Guid executionId)
    {
        // 提交到后台执行（使用 Task.Run）
        _ = Task.Run(async () =>
        {
            try
            {
                _logger.LogInformation("开始异步执行，执行ID: {ExecutionId}", executionId);
                var result = await ExecuteSyncAsync(executionId);
                _logger.LogInformation("异步执行完成，执行ID: {ExecutionId}, 状态: {Status}", executionId, result.Status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "异步执行失败，执行ID: {ExecutionId}", executionId);
            }
        });

        return executionId;
    }

    /// <summary>
    /// 取消执行
    /// </summary>
    public async Task<int> CancelAsync(Guid executionId)
    {
        var execution = await _db.Queryable<EtlExecution>()
            .Where(x => x.Id == executionId)
            .FirstAsync();

        if (execution == null || execution.Status != "running")
        {
            return 0;
        }

        // 更新状态
        execution.Status = "cancelled";
        execution.EndTime = DateTime.Now;
        execution.ErrorMessage = "用户手动取消";

        return await _db.Updateable(execution).ExecuteCommandAsync();
    }

    /// <summary>
    /// 重试失败节点
    /// </summary>
    public async Task<int> RetryNodeAsync(Guid executionId, string nodeId)
    {
        // 1. 加载节点执行记录
        var nodeExecution = await _db.Queryable<EtlNodeExecution>()
            .Where(x => x.ExecutionId == executionId && x.NodeId == nodeId)
            .FirstAsync();

        if (nodeExecution == null || nodeExecution.Status != "failure")
        {
            return 0;
        }

        // 2. 重置状态
        nodeExecution.Status = "pending";
        nodeExecution.ErrorMessage = null;
        nodeExecution.RetryCount++;

        return await _db.Updateable(nodeExecution).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取执行进度
    /// </summary>
    public async Task<EtlExecutionProgressDto> GetProgressAsync(Guid executionId)
    {
        var execution = await _db.Queryable<EtlExecution>()
            .Where(x => x.Id == executionId)
            .FirstAsync();

        if (execution == null)
        {
            return new EtlExecutionProgressDto { ExecutionId = executionId, Status = "not_found" };
        }

        // 计算进度
        var nodeExecutions = await _db.Queryable<EtlNodeExecution>()
            .Where(x => x.ExecutionId == executionId)
            .ToListAsync();

        var completedNodes = nodeExecutions.Count(x => x.Status == "success" || x.Status == "failure");
        var totalNodes = nodeExecutions.Count;

        var currentNode = nodeExecutions.FirstOrDefault(x => x.Status == "running");

        // 计算执行时长
        var elapsedTime = execution.StartTime.HasValue
            ? (long)(DateTime.Now - execution.StartTime.Value).TotalMilliseconds
            : 0;

        // 预估剩余时间（基于已完成节点的平均耗时）
        var avgNodeTime = completedNodes > 0
            ? nodeExecutions.Where(x => x.Status == "success").Sum(x => x.Duration ?? 0) / completedNodes
            : 0;
        var remainingNodes = totalNodes - completedNodes;
        var estimatedRemainingTime = avgNodeTime * remainingNodes;

        return new EtlExecutionProgressDto
        {
            ExecutionId = executionId,
            Status = execution.Status ?? "unknown",
            Progress = totalNodes > 0 ? (int)((double)completedNodes / totalNodes * 100) : 0,
            CompletedNodes = completedNodes,
            TotalNodes = totalNodes,
            CurrentNodeId = currentNode?.NodeId,
            CurrentNodeName = currentNode?.NodeName,
            StartTime = execution.StartTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            ElapsedTime = elapsedTime,
            EstimatedRemainingTime = estimatedRemainingTime
        };
    }

    /// <summary>
    /// 获取执行结果
    /// </summary>
    public async Task<EtlExecutionResultDto> GetResultAsync(Guid executionId)
    {
        var execution = await _db.Queryable<EtlExecution>()
            .Where(x => x.Id == executionId)
            .FirstAsync();

        if (execution == null)
        {
            return new EtlExecutionResultDto { ExecutionId = executionId, Status = "not_found" };
        }

        // 获取节点执行结果
        var nodeExecutions = await _db.Queryable<EtlNodeExecution>()
            .Where(x => x.ExecutionId == executionId)
            .ToListAsync();

        var nodeResults = nodeExecutions.Adapt<List<EtlNodeResultDto>>();

        return new EtlExecutionResultDto
        {
            ExecutionId = executionId,
            Status = execution.Status ?? "unknown",
            Progress = nodeExecutions.Count > 0
                ? (int)((double)nodeExecutions.Count(x => x.Status == "success") / nodeExecutions.Count * 100)
                : 0,
            CompletedNodes = nodeExecutions.Count(x => x.Status == "success" || x.Status == "failure"),
            TotalNodes = nodeExecutions.Count,
            Duration = execution.Duration ?? (execution.StartTime.HasValue && execution.EndTime.HasValue
                ? (long)(execution.EndTime.Value - execution.StartTime.Value).TotalMilliseconds
                : 0),
            ErrorMessage = execution.ErrorMessage,
            NodeResults = nodeResults,
            StartTime = execution.StartTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            EndTime = execution.EndTime?.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    // ========== 内部执行流程 ==========

    /// <summary>
    /// 内部执行流程
    /// </summary>
    private async Task<EtlExecutionResultDto> ExecuteInternalAsync(EtlExecution execution)
    {
        var startTime = DateTime.Now;

        try
        {
            _logger.LogInformation("开始执行任务流，执行ID: {ExecutionId}, 任务流ID: {PipelineId}", execution.Id, execution.PipelineId);

            // 1. 更新执行状态为 running
            await UpdateExecutionStatusAsync(execution.Id, "running", startTime);

            // 2. 加载任务流获取 DagConfig
            var pipeline = await _db.Queryable<EtlPipeline>()
                .Where(x => x.Id == execution.PipelineId)
                .FirstAsync();

            if (pipeline == null || string.IsNullOrEmpty(pipeline.DagConfig))
            {
                _logger.LogWarning("任务流不存在或DAG配置为空，执行ID: {ExecutionId}", execution.Id);
                return await FailExecutionAsync(execution.Id, "任务流不存在或DAG配置为空");
            }

            _logger.LogInformation("DAG配置长度: {Length}", pipeline.DagConfig?.Length ?? 0);

            // 3. 解析 DAG 配置
            var dagConfig = ParseDagConfig(pipeline.DagConfig);
            if (dagConfig.Nodes.Count == 0)
            {
                _logger.LogWarning("DAG配置无效或没有节点，执行ID: {ExecutionId}", execution.Id);
                return await FailExecutionAsync(execution.Id, "DAG 配置无效或没有节点");
            }

            _logger.LogInformation("DAG节点数量: {NodeCount}, 边数量: {EdgeCount}", dagConfig.Nodes.Count, dagConfig.Edges.Count);

            // 3. 解析 DAG（拓扑排序）
            var executionPlan = _dagParser.Parse(dagConfig);
            _logger.LogInformation("执行计划解析完成，总节点: {TotalNodes}, 并行组数量: {ParallelGroupCount}", executionPlan.TotalNodes, executionPlan.ParallelGroups.Count);

            // 4. 创建执行上下文
            var context = EtlExecutionContext.Create(execution, dagConfig, executionPlan, _db);
            context.Logger = _etlLogService;  // 注入日志服务
            context.StartTime = startTime;
            context.TotalNodes = executionPlan.TotalNodes;

            // 5. 初始化节点执行记录
            await InitNodeExecutionsAsync(execution.Id, dagConfig.Nodes);

            // 6. 按并行组执行节点
            foreach (var parallelGroup in executionPlan.ParallelGroups)
            {
                if (context.IsCancelled)
                {
                    _logger.LogWarning("执行被取消，执行ID: {ExecutionId}", execution.Id);
                    break;
                }

                _logger.LogInformation("执行并行组，节点数量: {NodeCount}, 节点ID: {NodeIds}", parallelGroup.Count, string.Join(",", parallelGroup));

                // 并行执行组内的节点
                var tasks = parallelGroup.Select(nodeId => ExecuteNodeAsync(context, dagConfig, nodeId));
                var results = await Task.WhenAll(tasks);

                // 处理失败策略
                var failedResults = results.Where(r => !r.Success).ToList();
                if (failedResults.Count > 0)
                {
                    // 记录每个失败节点的详细错误信息
                    foreach (var failed in failedResults)
                    {
                        var failedNodeId = context.NodeResults
                            .Where(kv => kv.Value == failed)
                            .Select(kv => kv.Key)
                            .FirstOrDefault();
                        var failedNode = dagConfig.Nodes.FirstOrDefault(n => n.Id == failedNodeId);
                        _logger.LogWarning("节点执行失败，节点ID: {NodeId}, 节点名称: {NodeName}, 错误信息: {ErrorMessage}, 配置: {Config}",
                            failedNodeId, failedNode?.Name, failed.ErrorMessage, failedNode?.Config);
                    }

                    _logger.LogWarning("并行组有失败节点，数量: {FailedCount}", failedResults.Count);
                    var shouldStop = await HandleFailuresAsync(context, dagConfig, failedResults);
                    if (shouldStop)
                    {
                        break;
                    }
                }
            }

            // 7. 保存节点执行结果
            await SaveNodeExecutionsAsync(context);

            // 8. 更新执行状态
            var finalStatus = context.IsCancelled ? "cancelled"
                : context.GetFailedNodes().Count > 0 ? "failure"
                : "success";

            _logger.LogInformation("执行完成，最终状态: {FinalStatus}, 执行ID: {ExecutionId}", finalStatus, execution.Id);

            return await CompleteExecutionAsync(execution.Id, context, finalStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行异常，执行ID: {ExecutionId}", execution.Id);
            return await FailExecutionAsync(execution.Id, $"执行异常: {ex.Message}");
        }
    }

    /// <summary>
    /// 加载执行记录
    /// </summary>
    private async Task<EtlExecution?> LoadExecutionAsync(Guid executionId)
    {
        return await _db.Queryable<EtlExecution>()
            .Where(x => x.Id == executionId)
            .FirstAsync();
    }

    /// <summary>
    /// 解析 DAG 配置
    /// </summary>
    private DagConfig ParseDagConfig(string? dagConfigJson)
    {
        if (string.IsNullOrEmpty(dagConfigJson))
        {
            return new DagConfig { Nodes = new List<DagNode>(), Edges = new List<DagEdge>() };
        }

        try
        {
            // 先解析为 JObject，处理 Config 字段的对象转字符串
            var jObject = Newtonsoft.Json.Linq.JObject.Parse(dagConfigJson);

            // 处理 nodes 中的 config 字段（前端发送的是 JSON 对象，需要转为字符串）
            var nodesArray = jObject["nodes"] as Newtonsoft.Json.Linq.JArray;
            if (nodesArray != null)
            {
                foreach (var node in nodesArray)
                {
                    var configToken = node["config"];
                    if (configToken != null && configToken.Type == Newtonsoft.Json.Linq.JTokenType.Object)
                    {
                        // 将 JObject 转为字符串
                        node["config"] = configToken.ToString(Newtonsoft.Json.Formatting.None);
                    }
                }
            }

            // 使用 camelCase 反序列化设置
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };

            return JsonConvert.DeserializeObject<DagConfig>(jObject.ToString(), settings)
                ?? new DagConfig { Nodes = new List<DagNode>(), Edges = new List<DagEdge>() };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DAG配置解析失败，JSON长度: {Length}", dagConfigJson.Length);
            return new DagConfig { Nodes = new List<DagNode>(), Edges = new List<DagEdge>() };
        }
    }

    /// <summary>
    /// 更新执行状态
    /// </summary>
    private async Task UpdateExecutionStatusAsync(Guid executionId, string status, DateTime startTime)
    {
        await _db.Updateable<EtlExecution>()
            .SetColumns(x => x.Status == status)
            .SetColumns(x => x.StartTime == startTime)
            .Where(x => x.Id == executionId)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 初始化节点执行记录
    /// </summary>
    private async Task InitNodeExecutionsAsync(Guid executionId, List<DagNode> nodes)
    {
        var nodeExecutions = nodes.Select(node => new EtlNodeExecution
        {
            Id = Guid.NewGuid(),
            ExecutionId = executionId,
            NodeId = node.Id,
            NodeName = node.Name,
            NodeType = node.Type,
            NodeConfig = node.Config,
            Status = "pending",
            StartTime = DateTime.Now
        }).ToList();

        await _db.Insertable(nodeExecutions).ExecuteCommandAsync();
    }

    /// <summary>
    /// 执行单个节点
    /// </summary>
    private async Task<EtlNodeResult> ExecuteNodeAsync(
        EtlExecutionContext context,
        DagConfig dagConfig,
        string nodeId)
    {
        var node = dagConfig.Nodes.FirstOrDefault(n => n.Id == nodeId);
        if (node == null)
        {
            return EtlNodeResult.FailResult($"节点不存在: {nodeId}");
        }

        // 记录节点配置信息
        _logger.LogInformation("准备执行节点，ID: {NodeId}, 名称: {NodeName}, 类型: {NodeType}, 配置: {Config}",
            node.Id, node.Name, node.Type, node.Config ?? "(空)");

        // 写入执行日志
        if (context.Logger != null)
        {
            await context.Logger.LogInfoAsync(context.ExecutionId, node.Id, node.Name,
                $"开始执行节点: {node.Name ?? node.Type}");
        }

        // 更新节点状态为 running
        await UpdateNodeExecutionStatusAsync(context.ExecutionId, nodeId, "running");

        // 创建节点执行记录实体
        context.CreateNodeExecution(nodeId, node);

        try
        {
            // 获取执行器
            var executor = _executorFactory.GetExecutor(node.Type);

            // 检查上游状态
            var (canExecute, reason) = await executor.CheckUpstreamAsync(context, node);
            if (!canExecute)
            {
                context.UpdateNodeExecutionFailure(nodeId, EtlNodeResult.FailResult(reason ?? "上游检查失败"));

                // 写入失败日志
                if (context.Logger != null)
                {
                    await context.Logger.LogErrorAsync(context.ExecutionId, node.Id, node.Name,
                        $"上游检查失败: {reason}");
                }

                return EtlNodeResult.FailResult(reason ?? "上游检查失败");
            }

            // 执行节点
            var result = await executor.ExecuteAsync(context, node);

            // 更新执行结果
            if (result.Success)
            {
                context.UpdateNodeExecutionSuccess(nodeId, result);

                // 写入成功日志
                if (context.Logger != null)
                {
                    var processedRows = result.ProcessedRows ?? 0;
                    await context.Logger.LogInfoAsync(context.ExecutionId, node.Id, node.Name,
                        $"节点执行成功，处理行数: {processedRows}");
                }

                // 设置输出变量
                if (result.Outputs != null)
                {
                    context.SetOutputVariables(nodeId, result.Outputs);
                }

                context.SetNodeResult(nodeId, result);
            }
            else
            {
                context.UpdateNodeExecutionFailure(nodeId, result);
                context.SetNodeResult(nodeId, result);

                // 写入失败日志
                if (context.Logger != null)
                {
                    await context.Logger.LogErrorAsync(context.ExecutionId, node.Id, node.Name,
                        $"节点执行失败: {result.ErrorMessage}");
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            var result = EtlNodeResult.FailResult($"执行异常: {ex.Message}");
            context.UpdateNodeExecutionFailure(nodeId, result);
            context.SetNodeResult(nodeId, result);

            // 写入异常日志
            if (context.Logger != null)
            {
                await context.Logger.LogErrorAsync(context.ExecutionId, node.Id, node.Name,
                    $"执行异常: {ex.Message}");
            }

            return result;
        }
    }

    /// <summary>
    /// 更新节点执行状态
    /// </summary>
    private async Task UpdateNodeExecutionStatusAsync(Guid executionId, string nodeId, string status)
    {
        await _db.Updateable<EtlNodeExecution>()
            .SetColumns(x => x.Status == status)
            .SetColumns(x => x.StartTime == DateTime.Now)
            .Where(x => x.ExecutionId == executionId && x.NodeId == nodeId)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 处理失败策略
    /// </summary>
    private async Task<bool> HandleFailuresAsync(
        EtlExecutionContext context,
        DagConfig dagConfig,
        List<EtlNodeResult> failedResults)
    {
        // 检查全局失败策略
        var globalStrategy = dagConfig.Global?.FailureStrategy ?? "stop";

        foreach (var failed in failedResults)
        {
            // 获取失败节点的 ID
            var failedNodeId = context.NodeResults
                .Where(kv => kv.Value == failed)
                .Select(kv => kv.Key)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(failedNodeId)) continue;

            var nodeStrategy = context.GetNodeFailureStrategy(failedNodeId);

            // 处理重试
            var (retryTimes, retryInterval) = context.GetNodeRetryConfig(failedNodeId);
            if (retryTimes > 0 && nodeStrategy == "retry")
            {
                // TODO: 实现重试逻辑
                // 目前直接标记失败
            }

            // 根据策略决定是否继续
            if (nodeStrategy == "stop" || (nodeStrategy != "skip" && globalStrategy == "stop"))
            {
                // 停止执行
                context.Cancel($"节点失败: {failed.ErrorMessage}");
                await Task.CompletedTask;
                return true;
            }
        }

        await Task.CompletedTask;
        return false;
    }

    /// <summary>
    /// 保存节点执行结果
    /// </summary>
    private async Task SaveNodeExecutionsAsync(EtlExecutionContext context)
    {
        foreach (var (nodeId, execution) in context.NodeExecutions)
        {
            await _db.Updateable(execution).ExecuteCommandAsync();
        }
    }

    /// <summary>
    /// 完成执行
    /// </summary>
    private async Task<EtlExecutionResultDto> CompleteExecutionAsync(
        Guid executionId,
        EtlExecutionContext context,
        string status)
    {
        var endTime = DateTime.Now;
        var duration = (long)(endTime - context.StartTime).TotalMilliseconds;

        // 更新执行记录
        await _db.Updateable<EtlExecution>()
            .SetColumns(x => x.Status == status)
            .SetColumns(x => x.EndTime == endTime)
            .SetColumns(x => x.Duration == duration)
            .SetColumns(x => x.ErrorMessage == (status == "failure" ? context.CancelReason : null))
            .Where(x => x.Id == executionId)
            .ExecuteCommandAsync();

        // 返回结果
        return await GetResultAsync(executionId);
    }

    /// <summary>
    /// 标记执行失败
    /// </summary>
    private async Task<EtlExecutionResultDto> FailExecutionAsync(Guid executionId, string errorMessage)
    {
        var endTime = DateTime.Now;

        await _db.Updateable<EtlExecution>()
            .SetColumns(x => x.Status == "failure")
            .SetColumns(x => x.EndTime == endTime)
            .SetColumns(x => x.ErrorMessage == errorMessage)
            .Where(x => x.Id == executionId)
            .ExecuteCommandAsync();

        return await GetResultAsync(executionId);
    }

    /// <summary>
    /// 创建错误结果
    /// </summary>
    private EtlExecutionResultDto CreateErrorResult(string message, Guid executionId)
    {
        return new EtlExecutionResultDto
        {
            ExecutionId = executionId,
            Status = "error",
            ErrorMessage = message,
            Progress = 0,
            CompletedNodes = 0,
            TotalNodes = 0
        };
    }
}