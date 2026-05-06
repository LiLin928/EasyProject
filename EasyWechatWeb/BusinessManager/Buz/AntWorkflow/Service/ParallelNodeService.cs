using Newtonsoft.Json;
using EasyWeChatModels.Enums;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models.NodeConfigs;
using EasyWeChatModels.Models.AntWorkflow;
using BusinessManager.Buz.IService;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 并行分支节点处理器服务
/// </summary>
public class ParallelNodeService : INodeHandlerService
{
    public AntNodeType NodeType => AntNodeType.Parallel;

    /// <inheritdoc/>
    public async Task HandleEnterAsync(NodeHandlerContext context)
    {
        // 解析并行节点配置
        var config = JsonConvert.DeserializeObject<ParallelNodeConfig>(context.DagNode.Config?.ToString() ?? "");
        if (config == null || config.ParallelNodes == null || config.ParallelNodes.Count == 0)
        {
            // 没有并行分支配置，自动完成
            context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
            return;
        }

        // 并行分支节点立即完成，推进到所有分支
        context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
        await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task HandleCompleteAsync(NodeHandlerContext context)
    {
        // 检查所有并行分支是否完成
        var config = JsonConvert.DeserializeObject<ParallelNodeConfig>(context.DagNode.Config?.ToString() ?? "");
        if (config == null)
        {
            context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
            return;
        }

        // 获取所有分支节点的状态
        var dagNodeId = context.DagNode.Id;
        var branchNodes = await context.Db.Queryable<AntWorkflowInstanceNode>()
            .Where(n => n.InstanceId == context.Instance.Id && n.ParentNodeId == dagNodeId)
            .ToListAsync();

        var completedCount = branchNodes.Count(n => n.ApproveStatus == (int)NodeApproveStatus.Completed);
        var totalBranches = config.ParallelNodes?.Count ?? branchNodes.Count;

        bool isComplete = false;

        switch (config.CompleteCondition?.ToLower())
        {
            case "all":
                // 所有分支都完成
                isComplete = completedCount >= totalBranches;
                break;

            case "any":
                // 任一分支完成
                isComplete = completedCount >= 1;
                break;

            case "count":
                // 达到指定数量完成
                var targetCount = config.CompleteCount ?? 1;
                isComplete = completedCount >= targetCount;
                break;

            default:
                // 默认所有分支完成
                isComplete = completedCount >= totalBranches;
                break;
        }

        if (isComplete)
        {
            context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
        }
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetNextNodesAsync(NodeHandlerContext context)
    {
        // 并行分支：返回所有并行分支的下一节点
        var config = JsonConvert.DeserializeObject<ParallelNodeConfig>(context.DagNode.Config?.ToString() ?? "");

        if (config == null || config.ParallelNodes == null || config.ParallelNodes.Count == 0)
        {
            // 没有并行配置，按普通边处理
            return context.DagConfig.Edges
                .Where(e => e.SourceNodeId == context.DagNode.Id)
                .Select(e => e.TargetNodeId)
                .ToList();
        }

        // 查找每个分支对应的边
        var edges = context.DagConfig.Edges
            .Where(e => e.SourceNodeId == context.DagNode.Id)
            .ToList();

        var nextNodes = new List<string>();

        foreach (var branch in config.ParallelNodes)
        {
            // 根据分支ID找对应的边（通过 SourcePort 或 Condition）
            var branchEdges = edges.Where(e =>
                e.SourcePort == branch.Id ||
                (e.Condition != null && e.Condition.BranchId == branch.Id)
            ).ToList();

            if (branchEdges.Count > 0)
            {
                nextNodes.AddRange(branchEdges.Select(e => e.TargetNodeId));
            }
        }

        // 如果没有通过分支匹配找到，返回所有边指向的节点
        if (nextNodes.Count == 0)
        {
            nextNodes = edges.Select(e => e.TargetNodeId).ToList();
        }

        return nextNodes;
    }
}