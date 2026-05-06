using EasyWeChatModels.Dto.AntWorkflow;
using EasyWeChatModels.Entitys;
using CommonManager.Base;
using AntWorkflowEntity = EasyWeChatModels.Entitys.AntWorkflow;

namespace BusinessManager.Buz.IService;

/// <summary>
/// Ant流程服务接口
/// </summary>
public interface IAntWorkflowService
{
    /// <summary>
    /// 分页查询流程列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页结果</returns>
    Task<PageResponse<AntWorkflowDto>> GetPageListAsync(QueryAntWorkflowDto query);

    /// <summary>
    /// 根据ID获取流程详情
    /// </summary>
    /// <param name="id">流程ID</param>
    /// <returns>流程DTO，不存在返回null</returns>
    Task<AntWorkflowDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建流程（草稿状态）
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <param name="creatorId">创建人ID</param>
    /// <returns>新流程ID</returns>
    Task<Guid> CreateAsync(CreateAntWorkflowDto dto, Guid creatorId);

    /// <summary>
    /// 更新流程
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响行数</returns>
    Task<int> UpdateAsync(UpdateAntWorkflowDto dto);

    /// <summary>
    /// 删除流程（仅草稿状态可删除）
    /// </summary>
    /// <param name="id">流程ID</param>
    /// <returns>影响行数</returns>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 发布流程（直接发布，跳过审核）
    /// </summary>
    /// <param name="id">流程ID</param>
    /// <param name="publisherId">发布人ID</param>
    /// <param name="publisherName">发布人姓名</param>
    /// <returns>影响行数</returns>
    Task<int> PublishAsync(Guid id, Guid publisherId, string publisherName);

    /// <summary>
    /// 停用流程
    /// </summary>
    /// <param name="id">流程ID</param>
    /// <returns>影响行数</returns>
    Task<int> DisableAsync(Guid id);

    /// <summary>
    /// 启用流程
    /// </summary>
    /// <param name="id">流程ID</param>
    /// <returns>影响行数</returns>
    Task<int> EnableAsync(Guid id);

    /// <summary>
    /// 复制流程
    /// </summary>
    /// <param name="dto">复制参数</param>
    /// <param name="creatorId">创建人ID</param>
    /// <returns>新流程ID</returns>
    Task<Guid> CopyAsync(CopyAntWorkflowDto dto, Guid creatorId);

    /// <summary>
    /// 根据编码获取流程
    /// </summary>
    /// <param name="code">流程编码</param>
    /// <returns>流程实体，不存在返回null</returns>
    Task<AntWorkflowEntity?> GetByCodeAsync(string code);

    /// <summary>
    /// 获取已发布的流程列表
    /// </summary>
    /// <param name="categoryCode">分类编码（可选）</param>
    /// <returns>流程列表</returns>
    Task<List<AntWorkflowDto>> GetPublishedListAsync(string? categoryCode = null);
}