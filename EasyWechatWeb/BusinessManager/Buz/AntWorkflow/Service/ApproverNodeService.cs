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
/// 审批节点处理器服务
/// </summary>
public class ApproverNodeService : INodeHandlerService
{
    public AntNodeType NodeType => AntNodeType.Approver;

    /// <summary>日志服务（Autofac 属性注入）</summary>
    public ILogger<ApproverNodeService> _logger { get; set; } = null!;

    /// <summary>审批人解析服务（Autofac 属性注入）</summary>
    public IApproverResolverService _approverResolverService { get; set; } = null!;

    /// <summary>审批模式处理器（Autofac 属性注入）</summary>
    public IApproveModeHandler _approveModeHandler { get; set; } = null!;

    /// <inheritdoc/>
    public async Task HandleEnterAsync(NodeHandlerContext context)
    {
        _logger.LogInformation("审批节点进入处理: NodeId={NodeId}, NodeName={NodeName}",
            context.DagNode.Id, context.DagNode.Name);

        // 解析审批节点配置
        var config = JsonConvert.DeserializeObject<ApproverNodeConfig>(context.DagNode.Config?.ToString() ?? "");
        if (config == null)
        {
            _logger.LogWarning("审批节点配置为空，自动通过: NodeId={NodeId}", context.DagNode.Id);
            // 没有配置，自动通过
            context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
            return;
        }

        _logger.LogInformation("审批节点配置解析成功: SetType={SetType}, NodeUserListCount={NodeUserListCount}, NoHandlerAction={NoHandlerAction}",
            config.SetType, config.NodeUserList?.Count ?? 0, config.NoHandlerAction);

        // 使用 ApproverResolverService 获取审批人
        var handlers = await _approverResolverService.GetApproversAsync(context, config);
        _logger.LogInformation("获取处理人列表: HandlersCount={HandlersCount}", handlers.Count);

        if (handlers.Count == 0)
        {
            _logger.LogWarning("无处理人，执行 NoHandlerAction: Action={Action}", config.NoHandlerAction);
            // 无处理人，根据配置处理
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
                    handlers.Add(admin);
                    // 使用 ApproveModeHandler 创建任务
                    await _approveModeHandler.CreateTasksAsync(context, config, handlers);
                    _logger.LogInformation("已为管理员创建审批任务: AdminId={AdminId}", admin.TargetId);
                }
                else
                {
                    _logger.LogWarning("未找到管理员，自动通过");
                    context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
                    await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
                }
            }
            return;
        }

        // 使用 ApproveModeHandler 创建待办任务
        _logger.LogInformation("开始创建待办任务");
        await _approveModeHandler.CreateTasksAsync(context, config, handlers);
        _logger.LogInformation("待办任务创建完成: Count={Count}", handlers.Count);
    }

    /// <inheritdoc/>
    public async Task HandleCompleteAsync(NodeHandlerContext context)
    {
        // 解析审批节点配置
        var config = JsonConvert.DeserializeObject<ApproverNodeConfig>(context.DagNode.Config?.ToString() ?? "");

        // 使用 ApproveModeHandler 处理审批通过后的清理
        if (config != null)
        {
            await _approveModeHandler.HandleApprovePassAsync(context, config);
        }

        // 更新节点状态为已完成
        context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
        await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();

        // 删除该节点的所有待办任务
        await context.Db.Deleteable<AntWorkflowCurrentTask>()
            .Where(t => t.InstanceId == context.Instance.Id && t.NodeId == context.DagNode.Id)
            .ExecuteCommandAsync();

        _logger.LogInformation("审批节点完成处理: NodeId={NodeId}", context.DagNode.Id);
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetNextNodesAsync(NodeHandlerContext context)
    {
        // 根据边找到下一节点
        var edges = context.DagConfig.Edges
            .Where(e => e.SourceNodeId == context.DagNode.Id)
            .ToList();

        return edges.Select(e => e.TargetNodeId).ToList();
    }
}