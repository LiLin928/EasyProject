using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Etl.IService;

/// <summary>
/// ETL任务流服务接口
/// </summary>
public interface IEtlPipelineService
{
    /// <summary>
    /// 获取任务流分页列表
    /// </summary>
    Task<PageResponse<EtlPipelineDto>> GetPageListAsync(QueryEtlPipelineDto query);

    /// <summary>
    /// 获取任务流详情
    /// </summary>
    Task<EtlPipelineDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建任务流
    /// </summary>
    Task<Guid> AddAsync(CreateEtlPipelineDto dto, Guid creatorId, string? creatorName);

    /// <summary>
    /// 更新任务流
    /// </summary>
    Task<int> UpdateAsync(UpdateEtlPipelineDto dto);

    /// <summary>
    /// 删除任务流
    /// </summary>
    Task<int> DeleteAsync(List<Guid> ids);

    /// <summary>
    /// 发布任务流
    /// </summary>
    Task<int> PublishAsync(Guid id);

    /// <summary>
    /// 取消发布
    /// </summary>
    Task<int> UnpublishAsync(Guid id);

    /// <summary>
    /// 复制任务流
    /// </summary>
    Task<Guid> CopyAsync(Guid id, string name);

    /// <summary>
    /// 执行任务流
    /// </summary>
    Task<Guid> ExecuteAsync(Guid id, string? paramsJson, Guid? triggerUserId, string? triggerUserName);

    /// <summary>
    /// 获取DAG定义
    /// </summary>
    Task<string?> GetDagConfigAsync(Guid id);

    /// <summary>
    /// 保存DAG定义
    /// </summary>
    Task<int> SaveDagConfigAsync(Guid id, string dagConfig);
}