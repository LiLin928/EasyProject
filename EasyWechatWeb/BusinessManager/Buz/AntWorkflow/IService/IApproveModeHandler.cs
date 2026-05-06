// BusinessManager/Buz/AntWorkflow/IService/IApproveModeHandler.cs
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Enums;
using EasyWeChatModels.Models.NodeConfigs;
using EasyWeChatModels.Models.AntWorkflow;

namespace BusinessManager.Buz.AntWorkflow.IService;

/// <summary>
/// 审批模式处理器接口
/// </summary>
public interface IApproveModeHandler
{
    /// <summary>
    /// 创建审批任务（按模式）
    /// </summary>
    Task CreateTasksAsync(NodeHandlerContext context, ApproverNodeConfig config, List<NodeUser> handlers);

    /// <summary>
    /// 判断审批后是否应推进到下一节点
    /// </summary>
    Task<bool> ShouldAdvanceAsync(NodeHandlerContext context, ApproverNodeConfig config);

    /// <summary>
    /// 处理审批通过（按模式）
    /// </summary>
    Task HandleApprovePassAsync(NodeHandlerContext context, ApproverNodeConfig config);
}