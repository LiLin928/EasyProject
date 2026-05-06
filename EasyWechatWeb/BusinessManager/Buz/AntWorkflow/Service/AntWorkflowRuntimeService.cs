using SqlSugar;
using Newtonsoft.Json;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Dto.AntWorkflow;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Enums;
using EasyWeChatModels.Models;
using EasyWeChatModels.Models.AntWorkflow;
using BusinessManager.Buz.AntWorkflow.IService;
using BusinessManager.Buz.IService;
using BusinessManager.Factory;
using CommonManager.Base;
using Microsoft.Extensions.Logging;
using AntWorkflowEntity = EasyWeChatModels.Entitys.AntWorkflow;

namespace BusinessManager.Buz.Service;

/// <summary>
/// Ant流程运行时服务实现
/// </summary>
public class AntWorkflowRuntimeService : IAntWorkflowRuntimeService
{
    /// <summary>数据库上下文（Autofac 属性注入）</summary>
    public ISqlSugarClient _db { get; set; } = null!;

    /// <summary>节点处理器工厂（Autofac 属性注入）</summary>
    public NodeHandlerFactory _handlerFactory { get; set; } = null!;

    /// <summary>业务审核点服务（Autofac 属性注入）</summary>
    public IBusinessAuditPointService _businessAuditPointService { get; set; } = null!;

    /// <summary>日志服务（Autofac 属性注入）</summary>
    public ILogger<AntWorkflowRuntimeService> _logger { get; set; } = null!;

    /// <inheritdoc/>
    public async Task<Guid> StartAsync(StartAntWorkflowDto dto, Guid initiatorId, string initiatorName)
    {
        _logger.LogInformation("开始启动流程: WorkflowId={WorkflowId}, InitiatorId={InitiatorId}, InitiatorName={InitiatorName}",
            dto.WorkflowId, initiatorId, initiatorName);

        // 1. 获取流程定义，验证状态
        var workflow = await _db.Queryable<AntWorkflowEntity>()
            .Where(w => w.Id == dto.WorkflowId && w.Status == (int)WorkflowStatus.Published)
            .FirstAsync();

        if (workflow == null)
        {
            _logger.LogError("流程不存在或未发布: WorkflowId={WorkflowId}", dto.WorkflowId);
            throw new Exception("流程不存在或未发布");
        }

        _logger.LogInformation("流程定义获取成功: WorkflowId={WorkflowId}, Name={Name}", workflow.Id, workflow.Name);

        // 2. 解析 DAG 配置
        var dagConfig = JsonConvert.DeserializeObject<DagConfig>(workflow.FlowConfig ?? "");
        if (dagConfig == null || dagConfig.Nodes.Count == 0)
        {
            _logger.LogError("流程配置无效: FlowConfig={FlowConfig}", workflow.FlowConfig);
            throw new Exception("流程配置无效");
        }

        _logger.LogInformation("DAG配置解析成功: NodesCount={NodesCount}, EdgesCount={EdgesCount}",
            dagConfig.Nodes.Count, dagConfig.Edges.Count);

        // 3. 创建流程实例
        var instance = new AntWorkflowInstance
        {
            Id = Guid.NewGuid(),
            WorkflowId = workflow.Id,
            Title = dto.Title ?? $"{workflow.Name}-{DateTime.Now:yyyyMMddHHmmss}",
            BusinessType = dto.BusinessType,
            BusinessId = dto.BusinessId,
            BusinessData = dto.BusinessData,
            FormData = dto.FormData,
            Status = (int)InstanceStatus.Approving,
            FlowConfig = workflow.FlowConfig,
            StartTime = DateTime.Now,
            InitiatorId = initiatorId,
            CreateTime = DateTime.Now
        };

        // 4. 找到开始节点
        var startNode = dagConfig.Nodes.FirstOrDefault(n => n.Type == AntNodeType.Start);
        if (startNode == null)
        {
            _logger.LogError("流程缺少开始节点");
            throw new Exception("流程缺少开始节点");
        }
        instance.StartNodeId = startNode.Id;

        _logger.LogInformation("开始节点找到: NodeId={NodeId}, NodeName={NodeName}", startNode.Id, startNode.Name);

        // 5. 创建流程实例和节点（不使用事务，分开执行）
        // 先创建实例
        await _db.Insertable(instance).ExecuteCommandAsync();
        _logger.LogInformation("流程实例创建成功: InstanceId={InstanceId}", instance.Id);

        // 创建所有节点状态并保存引用
        var createdNodes = new Dictionary<string, AntWorkflowInstanceNode>();
        foreach (var node in dagConfig.Nodes)
        {
            var instanceNode = new AntWorkflowInstanceNode
            {
                Id = Guid.NewGuid(),
                InstanceId = instance.Id,
                NodeId = node.Id,
                NodeName = node.Name,
                NodeType = (int)node.Type,
                NodeConfig = node.Config?.ToString(),
                ApproveStatus = (int)NodeApproveStatus.Pending,
                CreateTime = DateTime.Now
            };
            createdNodes[node.Id] = instanceNode;
            await _db.Insertable(instanceNode).ExecuteCommandAsync();
        }

        _logger.LogInformation("所有节点实例创建成功: NodesCount={NodesCount}", createdNodes.Count);

        // 处理开始节点（自动完成）- 使用已创建的节点对象
        var startInstanceNode = createdNodes.GetValueOrDefault(startNode.Id);
        if (startInstanceNode != null)
        {
            _logger.LogInformation("开始处理开始节点: InstanceNodeId={InstanceNodeId}", startInstanceNode.Id);
            await ProcessStartNodeWithNode(instance, dagConfig, startNode, startInstanceNode, initiatorId, initiatorName, createdNodes);
        }
        else
        {
            _logger.LogWarning("开始节点实例未找到: NodeId={NodeId}", startNode.Id);
        }

        _logger.LogInformation("流程启动完成: InstanceId={InstanceId}", instance.Id);
        return instance.Id;
    }

    private async Task ProcessStartNodeWithNode(AntWorkflowInstance instance, DagConfig dagConfig,
        DagNode startNode, AntWorkflowInstanceNode startInstanceNode, Guid initiatorId, string initiatorName,
        Dictionary<string, AntWorkflowInstanceNode> createdNodes)
    {
        _logger.LogInformation("获取开始节点处理器: NodeType={NodeType}", AntNodeType.Start);

        var handler = _handlerFactory.GetHandler(AntNodeType.Start);
        if (handler == null)
        {
            _logger.LogError("开始节点处理器未找到！请检查 NodeHandlerFactory 是否正确注入");
            return;
        }

        _logger.LogInformation("开始节点处理器获取成功: HandlerType={HandlerType}", handler.GetType().Name);

        var context = new NodeHandlerContext
        {
            Db = _db,
            Instance = instance,
            DagConfig = dagConfig,
            DagNode = startNode,
            InstanceNode = startInstanceNode,
            BusinessData = instance.BusinessData,
            FormData = instance.FormData,
            OperatorId = initiatorId,
            OperatorName = initiatorName
        };

        _logger.LogInformation("执行 HandleEnterAsync");
        await handler.HandleEnterAsync(context);
        _logger.LogInformation("HandleEnterAsync 完成，节点状态应为 Completed");

        _logger.LogInformation("执行 HandleCompleteAsync");
        await handler.HandleCompleteAsync(context);
        _logger.LogInformation("HandleCompleteAsync 完成，审批记录已创建");

        // 推进到下一节点 - 使用已创建的节点对象
        var nextNodeIds = await handler.GetNextNodesAsync(context);
        _logger.LogInformation("获取下一节点列表: NextNodeIds={NextNodeIds}", string.Join(",", nextNodeIds));

        foreach (var nextNodeId in nextNodeIds)
        {
            var nextInstanceNode = createdNodes.GetValueOrDefault(nextNodeId);
            if (nextInstanceNode != null)
            {
                _logger.LogInformation("推进到下一节点: NodeId={NodeId}, NodeName={NodeName}, NodeType={NodeType}",
                    nextNodeId, nextInstanceNode.NodeName, nextInstanceNode.NodeType);
                await EnterNextNodeWithNode(instance, dagConfig, nextNodeId, nextInstanceNode);
            }
            else
            {
                _logger.LogWarning("下一节点实例未找到: NodeId={NodeId}", nextNodeId);
            }
        }
    }

    private async Task EnterNextNodeWithNode(AntWorkflowInstance instance, DagConfig dagConfig, string nodeId, AntWorkflowInstanceNode instanceNode)
    {
        var node = dagConfig.Nodes.FirstOrDefault(n => n.Id == nodeId);
        if (node == null)
        {
            _logger.LogWarning("节点配置未找到: NodeId={NodeId}", nodeId);
            return;
        }

        _logger.LogInformation("进入节点处理: NodeId={NodeId}, NodeType={NodeType}, NodeName={NodeName}",
            nodeId, node.Type, node.Name);

        // 更新节点状态为处理中
        instanceNode.ApproveStatus = (int)NodeApproveStatus.Processing;
        await _db.Updateable(instanceNode).ExecuteCommandAsync();
        _logger.LogInformation("节点状态更新为 Processing: InstanceNodeId={InstanceNodeId}", instanceNode.Id);

        // 根据节点类型处理
        var handler = _handlerFactory.GetHandler(node.Type);
        if (handler == null)
        {
            _logger.LogWarning("节点处理器未找到: NodeType={NodeType}", node.Type);
            return;
        }

        _logger.LogInformation("节点处理器获取成功: HandlerType={HandlerType}", handler.GetType().Name);

        var context = new NodeHandlerContext
        {
            Db = _db,
            Instance = instance,
            DagConfig = dagConfig,
            DagNode = node,
            InstanceNode = instanceNode,
            BusinessData = instance.BusinessData,
            FormData = instance.FormData
        };

        _logger.LogInformation("执行 HandleEnterAsync for {NodeType}", node.Type);
        await handler.HandleEnterAsync(context);
        _logger.LogInformation("HandleEnterAsync 完成 for {NodeType}", node.Type);
    }

    private async Task ProcessStartNode(AntWorkflowInstance instance, DagConfig dagConfig,
        DagNode startNode, Guid initiatorId, string initiatorName)
    {
        // 获取开始节点的实例记录
        var instanceNode = await _db.Queryable<AntWorkflowInstanceNode>()
            .Where(n => n.InstanceId == instance.Id && n.NodeId == startNode.Id)
            .FirstAsync();

        var handler = _handlerFactory.GetHandler(AntNodeType.Start);
        if (handler != null)
        {
            var context = new NodeHandlerContext
            {
                Db = _db,
                Instance = instance,
                DagConfig = dagConfig,
                DagNode = startNode,
                InstanceNode = instanceNode,
                BusinessData = instance.BusinessData,
                FormData = instance.FormData,
                OperatorId = initiatorId,
                OperatorName = initiatorName
            };

            await handler.HandleEnterAsync(context);
            await handler.HandleCompleteAsync(context);

            // 推进到下一节点
            var nextNodeIds = await handler.GetNextNodesAsync(context);
            foreach (var nextNodeId in nextNodeIds)
            {
                await EnterNextNode(instance, dagConfig, nextNodeId);
            }
        }
    }

    private async Task EnterNextNode(AntWorkflowInstance instance, DagConfig dagConfig, string nodeId)
    {
        var node = dagConfig.Nodes.FirstOrDefault(n => n.Id == nodeId);
        if (node == null) return;

        var instanceNode = await _db.Queryable<AntWorkflowInstanceNode>()
            .Where(n => n.InstanceId == instance.Id && n.NodeId == nodeId)
            .FirstAsync();

        instanceNode.ApproveStatus = (int)NodeApproveStatus.Processing;
        await _db.Updateable(instanceNode).ExecuteCommandAsync();

        // 根据节点类型处理
        var handler = _handlerFactory.GetHandler(node.Type);
        if (handler != null)
        {
            var context = new NodeHandlerContext
            {
                Db = _db,
                Instance = instance,
                DagConfig = dagConfig,
                DagNode = node,
                InstanceNode = instanceNode,
                BusinessData = instance.BusinessData,
                FormData = instance.FormData
            };

            await handler.HandleEnterAsync(context);
        }
    }

    /// <inheritdoc/>
    public async Task<PageResponse<AntWorkflowInstanceDto>> GetMyInstancesAsync(
        QueryMyInstanceDto query, Guid userId)
    {
        var queryable = _db.Queryable<AntWorkflowInstance>()
            .Where(i => i.InitiatorId == userId);

        if (!string.IsNullOrEmpty(query.WorkflowName))
        {
            // 关联查询流程名称
            var workflowIds = await _db.Queryable<AntWorkflowEntity>()
                .Where(w => w.Name.Contains(query.WorkflowName))
                .Select(w => w.Id)
                .ToListAsync();
            queryable = queryable.WhereIF(workflowIds.Count > 0, i => workflowIds.Contains(i.WorkflowId));
        }

        // 使用 WhereIF
        queryable = queryable
            .WhereIF(!string.IsNullOrEmpty(query.BusinessType), i => i.BusinessType == query.BusinessType)
            .WhereIF(query.Status.HasValue, i => i.Status == query.Status!.Value);

        var totalCount = await queryable.CountAsync();
        var list = await queryable
            .OrderByDescending(i => i.CreateTime)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // 获取流程名称
        var wfIds = list.Select(i => i.WorkflowId).Distinct().ToList();
        var workflows = await _db.Queryable<AntWorkflowEntity>()
            .Where(w => wfIds.Contains(w.Id))
            .ToListAsync();
        var workflowDict = workflows.ToDictionary(w => w.Id, w => w.Name);

        var dtoList = list.Select(i => new AntWorkflowInstanceDto
        {
            Id = i.Id,
            WorkflowId = i.WorkflowId,
            WorkflowName = workflowDict.GetValueOrDefault(i.WorkflowId),
            Title = i.Title,
            BusinessId = i.BusinessId,
            BusinessType = i.BusinessType,
            Status = i.Status,
            InitiatorId = i.InitiatorId,
            StartTime = i.StartTime,
            FinishTime = i.FinishTime,
            CreateTime = i.CreateTime
        }).ToList();

        return PageResponse<AntWorkflowInstanceDto>.Create(dtoList, totalCount, query.PageIndex, query.PageSize);
    }

    /// <inheritdoc/>
    public async Task<AntWorkflowInstanceDetailDto?> GetInstanceDetailAsync(Guid instanceId)
    {
        var instance = await _db.Queryable<AntWorkflowInstance>()
            .Where(i => i.Id == instanceId)
            .FirstAsync();

        if (instance == null) return null;

        var workflow = await _db.Queryable<AntWorkflowEntity>()
            .Where(w => w.Id == instance.WorkflowId)
            .FirstAsync();

        // 查询发起人姓名
        string? initiatorName = null;
        if (instance.InitiatorId.HasValue)
        {
            var initiator = await _db.Queryable<User>()
                .Where(u => u.Id == instance.InitiatorId.Value)
                .FirstAsync();
            initiatorName = initiator?.RealName;
        }

        // 获取节点状态
        var nodeStatusList = await _db.Queryable<AntWorkflowInstanceNode>()
            .Where(n => n.InstanceId == instanceId)
            .ToListAsync();

        // 获取审批记录
        var approveRecords = await _db.Queryable<AntWorkflowApproveRecord>()
            .Where(r => r.InstanceId == instanceId)
            .OrderByDescending(r => r.ApproveTime)
            .ToListAsync();

        // 获取当前处理人
        var currentTasks = await _db.Queryable<AntWorkflowCurrentTask>()
            .Where(t => t.InstanceId == instanceId && t.ActiveStatus == 1)
            .ToListAsync();

        return new AntWorkflowInstanceDetailDto
        {
            Instance = new AntWorkflowInstanceDto
            {
                Id = instance.Id,
                WorkflowId = instance.WorkflowId,
                WorkflowName = workflow?.Name,
                Title = instance.Title,
                BusinessId = instance.BusinessId,
                BusinessType = instance.BusinessType,
                Status = instance.Status,
                InitiatorId = instance.InitiatorId,
                InitiatorName = initiatorName,
                StartTime = instance.StartTime,
                FinishTime = instance.FinishTime,
                CreateTime = instance.CreateTime
            },
            BusinessData = instance.BusinessData,
            FormData = instance.FormData,
            FlowConfig = instance.FlowConfig,
            NodeStatusList = nodeStatusList.Select(n => new AntNodeStatusDto
            {
                NodeId = n.NodeId,
                NodeName = n.NodeName,
                NodeType = n.NodeType,
                ApproveStatus = n.ApproveStatus,
                Handlers = GetHandlersForNode(n, currentTasks)
            }).ToList(),
            ApproveRecords = approveRecords.Select(r => new AntExecutionLogDto
            {
                Id = r.Id,
                NodeName = r.NodeName,
                NodeType = r.NodeType,
                HandlerName = r.HandlerName,
                ApproveStatus = r.ApproveStatus,
                ApproveDesc = r.ApproveDesc,
                ApproveTime = r.ApproveTime,
                Duration = r.Duration,
                TransferToName = r.TransferToName
            }).ToList()
        };
    }

    private List<AntHandlerDto>? GetHandlersForNode(AntWorkflowInstanceNode node, List<AntWorkflowCurrentTask> tasks)
    {
        var nodeTasks = tasks.Where(t => t.NodeId == node.NodeId).ToList();
        if (nodeTasks.Count == 0) return null;

        // 查询用户姓名
        return nodeTasks.Select(t => new AntHandlerDto
        {
            UserId = t.HandlerId,
            UserName = null // 需要关联用户表查询，这里暂时返回null
        }).ToList();
    }

    /// <inheritdoc/>
    public async Task<int> CancelAsync(Guid instanceId, Guid userId, string? reason)
    {
        var instance = await _db.Queryable<AntWorkflowInstance>()
            .Where(i => i.Id == instanceId && i.InitiatorId == userId)
            .FirstAsync();

        if (instance == null) return 0;

        if (instance.Status != (int)InstanceStatus.Approving)
        {
            throw new Exception("只有审批中的流程可以撤回");
        }

        // 更新实例状态
        instance.Status = (int)InstanceStatus.Withdrawn;
        instance.FinishTime = DateTime.Now;
        instance.UpdateTime = DateTime.Now;

        // 创建撤回记录
        var record = new AntWorkflowApproveRecord
        {
            Id = Guid.NewGuid(),
            InstanceId = instanceId,
            HandlerId = userId,
            ApproveStatus = (int)ApproveStatus.Withdraw,
            ApproveDesc = reason ?? "发起人撤回",
            ApproveTime = DateTime.Now
        };

        await _db.Ado.UseTranAsync(async () =>
        {
            await _db.Updateable(instance).ExecuteCommandAsync();
            await _db.Insertable(record).ExecuteCommandAsync();

            // 删除待办任务
            await _db.Deleteable<AntWorkflowCurrentTask>()
                .Where(t => t.InstanceId == instanceId)
                .ExecuteCommandAsync();
        });

        // 执行撤回回调，更新业务状态
        await ExecuteWithdrawCallbackAsync(instance, reason);

        return 1;
    }

    /// <summary>
    /// 执行撤回回调，更新业务状态
    /// </summary>
    private async Task ExecuteWithdrawCallbackAsync(AntWorkflowInstance instance, string? reason)
    {
        try
        {
            _logger.LogInformation("开始执行撤回回调: InstanceId={InstanceId}, BusinessId={BusinessId}",
                instance.Id, instance.BusinessId);

            if (string.IsNullOrEmpty(instance.BusinessId))
            {
                _logger.LogWarning("业务ID为空，无法执行撤回回调");
                return;
            }

            // 获取审核点编码
            string? auditPointCode = null;
            if (!string.IsNullOrEmpty(instance.BusinessData))
            {
                try
                {
                    var businessData = JsonConvert.DeserializeObject<Dictionary<string, object>>(instance.BusinessData);
                    if (businessData?.TryGetValue("AuditPointCode", out var code) == true)
                    {
                        auditPointCode = code?.ToString();
                    }
                }
                catch
                {
                    // 解析失败，忽略
                }
            }

            // 如果没有从 BusinessData 获取，尝试从业务表获取
            if (string.IsNullOrEmpty(auditPointCode) && !string.IsNullOrEmpty(instance.BusinessId))
            {
                try
                {
                    var businessId = Guid.Parse(instance.BusinessId);
                    var entity = await _db.Queryable<Product>()
                        .Where(p => p.Id == businessId)
                        .FirstAsync();
                    auditPointCode = entity?.AuditPointCode;
                }
                catch
                {
                    // 解析失败，忽略
                }
            }

            if (string.IsNullOrEmpty(auditPointCode))
            {
                _logger.LogWarning("无法获取审核点编码，使用默认撤回状态");
                // 使用默认撤回状态值（0 = 待审核）
                await UpdateBusinessStatusAsync(instance, 0, reason);
                return;
            }

            var auditPoint = await _businessAuditPointService.GetByCodeAsync(auditPointCode);
            if (auditPoint == null)
            {
                _logger.LogWarning("审核点配置不存在: {AuditPointCode}", auditPointCode);
                await UpdateBusinessStatusAsync(instance, 0, reason);
                return;
            }

            // 更新业务状态为撤回状态
            await UpdateBusinessStatusAsync(instance, auditPoint.WithdrawStatusValue, reason);

            _logger.LogInformation("撤回回调执行完成: AuditStatus={AuditStatus}", auditPoint.WithdrawStatusValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行撤回回调失败: InstanceId={InstanceId}", instance.Id);
        }
    }

    /// <summary>
    /// 更新业务状态
    /// </summary>
    private async Task UpdateBusinessStatusAsync(AntWorkflowInstance instance, int auditStatus, string? remark = null)
    {
        if (string.IsNullOrEmpty(instance.BusinessId))
        {
            return;
        }

        try
        {
            var businessId = Guid.Parse(instance.BusinessId);

            var updateable = _db.Updateable<Product>()
                .Where(p => p.Id == businessId)
                .SetColumns(p => p.AuditStatus == auditStatus)
                .SetColumns(p => p.AuditTime == DateTime.Now)
                .SetColumns(p => p.UpdateTime == DateTime.Now);

            if (!string.IsNullOrEmpty(remark))
            {
                updateable = updateable.SetColumns(p => p.AuditRemark == remark);
            }

            await updateable.ExecuteCommandAsync();

            _logger.LogInformation("业务状态更新成功: BusinessId={BusinessId}, AuditStatus={AuditStatus}",
                businessId, auditStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新业务状态失败: BusinessId={BusinessId}", instance.BusinessId);
        }
    }

    /// <inheritdoc/>
    public async Task<List<AntExecutionLogDto>> GetLogsAsync(Guid instanceId)
    {
        var records = await _db.Queryable<AntWorkflowApproveRecord>()
            .Where(r => r.InstanceId == instanceId)
            .OrderBy(r => r.ApproveTime)
            .ToListAsync();

        return records.Select(r => new AntExecutionLogDto
        {
            Id = r.Id,
            NodeName = r.NodeName,
            NodeType = r.NodeType,
            HandlerName = r.HandlerName,
            ApproveStatus = r.ApproveStatus,
            ApproveDesc = r.ApproveDesc,
            ApproveTime = r.ApproveTime,
            Duration = r.Duration,
            TransferToName = r.TransferToName
        }).ToList();
    }
}