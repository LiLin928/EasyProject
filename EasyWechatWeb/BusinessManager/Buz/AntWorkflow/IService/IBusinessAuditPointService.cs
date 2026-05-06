using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using CommonManager.Base;

namespace BusinessManager.Buz.AntWorkflow.IService;

/// <summary>
/// 业务审核点服务接口
/// </summary>
public interface IBusinessAuditPointService
{
    /// <summary>
    /// 分页查询审核点列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页结果</returns>
    Task<PageResponse<BusinessAuditPointDto>> GetPageListAsync(QueryBusinessAuditPointDto query);

    /// <summary>
    /// 根据ID获取审核点详情
    /// </summary>
    /// <param name="id">审核点ID</param>
    /// <returns>审核点DTO，不存在返回null</returns>
    Task<BusinessAuditPointDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 添加审核点
    /// </summary>
    /// <param name="dto">新增审核点DTO</param>
    /// <param name="creatorId">创建人ID</param>
    /// <returns>新增审核点ID</returns>
    Task<Guid> AddAsync(AddBusinessAuditPointDto dto, Guid creatorId);

    /// <summary>
    /// 更新审核点
    /// </summary>
    /// <param name="dto">更新审核点DTO</param>
    /// <returns>影响行数</returns>
    Task<int> UpdateAsync(UpdateBusinessAuditPointDto dto);

    /// <summary>
    /// 删除审核点
    /// </summary>
    /// <param name="id">审核点ID</param>
    /// <returns>影响行数</returns>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 根据编码获取审核点
    /// </summary>
    /// <param name="code">审核点编码</param>
    /// <returns>审核点DTO，不存在返回null</returns>
    Task<BusinessAuditPointDto?> GetByCodeAsync(string code);

    /// <summary>
    /// 根据处理表名获取审核点列表
    /// </summary>
    /// <param name="tableName">处理表名</param>
    /// <returns>审核点列表</returns>
    Task<List<BusinessAuditPointDto>> GetByTableNameAsync(string tableName);
}