using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 轮播图服务接口
/// </summary>
/// <remarks>
/// 提供轮播图管理、排序管理等功能
/// </remarks>
public interface IBannerService
{
    /// <summary>
    /// 获取轮播图分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页轮播图列表</returns>
    Task<PageResponse<BannerDto>> GetPageListAsync(QueryBannerDto query);

    /// <summary>
    /// 获取所有启用的轮播图列表（用于前端展示）
    /// </summary>
    /// <returns>轮播图列表</returns>
    Task<List<BannerDto>> GetActiveListAsync();

    /// <summary>
    /// 根据ID获取轮播图详情
    /// </summary>
    /// <param name="id">轮播图ID</param>
    /// <returns>轮播图信息</returns>
    Task<BannerDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 添加轮播图
    /// </summary>
    /// <param name="dto">添加参数</param>
    /// <returns>新创建的轮播图ID</returns>
    Task<Guid> AddAsync(AddBannerDto dto);

    /// <summary>
    /// 更新轮播图
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateAsync(UpdateBannerDto dto);

    /// <summary>
    /// 删除轮播图
    /// </summary>
    /// <param name="id">轮播图ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 更新轮播图状态
    /// </summary>
    /// <param name="id">轮播图ID</param>
    /// <param name="status">状态值</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateStatusAsync(Guid id, int status);

    /// <summary>
    /// 批量更新排序
    /// </summary>
    /// <param name="dto">排序参数</param>
    /// <returns>影响的行数</returns>
    Task<int> BatchSortAsync(SortBannerDto dto);
}