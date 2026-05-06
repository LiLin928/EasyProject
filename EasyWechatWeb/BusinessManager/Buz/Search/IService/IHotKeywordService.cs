using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 热门关键词服务接口
/// </summary>
/// <remarks>
/// 提供热门关键词管理功能
/// </remarks>
public interface IHotKeywordService
{
    /// <summary>
    /// 获取分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页列表</returns>
    Task<PageResponse<HotKeywordDto>> GetPageListAsync(QueryHotKeywordDto query);

    /// <summary>
    /// 根据ID获取详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    Task<HotKeywordDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的ID</returns>
    Task<Guid> CreateAsync(AddHotKeywordDto dto);

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateAsync(UpdateHotKeywordDto dto);

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 更新状态
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="status">状态</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateStatusAsync(Guid id, int status);

    /// <summary>
    /// 获取推荐关键词列表
    /// </summary>
    /// <param name="limit">数量限制</param>
    /// <returns>关键词列表</returns>
    Task<List<HotKeywordDto>> GetRecommendListAsync(int limit = 10);

    /// <summary>
    /// 获取统计
    /// </summary>
    /// <returns>统计信息</returns>
    Task<HotKeywordStatisticsDto> GetStatisticsAsync();
}