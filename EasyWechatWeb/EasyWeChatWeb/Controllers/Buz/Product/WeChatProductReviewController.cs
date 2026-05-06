using BusinessManager.Buz.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.WeChatPro;

/// <summary>
/// 微信商品评价控制器
/// </summary>
[ApiController]
[Route("api/wechat/product")]
public class WeChatProductReviewController : BaseController
{
    /// <summary>
    /// 商品评价服务
    /// </summary>
    public IWeChatProductReviewService _reviewService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatProductReviewController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取商品评价列表
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <param name="query">查询参数</param>
    /// <returns>评价分页列表</returns>
    [HttpGet("{id}/reviews")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<WxProductReviewDto>>), 200)]
    public async Task<ApiResponse<PageResponse<WxProductReviewDto>>> GetReviews(
        Guid id, [FromQuery] QueryWxReviewDto query)
    {
        try
        {
            var result = await _reviewService.GetProductReviewsAsync(id, query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取商品评价失败: {ProductId}", id);
            return Error<PageResponse<WxProductReviewDto>>("获取商品评价失败");
        }
    }

    /// <summary>
    /// 获取商品评价统计
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <returns>评价统计信息</returns>
    [HttpGet("{id}/review-stats")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<WxReviewSummaryDto>), 200)]
    public async Task<ApiResponse<WxReviewSummaryDto>> GetReviewStats(Guid id)
    {
        try
        {
            var result = await _reviewService.GetReviewSummaryAsync(id);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取评价统计失败: {ProductId}", id);
            return Error<WxReviewSummaryDto>("获取评价统计失败");
        }
    }

    /// <summary>
    /// 提交商品评价
    /// </summary>
    /// <param name="dto">评价内容</param>
    /// <returns>评价ID</returns>
    [HttpPost("review")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> SubmitReview([FromBody] SubmitReviewDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<Guid>("请先登录", 401);
            }

            var result = await _reviewService.SubmitReviewAsync(userId, dto);
            return Success(result, "评价提交成功");
        }
        catch (BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "提交评价失败");
            return Error<Guid>("提交评价失败");
        }
    }
}