using EasyWeChatModels.Dto.AntWorkflow;
using CommonManager.Base;

namespace BusinessManager.Buz.IService;

/// <summary>
/// Ant流程运行时服务接口
/// </summary>
public interface IAntWorkflowRuntimeService
{
    /// <summary>
    /// 发起流程
    /// </summary>
    /// <param name="dto">发起参数</param>
    /// <param name="initiatorId">发起人ID</param>
    /// <param name="initiatorName">发起人姓名</param>
    /// <returns>实例ID</returns>
    Task<Guid> StartAsync(StartAntWorkflowDto dto, Guid initiatorId, string initiatorName);

    /// <summary>
    /// 获取我发起的流程实例列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <param name="userId">用户ID</param>
    /// <returns>分页结果</returns>
    Task<PageResponse<AntWorkflowInstanceDto>> GetMyInstancesAsync(QueryMyInstanceDto query, Guid userId);

    /// <summary>
    /// 获取实例详情
    /// </summary>
    /// <param name="instanceId">实例ID</param>
    /// <returns>实例详情，不存在返回null</returns>
    Task<AntWorkflowInstanceDetailDto?> GetInstanceDetailAsync(Guid instanceId);

    /// <summary>
    /// 取消/撤回流程实例
    /// </summary>
    /// <param name="instanceId">实例ID</param>
    /// <param name="userId">用户ID</param>
    /// <param name="reason">撤回原因</param>
    /// <returns>影响行数</returns>
    Task<int> CancelAsync(Guid instanceId, Guid userId, string? reason);

    /// <summary>
    /// 获取执行日志（审批记录）
    /// </summary>
    /// <param name="instanceId">实例ID</param>
    /// <returns>审批记录列表</returns>
    Task<List<AntExecutionLogDto>> GetLogsAsync(Guid instanceId);
}