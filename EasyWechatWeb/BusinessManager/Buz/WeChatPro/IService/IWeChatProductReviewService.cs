using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 微信商品评价服务接口
/// </summary>
public interface IWeChatProductReviewService
{
    /// <summary>
    /// 获取商品评价列表
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <param name="query">查询参数</param>
    /// <returns>评价分页列表</returns>
    Task<PageResponse<WxProductReviewDto>> GetProductReviewsAsync(Guid productId, QueryWxReviewDto query);

    /// <summary>
    /// 获取商品评价统计
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>评价统计信息</returns>
    Task<WxReviewSummaryDto> GetReviewSummaryAsync(Guid productId);

    /// <summary>
    /// 提交订单评价
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="dto">评价内容</param>
    /// <returns>评价ID</returns>
    Task<Guid> SubmitReviewAsync(Guid userId, SubmitReviewDto dto);
}