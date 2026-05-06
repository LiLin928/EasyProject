using Newtonsoft.Json;
using EasyWeChatModels.Enums;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models.NodeConfigs;
using EasyWeChatModels.Models.AntWorkflow;
using BusinessManager.Buz.IService;
using BusinessManager.Buz.AntWorkflow.IService;
using Microsoft.Extensions.Logging;

namespace BusinessManager.Buz.AntWorkflow.Service;

/// <summary>
/// 会签节点处理器服务
/// </summary>
/// <remarks>
/// 会签节点支持三种通过条件：
/// - all: 所有审批人都通过
/// - percent: 达到指定比例通过（如60%通过）
/// - count: 达到指定数量通过（如3人通过）
/// 任一驳回则整个节点驳回
/// </remarks>
public class CounterSignNodeService : INodeHandlerService
{
    public AntNodeType NodeType => AntNodeType.CounterSign;

    /// <summary>日志服务（Autofac 属性注入）</summary>
    public ILogger<CounterSignNodeService> _logger { get; set; } = null!;

    /// <summary>审批人解析服务（Autofac 属性注入）</summary>
    public IApproverResolverService _approverResolverService { get; set; } = null!;

    /// <inheritdoc/>
    public async Task HandleEnterAsync(NodeHandlerContext context)
    {
        _logger.LogInformation("会签节点进入处理: NodeId={NodeId}, NodeName={NodeName}",
            context.DagNode.Id, context.DagNode.Name);

        // 解析会签节点配置
        var config = JsonConvert.DeserializeObject<CounterSignNodeConfig>(context.DagNode.Config?.ToString() ?? "");
        if (config == null)
        {
            _logger.LogWarning("会签节点配置为空，自动通过: NodeId={NodeId}", context.DagNode.Id);
            // 没有配置，自动通过
            context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
            return;
        }

        _logger.LogInformation("会签节点配置解析成功: SetType={SetType}, NodeUserListCount={NodeUserListCount}, NoHandlerAction={NoHandlerAction}, PassConditionType={PassConditionType}",
            config.SetType, config.NodeUserList?.Count ?? 0, config.NoHandlerAction, config.PassCondition?.Type ?? "all");

        // 构建 ApproverNodeConfig 用于获取审批人
        var approverConfig = new ApproverNodeConfig
        {
            SetType = config.SetType,
            NodeUserList = config.NodeUserList ?? new List<NodeUser>(),
            NoHandlerAction = config.NoHandlerAction,
            DirectorLevel = config.DirectorLevel
        };

        // 使用 ApproverResolverService 获取审批人
        var handlers = await _approverResolverService.GetApproversAsync(context, approverConfig);
        _logger.LogInformation("获取处理人列表: HandlersCount={HandlersCount}", handlers.Count);

        if (handlers.Count == 0)
        {
            await HandleNoHandlerAsync(context, config);
            return;
        }

        // 会签模式：创建所有审批人的待办任务（同时激活）
        foreach (var handler in handlers)
        {
            var task = new AntWorkflowCurrentTask
            {
                Id = Guid.NewGuid(),
                InstanceId = context.Instance.Id,
                InstanceNodeId = context.InstanceNode.Id,
                NodeId = context.DagNode.Id,
                NodeType = (int)AntNodeType.CounterSign,
                HandlerId = handler.TargetId,
                HandlerType = handler.Type,
                EntryTime = DateTime.Now,
                TaskType = 1, // 审批任务
                ActiveStatus = 1, // 全部激活
                NodeOrder = 1 // 会签所有任务顺序相同
            };
            await context.Db.Insertable(task).ExecuteCommandAsync();
        }

        _logger.LogInformation("会签任务创建完成: Count={Count}", handlers.Count);
    }

    /// <inheritdoc/>
    public async Task HandleCompleteAsync(NodeHandlerContext context)
    {
        _logger.LogInformation("会签节点完成处理: NodeId={NodeId}", context.DagNode.Id);

        // 解析会签节点配置
        var config = JsonConvert.DeserializeObject<CounterSignNodeConfig>(context.DagNode.Config?.ToString() ?? "");
        var passCondition = config?.PassCondition ?? new PassConditionConfig();

        // 检查会签是否完成
        var tasks = await context.Db.Queryable<AntWorkflowCurrentTask>()
            .Where(t => t.InstanceId == context.Instance.Id && t.NodeId == context.DagNode.Id && t.ActiveStatus == 1)
            .ToListAsync();

        // 获取该节点的审批记录
        var approveRecords = await context.Db.Queryable<AntWorkflowApproveRecord>()
            .Where(r => r.InstanceId == context.Instance.Id && r.NodeId == context.DagNode.Id)
            .ToListAsync();

        // 统计审批结果
        var passCount = approveRecords.Count(r => r.ApproveStatus == (int)ApproveStatus.Pass);
        var rejectCount = approveRecords.Count(r => r.ApproveStatus == (int)ApproveStatus.Reject);
        var totalHandlers = tasks.Count + approveRecords.Count; // 待处理 + 已处理

        _logger.LogInformation("会签统计: PassCount={PassCount}, RejectCount={RejectCount}, TotalHandlers={TotalHandlers}, RemainingTasks={RemainingTasks}",
            passCount, rejectCount, totalHandlers, tasks.Count);

        // 任一驳回则驳回
        if (rejectCount > 0)
        {
            await HandleRejectAsync(context, tasks);
            return;
        }

        // 根据通过条件判断
        bool isComplete = CheckPassCondition(passCondition, passCount, totalHandlers);

        if (isComplete)
        {
            context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();

            // 删除剩余的待办任务
            if (tasks.Count > 0)
            {
                await context.Db.Deleteable(tasks).ExecuteCommandAsync();
            }

            _logger.LogInformation("会签完成: PassCount={PassCount}, Total={Total}, Type={Type}",
                passCount, totalHandlers, passCondition.Type);
        }
        else
        {
            _logger.LogInformation("会签未完成，等待更多审批: PassCount={PassCount}, Total={Total}, Type={Type}",
                passCount, totalHandlers, passCondition.Type);
        }
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetNextNodesAsync(NodeHandlerContext context)
    {
        // 只有节点完成时才推进到下一节点
        if (context.InstanceNode.ApproveStatus != (int)NodeApproveStatus.Completed)
        {
            return new List<string>();
        }

        // 根据边找到下一节点
        var edges = context.DagConfig.Edges
            .Where(e => e.SourceNodeId == context.DagNode.Id)
            .ToList();

        return edges.Select(e => e.TargetNodeId).ToList();
    }

    /// <summary>
    /// 处理无审批人情况
    /// </summary>
    private async Task HandleNoHandlerAsync(NodeHandlerContext context, CounterSignNodeConfig config)
    {
        _logger.LogWarning("无处理人，执行 NoHandlerAction: Action={Action}", config.NoHandlerAction);

        if (config.NoHandlerAction == NoHandlerAction.AutoPass)
        {
            _logger.LogInformation("自动通过: NodeId={NodeId}", context.DagNode.Id);
            context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
        }
        else if (config.NoHandlerAction == NoHandlerAction.AutoReject)
        {
            _logger.LogInformation("自动驳回: NodeId={NodeId}", context.DagNode.Id);
            context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            context.Instance.Status = (int)InstanceStatus.Rejected;
            context.Instance.FinishTime = DateTime.Now;
            await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
            await context.Db.Updateable(context.Instance).ExecuteCommandAsync();
        }
        else if (config.NoHandlerAction == NoHandlerAction.Transfer)
        {
            _logger.LogInformation("转交管理员: NodeId={NodeId}", context.DagNode.Id);
            // 获取管理员作为审批人
            var admin = await _approverResolverService.GetAdminAsync();
            if (admin != null)
            {
                var task = new AntWorkflowCurrentTask
                {
                    Id = Guid.NewGuid(),
                    InstanceId = context.Instance.Id,
                    InstanceNodeId = context.InstanceNode.Id,
                    NodeId = context.DagNode.Id,
                    NodeType = (int)AntNodeType.CounterSign,
                    HandlerId = admin.TargetId,
                    HandlerType = admin.Type,
                    EntryTime = DateTime.Now,
                    TaskType = 1,
                    ActiveStatus = 1,
                    NodeOrder = 1
                };
                await context.Db.Insertable(task).ExecuteCommandAsync();
                _logger.LogInformation("已为管理员创建会签任务: AdminId={AdminId}", admin.TargetId);
            }
            else
            {
                _logger.LogWarning("未找到管理员，自动通过");
                context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
                await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
            }
        }
    }

    /// <summary>
    /// 处理驳回情况
    /// </summary>
    private async Task HandleRejectAsync(NodeHandlerContext context, List<AntWorkflowCurrentTask> tasks)
    {
        _logger.LogInformation("会签驳回，流程终止: NodeId={NodeId}", context.DagNode.Id);

        context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
        context.Instance.Status = (int)InstanceStatus.Rejected;
        context.Instance.FinishTime = DateTime.Now;

        // 删除所有待办任务
        if (tasks.Count > 0)
        {
            await context.Db.Deleteable(tasks).ExecuteCommandAsync();
        }

        await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
        await context.Db.Updateable(context.Instance).ExecuteCommandAsync();
    }

    /// <summary>
    /// 检查通过条件
    /// </summary>
    /// <param name="config">通过条件配置</param>
    /// <param name="passCount">已通过数量</param>
    /// <param name="totalHandlers">总处理人数</param>
    /// <returns>是否满足通过条件</returns>
    private bool CheckPassCondition(PassConditionConfig config, int passCount, int totalHandlers)
    {
        var result = config.Type switch
        {
            "all" => passCount >= totalHandlers && totalHandlers > 0,
            "percent" => totalHandlers > 0 && (passCount * 100 / totalHandlers) >= (config.Percent ?? 100),
            "count" => passCount >= (config.Count ?? totalHandlers),
            _ => passCount >= totalHandlers && totalHandlers > 0 // 默认全部通过
        };

        _logger.LogDebug("通过条件检查: Type={Type}, PassCount={PassCount}, Total={Total}, Percent={Percent}, Count={Count}, Result={Result}",
            config.Type, passCount, totalHandlers, config.Percent, config.Count, result);

        return result;
    }
}