using SqlSugar;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Enums;
using EasyWeChatModels.Models.NodeConfigs;
using EasyWeChatModels.Models.AntWorkflow;
using BusinessManager.Buz.AntWorkflow.IService;
using Microsoft.Extensions.Logging;

namespace BusinessManager.Buz.AntWorkflow.Service;

/// <summary>
/// 审批模式处理器实现
/// </summary>
public class ApproveModeHandler : IApproveModeHandler
{
    /// <summary>数据库客户端（Autofac 属性注入）</summary>
    public ISqlSugarClient _db { get; set; } = null!;

    /// <summary>日志服务（Autofac 属性注入）</summary>
    public ILogger<ApproveModeHandler> _logger { get; set; } = null!;

    /// <inheritdoc/>
    public async Task CreateTasksAsync(NodeHandlerContext context, ApproverNodeConfig config, List<NodeUser> handlers)
    {
        switch (config.ExamineMode)
        {
            case ExamineMode.Sequential:
                await CreateSequentialTasksAsync(context, handlers);
                break;

            case ExamineMode.Or:
                await CreateOrTasksAsync(context, handlers);
                break;

            default:
                await CreateOrTasksAsync(context, handlers);
                break;
        }
    }

    private async Task CreateSequentialTasksAsync(NodeHandlerContext context, List<NodeUser> handlers)
    {
        _logger.LogInformation("创建依次审批任务: HandlersCount={Count}", handlers.Count);

        for (int i = 0; i < handlers.Count; i++)
        {
            var handler = handlers[i];
            var task = new AntWorkflowCurrentTask
            {
                Id = Guid.NewGuid(),
                InstanceId = context.Instance.Id,
                InstanceNodeId = context.InstanceNode.Id,
                NodeId = context.DagNode.Id,
                NodeType = (int)AntNodeType.Approver,
                HandlerId = handler.TargetId,
                HandlerType = handler.Type,
                EntryTime = DateTime.Now,
                TaskType = 1,
                ActiveStatus = i == 0 ? 1 : 0,
                NodeOrder = i + 1
            };
            await _db.Insertable(task).ExecuteCommandAsync();
        }
    }

    private async Task CreateOrTasksAsync(NodeHandlerContext context, List<NodeUser> handlers)
    {
        foreach (var handler in handlers)
        {
            var task = new AntWorkflowCurrentTask
            {
                Id = Guid.NewGuid(),
                InstanceId = context.Instance.Id,
                InstanceNodeId = context.InstanceNode.Id,
                NodeId = context.DagNode.Id,
                NodeType = (int)AntNodeType.Approver,
                HandlerId = handler.TargetId,
                HandlerType = handler.Type,
                EntryTime = DateTime.Now,
                TaskType = 1,
                ActiveStatus = 1,
                NodeOrder = 1
            };
            await _db.Insertable(task).ExecuteCommandAsync();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ShouldAdvanceAsync(NodeHandlerContext context, ApproverNodeConfig config)
    {
        switch (config.ExamineMode)
        {
            case ExamineMode.Sequential:
                return await CheckSequentialAdvanceAsync(context);
            case ExamineMode.Or:
                return await CheckOrAdvanceAsync(context);
            default:
                return await CheckOrAdvanceAsync(context);
        }
    }

    private async Task<bool> CheckSequentialAdvanceAsync(NodeHandlerContext context)
    {
        var tasks = await _db.Queryable<AntWorkflowCurrentTask>()
            .Where(t => t.InstanceId == context.Instance.Id && t.NodeId == context.DagNode.Id)
            .OrderBy(t => t.NodeOrder)
            .ToListAsync();

        var activeTask = tasks.FirstOrDefault(t => t.ActiveStatus == 1);
        if (activeTask == null) return true;

        var approveRecord = await _db.Queryable<AntWorkflowApproveRecord>()
            .Where(r => r.InstanceId == context.Instance.Id &&
                        r.NodeId == context.DagNode.Id &&
                        r.HandlerId == activeTask.HandlerId &&
                        r.ApproveStatus == (int)ApproveStatus.Pass)
            .FirstAsync();

        if (approveRecord == null) return false;

        var nextTask = tasks.FirstOrDefault(t => t.NodeOrder > activeTask.NodeOrder && t.ActiveStatus == 0);
        if (nextTask != null)
        {
            nextTask.ActiveStatus = 1;
            activeTask.ActiveStatus = 0;
            await _db.Updateable(activeTask).ExecuteCommandAsync();
            await _db.Updateable(nextTask).ExecuteCommandAsync();
            return false;
        }

        return true;
    }

    private async Task<bool> CheckOrAdvanceAsync(NodeHandlerContext context)
    {
        var approveRecord = await _db.Queryable<AntWorkflowApproveRecord>()
            .Where(r => r.InstanceId == context.Instance.Id &&
                        r.NodeId == context.DagNode.Id &&
                        r.ApproveStatus == (int)ApproveStatus.Pass)
            .FirstAsync();

        return approveRecord != null;
    }

    /// <inheritdoc/>
    public async Task HandleApprovePassAsync(NodeHandlerContext context, ApproverNodeConfig config)
    {
        if (config.ExamineMode == ExamineMode.Or)
        {
            await _db.Deleteable<AntWorkflowCurrentTask>()
                .Where(t => t.InstanceId == context.Instance.Id &&
                            t.NodeId == context.DagNode.Id &&
                            t.ActiveStatus == 1)
                .ExecuteCommandAsync();

            _logger.LogInformation("或签通过，删除其他待办任务");
        }
    }
}