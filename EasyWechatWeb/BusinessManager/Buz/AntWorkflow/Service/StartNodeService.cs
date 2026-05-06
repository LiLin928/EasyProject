using EasyWeChatModels.Enums;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models.AntWorkflow;
using BusinessManager.Buz.IService;
using Microsoft.Extensions.Logging;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 开始节点处理器服务（发起人节点）
/// </summary>
public class StartNodeService : INodeHandlerService
{
    public AntNodeType NodeType => AntNodeType.Start;

    /// <summary>日志服务（Autofac 属性注入）</summary>
    public ILogger<StartNodeService> _logger { get; set; } = null!;

    /// <inheritdoc/>
    public async Task HandleEnterAsync(NodeHandlerContext context)
    {
        _logger.LogInformation("开始节点进入处理: NodeId={NodeId}, NodeName={NodeName}",
            context.DagNode.Id, context.DagNode.Name);

        // 开始节点自动完成，设置状态为已完成
        context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
        await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();

        _logger.LogInformation("开始节点状态更新为 Completed: InstanceNodeId={InstanceNodeId}, ApproveStatus={ApproveStatus}",
            context.InstanceNode.Id, context.InstanceNode.ApproveStatus);
    }

    /// <inheritdoc/>
    public async Task HandleCompleteAsync(NodeHandlerContext context)
    {
        _logger.LogInformation("开始节点完成处理: NodeId={NodeId}, OperatorId={OperatorId}, OperatorName={OperatorName}",
            context.DagNode.Id, context.OperatorId, context.OperatorName);

        // 创建审批记录（发起人提交）
        var record = new AntWorkflowApproveRecord
        {
            Id = Guid.NewGuid(),
            InstanceId = context.Instance.Id,
            InstanceNodeId = context.InstanceNode.Id,
            NodeId = context.InstanceNode.NodeId,
            NodeName = context.InstanceNode.NodeName,
            NodeType = (int)AntNodeType.Start,
            HandlerId = context.Instance.InitiatorId ?? Guid.Empty,
            HandlerName = context.OperatorName,
            ApproveStatus = (int)ApproveStatus.Pass,
            ApproveDesc = "发起流程",
            ApproveTime = DateTime.Now,
            EntryTime = context.Instance.StartTime
        };
        await context.Db.Insertable(record).ExecuteCommandAsync();

        _logger.LogInformation("审批记录创建成功: RecordId={RecordId}, HandlerId={HandlerId}, HandlerName={HandlerName}",
            record.Id, record.HandlerId, record.HandlerName);
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetNextNodesAsync(NodeHandlerContext context)
    {
        // 根据边找到下一节点
        var edges = context.DagConfig.Edges
            .Where(e => e.SourceNodeId == context.DagNode.Id)
            .ToList();

        _logger.LogInformation("获取下一节点: SourceNodeId={SourceNodeId}, NextNodeIds={NextNodeIds}",
            context.DagNode.Id, string.Join(",", edges.Select(e => e.TargetNodeId)));

        return edges.Select(e => e.TargetNodeId).ToList();
    }
}