using EasyWeChatModels.Dto.AntWorkflow;

namespace BusinessManager.Buz.IService;

/// <summary>
/// Ant流程版本服务接口
/// </summary>
public interface IAntWorkflowVersionService
{
    /// <summary>
    /// 获取流程的版本列表
    /// </summary>
    /// <param name="workflowId">流程ID</param>
    /// <returns>版本列表，按发布时间倒序</returns>
    Task<List<AntWorkflowVersionDto>> GetListByWorkflowIdAsync(Guid workflowId);

    /// <summary>
    /// 根据ID获取版本详情
    /// </summary>
    /// <param name="id">版本ID</param>
    /// <returns>版本DTO，不存在返回null</returns>
    Task<AntWorkflowVersionDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 获取流程的最新版本
    /// </summary>
    /// <param name="workflowId">流程ID</param>
    /// <returns>最新版本DTO，不存在返回null</returns>
    Task<AntWorkflowVersionDto?> GetLatestVersionAsync(Guid workflowId);
}