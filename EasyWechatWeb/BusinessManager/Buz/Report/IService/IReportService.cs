using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 报表服务接口
/// </summary>
public interface IReportService
{
    /// <summary>
    /// 获取报表列表（分页）
    /// </summary>
    Task<PageResponse<ReportDto>> GetPageListAsync(QueryReportDto query);

    /// <summary>
    /// 获取报表详情
    /// </summary>
    Task<ReportDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 添加报表
    /// </summary>
    Task<Guid> AddAsync(CreateReportDto dto, Guid creatorId);

    /// <summary>
    /// 更新报表
    /// </summary>
    Task<int> UpdateAsync(UpdateReportDto dto);

    /// <summary>
    /// 删除报表
    /// </summary>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 预览报表
    /// </summary>
    Task<PreviewResultDto> PreviewAsync(PreviewReportDto dto);

    /// <summary>
    /// 执行报表查询
    /// </summary>
    Task<PreviewResultDto> ExecuteAsync(Guid id);

    /// <summary>
    /// 获取报表分类列表
    /// </summary>
    Task<List<ReportCategoryDto>> GetCategoriesAsync();

    /// <summary>
    /// 获取发布报表数据（公开访问）
    /// </summary>
    Task<PublishReportDto?> GetPublishDataAsync(Guid id);
}