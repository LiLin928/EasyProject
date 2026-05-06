using EasyWeChatModels.Models.NodeConfigs;
using EasyWeChatModels.Models.AntWorkflow;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 审批人获取服务接口
/// </summary>
public interface IApproverResolverService
{
    /// <summary>
    /// 根据审批人设置类型获取审批人列表
    /// </summary>
    Task<List<NodeUser>> GetApproversAsync(NodeHandlerContext context, ApproverNodeConfig config);

    /// <summary>
    /// 获取用户直接主管
    /// </summary>
    Task<NodeUser?> GetSupervisorAsync(Guid userId);

    /// <summary>
    /// 获取用户多级主管
    /// </summary>
    Task<List<NodeUser>> GetMultiSupervisorAsync(Guid userId, int level);

    /// <summary>
    /// 获取系统管理员（用于转交）
    /// </summary>
    Task<NodeUser?> GetAdminAsync();
}