using Newtonsoft.Json;
using EasyWeChatModels.Enums;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models.NodeConfigs;
using EasyWeChatModels.Models.AntWorkflow;
using BusinessManager.Buz.IService;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 抄送节点处理器服务
/// </summary>
public class CopyerNodeService : INodeHandlerService
{
    public AntNodeType NodeType => AntNodeType.Copyer;

    /// <inheritdoc/>
    public async Task HandleEnterAsync(NodeHandlerContext context)
    {
        // 解析抄送节点配置
        var config = JsonConvert.DeserializeObject<CopyerNodeConfig>(context.DagNode.Config?.ToString() ?? "");
        if (config == null || config.NodeUserList == null || config.NodeUserList.Count == 0)
        {
            // 没有抄送人，直接标记完成
            context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
            return;
        }

        // 创建抄送记录
        foreach (var user in config.NodeUserList)
        {
            var ccRecord = new AntWorkflowCCRecord
            {
                Id = Guid.NewGuid(),
                InstanceId = context.Instance.Id,
                NodeId = context.DagNode.Id,
                NodeName = context.DagNode.Name,
                FromUserId = context.OperatorId,
                ToUserId = user.TargetId,
                SendTime = DateTime.Now,
                IsRead = 0
            };
            await context.Db.Insertable(ccRecord).ExecuteCommandAsync();
        }

        // 抄送节点自动完成，不阻塞流程
        context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
        await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task HandleCompleteAsync(NodeHandlerContext context)
    {
        // 抄送节点无需额外处理
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