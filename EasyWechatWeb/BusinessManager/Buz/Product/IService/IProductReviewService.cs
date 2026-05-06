using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 商品评价服务接口
/// </summary>
/// <remarks>
/// 提供商品评价管理、审核、回复等功能
/// </remarks>
public interface IProductReviewService
{
    /// <summary>
    /// 获取评价分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页评价列表</returns>
    Task<PageResponse<ProductReviewDto>> GetPageListAsync(QueryProductReviewDto query);

    /// <summary>
    /// 根据ID获取评价详情
    /// </summary>
    /// <param name="id">评价ID</param>
    /// <returns>评价信息</returns>
    Task<ProductReviewDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 回复评价
    /// </summary>
    /// <param name="dto">回复参数</param>
    /// <returns>影响的行数</returns>
    Task<int> ReplyAsync(ReplyReviewDto dto);

    /// <summary>
    /// 审核评价
    /// </summary>
    /// <param name="id">评价ID</param>
    /// <param name="status">状态（approved/rejected）</param>
    /// <returns>影响的行数</returns>
    Task<int> AuditAsync(Guid id, string status);

    /// <summary>
    /// 隐藏评价
    /// </summary>
    /// <param name="id">评价ID</param>
    /// <returns>影响的行数</returns>
    Task<int> HideAsync(Guid id);

    /// <summary>
    /// 获取评价统计
    /// </summary>
    /// <param name="productId">商品ID（可选，不传则统计全部）</param>
    /// <returns>评价统计信息</returns>
    Task<ReviewStatisticsDto> GetStatisticsAsync(Guid? productId);

    /// <summary>
    /// 删除评价
    /// </summary>
    /// <param name="id">评价ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 批量审核评价
    /// </summary>
    /// <param name="ids">评价ID列表</param>
    /// <param name="status">状态（approved/rejected）</param>
    /// <returns>影响的行数</returns>
    Task<int> BatchAuditAsync(List<Guid> ids, string status);
}