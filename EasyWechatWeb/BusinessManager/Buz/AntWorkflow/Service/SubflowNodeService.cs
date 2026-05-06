using Newtonsoft.Json;
using SqlSugar;
using EasyWeChatModels.Enums;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models;
using EasyWeChatModels.Models.NodeConfigs;
using EasyWeChatModels.Models.AntWorkflow;
using BusinessManager.Buz.IService;
using AntWorkflowEntity = EasyWeChatModels.Entitys.AntWorkflow;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 子流程节点处理器服务
/// </summary>
/// <remarks>
/// 子流程节点用于嵌套调用另一个流程，子流程完成后继续父流程
/// </remarks>
public class SubflowNodeService : INodeHandlerService
{
    public AntNodeType NodeType => AntNodeType.Subflow;

    /// <inheritdoc/>
    public async Task HandleEnterAsync(NodeHandlerContext context)
    {
        // 解析子流程配置
        var config = JsonConvert.DeserializeObject<SubflowNodeConfig>(context.DagNode.Config?.ToString() ?? "");
        if (config == null || config.SubflowId == Guid.Empty)
        {
            // 没有配置子流程，自动完成
            context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
            return;
        }

        // 查询子流程定义
        var subflowDef = await context.Db.Queryable<AntWorkflowEntity>()
            .Where(w => w.Id == config.SubflowId && w.Status == (int)WorkflowStatus.Published)
            .FirstAsync();

        if (subflowDef == null)
        {
            // 子流程不存在或未发布，自动完成
            context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
            return;
        }

        // 创建子流程实例
        var subflowInstance = new AntWorkflowInstance
        {
            Id = Guid.NewGuid(),
            WorkflowId = config.SubflowId,
            Title = $"[子流程] {context.Instance.Title}",
            BusinessType = context.Instance.BusinessType,
            BusinessId = context.Instance.BusinessId,
            BusinessData = context.Instance.BusinessData,
            FormData = config.PassFormData ? context.Instance.FormData : null,
            Status = (int)InstanceStatus.Approving,
            FlowConfig = subflowDef.FlowConfig,
            InitiatorId = context.Instance.InitiatorId,
            StartTime = DateTime.Now,
            CreateTime = DateTime.Now
        };

        // 记录父流程关联
        subflowInstance.ParentInstanceId = context.Instance.Id;
        subflowInstance.ParentNodeId = context.DagNode.Id;

        await context.Db.Insertable(subflowInstance).ExecuteCommandAsync();

        // 初始化子流程节点状态
        var dagConfig = JsonConvert.DeserializeObject<DagConfig>(subflowDef.FlowConfig ?? "");
        if (dagConfig != null)
        {
            foreach (var node in dagConfig.Nodes)
            {
                var subflowNode = new AntWorkflowInstanceNode
                {
                    Id = Guid.NewGuid(),
                    InstanceId = subflowInstance.Id,
                    NodeId = node.Id,
                    NodeName = node.Name,
                    NodeType = (int)node.Type,
                    NodeConfig = node.Config?.ToString(),
                    ApproveStatus = (int)NodeApproveStatus.Pending
                };
                await context.Db.Insertable(subflowNode).ExecuteCommandAsync();
            }

            // 处理子流程的开始节点（简化：直接标记完成并推进）
            var startNode = dagConfig.Nodes.FirstOrDefault(n => n.Type == AntNodeType.Start);
            if (startNode != null)
            {
                var startInstanceNode = await context.Db.Queryable<AntWorkflowInstanceNode>()
                    .Where(n => n.InstanceId == subflowInstance.Id && n.NodeId == startNode.Id)
                    .FirstAsync();

                if (startInstanceNode != null)
                {
                    // 标记开始节点完成
                    startInstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
                    await context.Db.Updateable(startInstanceNode).ExecuteCommandAsync();

                    // 创建发起人审批记录
                    var startRecord = new AntWorkflowApproveRecord
                    {
                        Id = Guid.NewGuid(),
                        InstanceId = subflowInstance.Id,
                        NodeId = startNode.Id,
                        NodeName = startNode.Name,
                        NodeType = (int)AntNodeType.Start,
                        HandlerId = context.Instance.InitiatorId ?? Guid.Empty,
                        HandlerName = context.OperatorName ?? "发起人",
                        ApproveStatus = (int)ApproveStatus.Pass,
                        ApproveDesc = "发起流程",
                        ApproveTime = DateTime.Now,
                        EntryTime = DateTime.Now,
                        Duration = 0
                    };
                    await context.Db.Insertable(startRecord).ExecuteCommandAsync();

                    // 推进到开始节点的下一节点
                    var startEdges = dagConfig.Edges.Where(e => e.SourceNodeId == startNode.Id).ToList();
                    foreach (var edge in startEdges)
                    {
                        var nextNode = dagConfig.Nodes.FirstOrDefault(n => n.Id == edge.TargetNodeId);
                        if (nextNode != null)
                        {
                            var nextInstanceNode = await context.Db.Queryable<AntWorkflowInstanceNode>()
                                .Where(n => n.InstanceId == subflowInstance.Id && n.NodeId == nextNode.Id)
                                .FirstAsync();

                            if (nextInstanceNode != null)
                            {
                                nextInstanceNode.ApproveStatus = (int)NodeApproveStatus.Processing;
                                await context.Db.Updateable(nextInstanceNode).ExecuteCommandAsync();
                            }
                        }
                    }
                }
            }
        }

        // 创建子流程待办任务（父流程等待）
        var waitTask = new AntWorkflowCurrentTask
        {
            Id = Guid.NewGuid(),
            InstanceId = context.Instance.Id,
            InstanceNodeId = context.InstanceNode.Id,
            NodeId = context.DagNode.Id,
            NodeType = (int)AntNodeType.Subflow,
            HandlerId = Guid.Empty, // 系统等待
            HandlerType = 0,
            EntryTime = DateTime.Now,
            TaskType = 1,
            ActiveStatus = 1,
            SourceNodeId = subflowInstance.Id.ToString() // 关联子流程实例
        };

        await context.Db.Insertable(waitTask).ExecuteCommandAsync();

        // 父流程节点状态保持处理中，等待子流程完成
        context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Processing;
        await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task HandleCompleteAsync(NodeHandlerContext context)
    {
        // 子流程完成时处理
        var task = await context.Db.Queryable<AntWorkflowCurrentTask>()
            .Where(t => t.InstanceId == context.Instance.Id && t.NodeId == context.DagNode.Id && t.ActiveStatus == 1)
            .FirstAsync();

        if (task != null)
        {
            // 删除等待任务
            await context.Db.Deleteable(task).ExecuteCommandAsync();
        }

        // 更新节点状态为已完成
        context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
        await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetNextNodesAsync(NodeHandlerContext context)
    {
        // 只有节点完成时才推进
        if (context.InstanceNode.ApproveStatus != (int)NodeApproveStatus.Completed)
        {
            return new List<string>();
        }

        var edges = context.DagConfig.Edges
            .Where(e => e.SourceNodeId == context.DagNode.Id)
            .ToList();

        return edges.Select(e => e.TargetNodeId).ToList();
    }
}