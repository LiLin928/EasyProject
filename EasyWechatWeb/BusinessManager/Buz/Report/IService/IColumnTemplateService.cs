using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 列配置模板服务接口
/// </summary>
public interface IColumnTemplateService
{
    /// <summary>
    /// 获取列模板列表（分页）
    /// </summary>
    Task<PageResponse<ColumnTemplateDto>> GetPageListAsync(QueryColumnTemplateDto query);

    /// <summary>
    /// 获取列模板详情
    /// </summary>
    Task<ColumnTemplateDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 获取单列模板列表
    /// </summary>
    Task<List<ColumnTemplateDto>> GetSingleTemplatesAsync();

    /// <summary>
    /// 添加列模板
    /// </summary>
    Task<Guid> AddAsync(CreateColumnTemplateDto dto, Guid creatorId);

    /// <summary>
    /// 更新列模板
    /// </summary>
    Task<int> UpdateAsync(UpdateColumnTemplateDto dto);

    /// <summary>
    /// 删除列模板
    /// </summary>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 从SQL获取列信息
    /// </summary>
    Task<List<DetectedColumnDto>> FetchColumnsAsync(FetchColumnsDto dto);
}