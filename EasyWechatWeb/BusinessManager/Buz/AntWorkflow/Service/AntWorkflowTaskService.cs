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
/// Ant流程任务服务实现
/// </summary>
public class AntWorkflowTaskService : IAntWorkflowTaskService
{
    /// <summary>数据库上下文（Autofac 属性注入）</summary>
    public ISqlSugarClient _db { get; set; } = null!;

    /// <summary>节点处理器工厂（Autofac 属性注入）</summary>
    public NodeHandlerFactory _handlerFactory { get; set; } = null!;

    /// <summary>业务审核点服务（Autofac 属性注入）</summary>
    public IBusinessAuditPointService _businessAuditPointService { get; set; } = null!;

    /// <summary>日志服务（Autofac 属性注入）</summary>
    public ILogger<AntWorkflowTaskService> _logger { get; set; } = null!;

    /// <inheritdoc/>
    public async Task<PageResponse<AntWorkflowTaskDto>> GetTodoTasksAsync(QueryTodoTaskDto query, Guid userId, bool isAdmin = false)
    {
        // 先查询符合条件的实例ID - 使用 WhereIF
        var instanceQueryable = _db.Queryable<AntWorkflowInstance>()
            .Where(i => i.Status == (int)InstanceStatus.Approving);

        if (!string.IsNullOrEmpty(query.WorkflowName))
        {
            var workflowIds = await _db.Queryable<AntWorkflowEntity>()
                .Where(w => w.Name.Contains(query.WorkflowName))
                .Select(w => w.Id)
                .ToListAsync();

            // 如果没有匹配的流程，直接返回空结果
            if (workflowIds.Count == 0)
            {
                return PageResponse<AntWorkflowTaskDto>.Create(new List<AntWorkflowTaskDto>(), 0, query.PageIndex, query.PageSize);
            }
            instanceQueryable = instanceQueryable.Where(i => workflowIds.Contains(i.WorkflowId));
        }

        instanceQueryable = instanceQueryable.WhereIF(!string.IsNullOrEmpty(query.BusinessType), i => i.BusinessType == query.BusinessType);

        var instanceIds = await instanceQueryable.Select(i => i.Id).ToListAsync();

        // 如果没有符合条件的实例，直接返回空结果
        if (instanceIds.Count == 0)
        {
            return PageResponse<AntWorkflowTaskDto>.Create(new List<AntWorkflowTaskDto>(), 0, query.PageIndex, query.PageSize);
        }

        // 查询待办任务 - 管理员可查看所有任务，普通用户只能查看自己的任务
        var taskQueryable = _db.Queryable<AntWorkflowCurrentTask>()
            .Where(t => t.ActiveStatus == 1 && instanceIds.Contains(t.InstanceId))
            .WhereIF(!isAdmin, t => t.HandlerId == userId);

        var totalCount = await taskQueryable.CountAsync();
        var tasks = await taskQueryable
            .OrderByDescending(t => t.EntryTime)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // 补充实例信息
        var taskInstanceIds = tasks.Select(t => t.InstanceId).Distinct().ToList();
        var instances = taskInstanceIds.Count > 0
            ? await _db.Queryable<AntWorkflowInstance>()
                .Where(i => taskInstanceIds.Contains(i.Id))
                .ToListAsync()
            : new List<AntWorkflowInstance>();
        var instanceDict = instances.ToDictionary(i => i.Id);

        var list = tasks.Select(t => new AntWorkflowTaskDto
        {
            Id = t.Id,
            InstanceId = t.InstanceId,
            InstanceTitle = instanceDict.GetValueOrDefault(t.InstanceId)?.Title,
            NodeId = t.NodeId,
            NodeType = t.NodeType,
            EntryTime = t.EntryTime,
            DueTime = t.DueTime,
            InitiatorId = instanceDict.GetValueOrDefault(t.InstanceId)?.InitiatorId,
            TaskType = t.TaskType
        }).ToList();

        // 补充节点名称和发起人信息
        await EnrichTaskList(list);

        return PageResponse<AntWorkflowTaskDto>.Create(list, totalCount, query.PageIndex, query.PageSize);
    }

    /// <inheritdoc/>
    public async Task<PageResponse<AntWorkflowTaskDto>> GetDoneTasksAsync(QueryDoneTaskDto query, Guid userId, bool isAdmin = false)
    {
        var queryable = _db.Queryable<AntWorkflowApproveRecord, AntWorkflowInstance>((r, i) =>
            new JoinQueryInfos(JoinType.Inner, r.InstanceId == i.Id))
            // 管理员可查看所有已办任务，普通用户只能查看自己的
            .WhereIF(!isAdmin, (r, i) => r.HandlerId == userId);

        if (!string.IsNullOrEmpty(query.WorkflowName))
        {
            var workflowIds = await _db.Queryable<AntWorkflowEntity>()
                .Where(w => w.Name.Contains(query.WorkflowName))
                .Select(w => w.Id)
                .ToListAsync();
            queryable = queryable.WhereIF(workflowIds.Count > 0, (r, i) => workflowIds.Contains(i.WorkflowId));
        }

        // 使用 WhereIF
        queryable = queryable
            .WhereIF(query.StartTime.HasValue, (r, i) => r.ApproveTime >= query.StartTime!.Value)
            .WhereIF(query.EndTime.HasValue, (r, i) => r.ApproveTime <= query.EndTime!.Value);

        var totalCount = await queryable.CountAsync();
        var records = await queryable
            .OrderByDescending((r, i) => r.ApproveTime)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var list = records.Select(r => new AntWorkflowTaskDto
        {
            Id = r.Id,
            InstanceId = r.InstanceId,
            NodeId = r.NodeId ?? string.Empty,
            NodeName = r.NodeName,
            NodeType = r.NodeType ?? 0,
            InitiatorId = Guid.Empty, // 需要补充
            TaskType = 1
        }).ToList();

        // 补充信息
        var instanceIds = list.Select(t => t.InstanceId).Distinct().ToList();
        var instances = instanceIds.Count > 0
            ? await _db.Queryable<AntWorkflowInstance>()
                .Where(i => instanceIds.Contains(i.Id))
                .ToListAsync()
            : new List<AntWorkflowInstance>();
        var instanceDict = instances.ToDictionary(i => i.Id);

        foreach (var task in list)
        {
            if (instanceDict.TryGetValue(task.InstanceId, out var instance))
            {
                task.InstanceTitle = instance.Title;
                task.InitiatorId = instance.InitiatorId;
            }
        }

        return PageResponse<AntWorkflowTaskDto>.Create(list, totalCount, query.PageIndex, query.PageSize);
    }

    /// <inheritdoc/>
    public async Task<PageResponse<AntWorkflowCcDto>> GetCcTasksAsync(QueryCcTaskDto query, Guid userId, bool isAdmin = false)
    {
        var queryable = _db.Queryable<AntWorkflowCCRecord>()
            // 管理员可查看所有抄送，普通用户只能查看发给自己的
            .WhereIF(!isAdmin, c => c.ToUserId == userId)
            // 条件筛选 - 使用 WhereIF
            .WhereIF(query.IsRead.HasValue, c => c.IsRead == query.IsRead!.Value);

        // 按流程名称筛选
        if (!string.IsNullOrEmpty(query.WorkflowName))
        {
            var workflowIds = await _db.Queryable<AntWorkflowEntity>()
                .Where(w => w.Name.Contains(query.WorkflowName))
                .Select(w => w.Id)
                .ToListAsync();

            if (workflowIds.Count > 0)
            {
                var filteredInstanceIds = await _db.Queryable<AntWorkflowInstance>()
                    .Where(i => workflowIds.Contains(i.WorkflowId))
                    .Select(i => i.Id)
                    .ToListAsync();

                queryable = queryable.WhereIF(filteredInstanceIds.Count > 0, c => filteredInstanceIds.Contains(c.InstanceId));
            }
            else
            {
                // 没有匹配的流程，返回空结果
                return PageResponse<AntWorkflowCcDto>.Create(new List<AntWorkflowCcDto>(), 0, query.PageIndex, query.PageSize);
            }
        }

        var totalCount = await queryable.CountAsync();
        var list = await queryable
            .OrderByDescending(c => c.SendTime)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // 补充实例标题
        var instanceIds = list.Select(c => c.InstanceId).Distinct().ToList();
        var instances = instanceIds.Count > 0
            ? await _db.Queryable<AntWorkflowInstance>()
                .Where(i => instanceIds.Contains(i.Id))
                .ToListAsync()
            : new List<AntWorkflowInstance>();
        var instanceDict = instances.ToDictionary(i => i.Id);

        var dtoList = list.Select(c => new AntWorkflowCcDto
        {
            Id = c.Id,
            InstanceId = c.InstanceId,
            InstanceTitle = instanceDict.GetValueOrDefault(c.InstanceId)?.Title,
            NodeName = c.NodeName,
            FromUserName = null, // 需要关联用户表
            SendTime = c.SendTime,
            IsRead = c.IsRead
        }).ToList();

        return PageResponse<AntWorkflowCcDto>.Create(dtoList, totalCount, query.PageIndex, query.PageSize);
    }

    /// <inheritdoc/>
    public async Task<AntWorkflowTaskDto?> GetTaskDetailAsync(Guid taskId)
    {
        var task = await _db.Queryable<AntWorkflowCurrentTask>()
            .Where(t => t.Id == taskId)
            .FirstAsync();

        if (task == null) return null;

        var instance = await _db.Queryable<AntWorkflowInstance>()
            .Where(i => i.Id == task.InstanceId)
            .FirstAsync();

        var instanceNode = await _db.Queryable<AntWorkflowInstanceNode>()
            .Where(n => n.Id == task.InstanceNodeId)
            .FirstAsync();

        return new AntWorkflowTaskDto
        {
            Id = task.Id,
            InstanceId = task.InstanceId,
            InstanceTitle = instance?.Title,
            NodeId = task.NodeId,
            NodeName = instanceNode?.NodeName,
            NodeType = task.NodeType,
            EntryTime = task.EntryTime,
            DueTime = task.DueTime,
            InitiatorId = instance?.InitiatorId,
            TaskType = task.TaskType
        };
    }

    /// <inheritdoc/>
    public async Task<int> ApproveAsync(ApproveAntTaskDto dto, Guid userId, string userName)
    {
        var task = await _db.Queryable<AntWorkflowCurrentTask>()
            .Where(t => t.Id == dto.TaskId && t.HandlerId == userId && t.ActiveStatus == 1)
            .FirstAsync();

        if (task == null) return 0;

        var instance = await _db.Queryable<AntWorkflowInstance>()
            .Where(i => i.Id == task.InstanceId)
            .FirstAsync();

        if (instance == null || instance.Status != (int)InstanceStatus.Approving)
        {
            throw new Exception("流程不在审批中状态");
        }

        // 解析 DAG 配置
        var dagConfig = JsonConvert.DeserializeObject<DagConfig>(instance.FlowConfig ?? "");
        if (dagConfig == null) throw new Exception("流程配置无效");

        var dagNode = dagConfig.Nodes.FirstOrDefault(n => n.Id == task.NodeId);
        var instanceNode = await _db.Queryable<AntWorkflowInstanceNode>()
            .Where(n => n.InstanceId == task.InstanceId && n.NodeId == task.NodeId)
            .FirstAsync();

        // 计算处理时长
        var duration = task.EntryTime.HasValue
            ? (int)(DateTime.Now - task.EntryTime.Value).TotalSeconds
            : 0;

        // 创建审批记录
        var record = new AntWorkflowApproveRecord
        {
            Id = Guid.NewGuid(),
            InstanceId = task.InstanceId,
            InstanceNodeId = task.InstanceNodeId,
            NodeId = task.NodeId,
            NodeName = instanceNode?.NodeName,
            NodeType = task.NodeType,
            HandlerId = userId,
            HandlerName = userName,
            ApproveStatus = (int)ApproveStatus.Pass,
            ApproveDesc = dto.ApproveDesc,
            ApproveTime = DateTime.Now,
            EntryTime = task.EntryTime,
            Duration = duration
        };

        // 更新实例数据
        if (!string.IsNullOrEmpty(dto.FormData))
        {
            instance.FormData = dto.FormData;
            instance.UpdateTime = DateTime.Now;
        }

        await _db.Ado.UseTranAsync(async () =>
        {
            await _db.Insertable(record).ExecuteCommandAsync();

            // 获取处理器并完成节点
            var handler = _handlerFactory.GetHandler((AntNodeType)task.NodeType);
            if (handler != null)
            {
                var context = new NodeHandlerContext
                {
                    Db = _db,
                    Instance = instance,
                    DagConfig = dagConfig,
                    DagNode = dagNode!,
                    InstanceNode = instanceNode!,
                    BusinessData = instance.BusinessData,
                    FormData = instance.FormData,
                    OperatorId = userId,
                    OperatorName = userName,
                    ApproveDesc = dto.ApproveDesc
                };

                await handler.HandleCompleteAsync(context);

                // 推进到下一节点
                var nextNodeIds = await handler.GetNextNodesAsync(context);
                foreach (var nextNodeId in nextNodeIds)
                {
                    await EnterNextNode(instance, dagConfig, nextNodeId);
                }
            }

            // 更新实例
            if (instance.Status != (int)InstanceStatus.Passed)
            {
                await _db.Updateable(instance).ExecuteCommandAsync();
            }
        });

        return 1;
    }

    /// <inheritdoc/>
    public async Task<int> RejectAsync(RejectAntTaskDto dto, Guid userId, string userName)
    {
        var task = await _db.Queryable<AntWorkflowCurrentTask>()
            .Where(t => t.Id == dto.TaskId && t.HandlerId == userId && t.ActiveStatus == 1)
            .FirstAsync();

        if (task == null) return 0;

        var instance = await _db.Queryable<AntWorkflowInstance>()
            .Where(i => i.Id == task.InstanceId)
            .FirstAsync();

        if (instance == null || instance.Status != (int)InstanceStatus.Approving)
        {
            throw new Exception("流程不在审批中状态");
        }

        var instanceNode = await _db.Queryable<AntWorkflowInstanceNode>()
            .Where(n => n.InstanceId == task.InstanceId && n.NodeId == task.NodeId)
            .FirstAsync();

        // 计算处理时长
        var duration = task.EntryTime.HasValue
            ? (int)(DateTime.Now - task.EntryTime.Value).TotalSeconds
            : 0;

        // 创建驳回记录
        var record = new AntWorkflowApproveRecord
        {
            Id = Guid.NewGuid(),
            InstanceId = task.InstanceId,
            InstanceNodeId = task.InstanceNodeId,
            NodeId = task.NodeId,
            NodeName = instanceNode?.NodeName,
            NodeType = task.NodeType,
            HandlerId = userId,
            HandlerName = userName,
            ApproveStatus = (int)ApproveStatus.Reject,
            ApproveDesc = dto.RejectReason,
            ApproveTime = DateTime.Now,
            EntryTime = task.EntryTime,
            Duration = duration
        };

        // 更新实例状态为驳回
        instance.Status = (int)InstanceStatus.Rejected;
        instance.FinishTime = DateTime.Now;
        instance.UpdateTime = DateTime.Now;

        await _db.Ado.UseTranAsync(async () =>
        {
            await _db.Insertable(record).ExecuteCommandAsync();
            await _db.Updateable(instance).ExecuteCommandAsync();

            // 删除所有待办任务
            await _db.Deleteable<AntWorkflowCurrentTask>()
                .Where(t => t.InstanceId == task.InstanceId)
                .ExecuteCommandAsync();

            // 更新节点状态
            instanceNode!.ApproveStatus = (int)NodeApproveStatus.Completed;
            await _db.Updateable(instanceNode).ExecuteCommandAsync();
        });

        // 执行驳回回调，更新业务状态
        await ExecuteRejectCallbackAsync(instance, dto.RejectReason);

        return 1;
    }

    /// <summary>
    /// 执行驳回回调，更新业务状态
    /// </summary>
    private async Task ExecuteRejectCallbackAsync(AntWorkflowInstance instance, string? rejectReason)
    {
        try
        {
            _logger.LogInformation("开始执行驳回回调: InstanceId={InstanceId}, BusinessId={BusinessId}",
                instance.Id, instance.BusinessId);

            if (string.IsNullOrEmpty(instance.BusinessId))
            {
                _logger.LogWarning("业务ID为空，无法执行驳回回调");
                return;
            }

            // 获取审核点编码
            string? auditPointCode = null;
            if (!string.IsNullOrEmpty(instance.BusinessData))
            {
                try
                {
                    var businessData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(instance.BusinessData);
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
                _logger.LogWarning("无法获取审核点编码，使用默认驳回状态");
                // 使用默认驳回状态值
                await UpdateBusinessStatusAsync(instance, 2, rejectReason);
                return;
            }

            var auditPoint = await _businessAuditPointService.GetByCodeAsync(auditPointCode);
            if (auditPoint == null)
            {
                _logger.LogWarning("审核点配置不存在: {AuditPointCode}", auditPointCode);
                await UpdateBusinessStatusAsync(instance, 2, rejectReason);
                return;
            }

            // 更新业务状态为驳回状态
            await UpdateBusinessStatusAsync(instance, auditPoint.RejectStatusValue, rejectReason);

            _logger.LogInformation("驳回回调执行完成: AuditStatus={AuditStatus}", auditPoint.RejectStatusValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行驳回回调失败: InstanceId={InstanceId}", instance.Id);
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
    public async Task<int> TransferAsync(TransferAntTaskDto dto, Guid userId, string userName)
    {
        var task = await _db.Queryable<AntWorkflowCurrentTask>()
            .Where(t => t.Id == dto.TaskId && t.HandlerId == userId && t.ActiveStatus == 1)
            .FirstAsync();

        if (task == null) return 0;

        // 创建转交记录
        var instanceNode = await _db.Queryable<AntWorkflowInstanceNode>()
            .Where(n => n.Id == task.InstanceNodeId)
            .FirstAsync();

        var duration = task.EntryTime.HasValue
            ? (int)(DateTime.Now - task.EntryTime.Value).TotalSeconds
            : 0;

        var record = new AntWorkflowApproveRecord
        {
            Id = Guid.NewGuid(),
            InstanceId = task.InstanceId,
            InstanceNodeId = task.InstanceNodeId,
            NodeId = task.NodeId,
            NodeName = instanceNode?.NodeName,
            NodeType = task.NodeType,
            HandlerId = userId,
            HandlerName = userName,
            ApproveStatus = (int)ApproveStatus.Transfer,
            ApproveDesc = dto.TransferReason,
            ApproveTime = DateTime.Now,
            EntryTime = task.EntryTime,
            Duration = duration,
            TransferToId = dto.TransferToUserId
        };

        // 创建新任务给转交目标用户
        var newTask = new AntWorkflowCurrentTask
        {
            Id = Guid.NewGuid(),
            InstanceId = task.InstanceId,
            InstanceNodeId = task.InstanceNodeId,
            NodeId = task.NodeId,
            NodeType = task.NodeType,
            HandlerId = dto.TransferToUserId,
            HandlerType = 1,
            EntryTime = DateTime.Now,
            SourceNodeId = task.NodeId,
            SourceUserId = userId,
            TaskType = 1,
            ActiveStatus = 1
        };

        // 原任务设为不激活
        task.ActiveStatus = 0;

        await _db.Ado.UseTranAsync(async () =>
        {
            await _db.Insertable(record).ExecuteCommandAsync();
            await _db.Insertable(newTask).ExecuteCommandAsync();
            await _db.Updateable(task).ExecuteCommandAsync();
        });

        return 1;
    }

    /// <inheritdoc/>
    public async Task<int> MarkCcReadAsync(List<Guid> ids, Guid userId)
    {
        return await _db.Updateable<AntWorkflowCCRecord>()
            .SetColumns(c => c.IsRead == 1)
            .SetColumns(c => c.ReadTime == DateTime.Now)
            .Where(c => ids.Contains(c.Id) && c.ToUserId == userId)
            .ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<int> AddSignerAsync(AddSignerAntTaskDto dto, Guid userId, string userName)
    {
        var task = await _db.Queryable<AntWorkflowCurrentTask>()
            .Where(t => t.Id == dto.TaskId && t.HandlerId == userId && t.ActiveStatus == 1)
            .FirstAsync();

        if (task == null) return 0;

        var instance = await _db.Queryable<AntWorkflowInstance>()
            .Where(i => i.Id == task.InstanceId)
            .FirstAsync();

        if (instance == null || instance.Status != (int)InstanceStatus.Approving)
        {
            throw new Exception("流程不在审批中状态");
        }

        var instanceNode = await _db.Queryable<AntWorkflowInstanceNode>()
            .Where(n => n.Id == task.InstanceNodeId)
            .FirstAsync();

        // 创建加签审批记录
        var record = new AntWorkflowApproveRecord
        {
            Id = Guid.NewGuid(),
            InstanceId = task.InstanceId,
            InstanceNodeId = task.InstanceNodeId,
            NodeId = task.NodeId,
            NodeName = instanceNode?.NodeName,
            NodeType = task.NodeType,
            HandlerId = userId,
            HandlerName = userName,
            ApproveStatus = (int)ApproveStatus.AddSigner, // 加签状态
            ApproveDesc = dto.Reason ?? "加签",
            ApproveTime = DateTime.Now,
            EntryTime = task.EntryTime,
            Duration = 0
        };

        // 创建加签任务
        var newTasks = new List<AntWorkflowCurrentTask>();
        var taskOrder = task.NodeOrder;

        foreach (var signerId in dto.SignerIds)
        {
            var signerTask = new AntWorkflowCurrentTask
            {
                Id = Guid.NewGuid(),
                InstanceId = task.InstanceId,
                InstanceNodeId = task.InstanceNodeId,
                NodeId = task.NodeId,
                NodeType = task.NodeType,
                HandlerId = signerId,
                HandlerType = 1,
                EntryTime = DateTime.Now,
                SourceNodeId = task.NodeId,
                SourceUserId = userId,
                TaskType = 1,
                ActiveStatus = 1,
                NodeOrder = dto.SignerType == "before" ? taskOrder++ : taskOrder + 100
            };
            newTasks.Add(signerTask);
        }

        // 如果是前加签，原任务等待加签人完成后再处理
        if (dto.SignerType == "before")
        {
            task.NodeOrder = taskOrder;
            task.ActiveStatus = 0; // 暂时不激活
        }

        await _db.Ado.UseTranAsync(async () =>
        {
            await _db.Insertable(record).ExecuteCommandAsync();
            await _db.Insertable(newTasks).ExecuteCommandAsync();
            if (dto.SignerType == "before")
            {
                await _db.Updateable(task).ExecuteCommandAsync();
            }
        });

        return newTasks.Count;
    }

    private async Task EnrichTaskList(List<AntWorkflowTaskDto> list)
    {
        if (list.Count == 0) return;

        var instanceIds = list.Select(t => t.InstanceId).Distinct().ToList();

        // 避免空列表导致的无效查询
        var instances = instanceIds.Count > 0
            ? await _db.Queryable<AntWorkflowInstance>()
                .Where(i => instanceIds.Contains(i.Id))
                .ToListAsync()
            : new List<AntWorkflowInstance>();
        var instanceDict = instances.ToDictionary(i => i.Id);

        // 查询发起人姓名
        var initiatorIds = instances.Where(i => i.InitiatorId.HasValue).Select(i => i.InitiatorId!.Value).Distinct().ToList();
        var users = initiatorIds.Count > 0
            ? await _db.Queryable<User>()
                .Where(u => initiatorIds.Contains(u.Id))
                .ToListAsync()
            : new List<User>();
        var userDict = users.ToDictionary(u => u.Id);

        var nodeIds = list.Select(t => t.NodeId).Distinct().ToList();

        // 避免空列表导致的无效查询
        var instanceNodes = instanceIds.Count > 0 && nodeIds.Count > 0
            ? await _db.Queryable<AntWorkflowInstanceNode>()
                .Where(n => instanceIds.Contains(n.InstanceId) && nodeIds.Contains(n.NodeId))
                .ToListAsync()
            : new List<AntWorkflowInstanceNode>();

        foreach (var task in list)
        {
            if (instanceDict.TryGetValue(task.InstanceId, out var instance))
            {
                task.InitiatorId = instance.InitiatorId;
                // 设置发起人姓名
                if (instance.InitiatorId.HasValue && userDict.TryGetValue(instance.InitiatorId.Value, out var user))
                {
                    task.InitiatorName = user.RealName;
                }
            }

            var node = instanceNodes.FirstOrDefault(n => n.InstanceId == task.InstanceId && n.NodeId == task.NodeId);
            if (node != null)
            {
                task.NodeName = node.NodeName;
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

        if (instanceNode == null) return;

        instanceNode.ApproveStatus = (int)NodeApproveStatus.Processing;
        await _db.Updateable(instanceNode).ExecuteCommandAsync();

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

            // 如果节点自动完成（如抄送节点），继续推进
            if (instanceNode.ApproveStatus == (int)NodeApproveStatus.Completed)
            {
                var nextNodeIds = await handler.GetNextNodesAsync(context);
                foreach (var nextNodeId in nextNodeIds)
                {
                    await EnterNextNode(instance, dagConfig, nextNodeId);
                }
            }
        }
    }
}