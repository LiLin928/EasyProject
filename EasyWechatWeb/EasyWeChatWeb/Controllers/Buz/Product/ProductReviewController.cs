using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 商品评价控制器
/// </summary>
/// <remarks>
/// 提供评价管理、审核、回复等接口
/// </remarks>
[ApiController]
[Route("api/product/review")]
[Authorize]
public class ProductReviewController : BaseController
{
    /// <summary>
    /// 商品评价服务接口
    /// </summary>
    public IProductReviewService _productReviewService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<ProductReviewController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取评价列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页评价列表</returns>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<ProductReviewDto>>), 200)]
    public async Task<ApiResponse<PageResponse<ProductReviewDto>>> GetList([FromBody] QueryProductReviewDto query)
    {
        try
        {
            var result = await _productReviewService.GetPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取评价列表失败");
            return Error<PageResponse<ProductReviewDto>>("获取评价列表失败");
        }
    }

    /// <summary>
    /// 获取评价详情
    /// </summary>
    /// <param name="id">评价ID</param>
    /// <returns>评价详情</returns>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProductReviewDto>), 200)]
    public async Task<ApiResponse<ProductReviewDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _productReviewService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<ProductReviewDto>("评价不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取评价详情失败: {Id}", id);
            return Error<ProductReviewDto>("获取评价详情失败");
        }
    }

    /// <summary>
    /// 回复评价
    /// </summary>
    /// <param name="dto">回复参数</param>
    /// <returns>影响的行数</returns>
    [HttpPost("reply")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Reply([FromBody] ReplyReviewDto dto)
    {
        try
        {
            var result = await _productReviewService.ReplyAsync(dto);
            return Success(result, "回复成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "回复评价失败");
            return Error<int>("回复评价失败");
        }
    }

    /// <summary>
    /// 审核评价
    /// </summary>
    /// <param name="dto">审核参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("audit")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Audit([FromBody] AuditReviewDto dto)
    {
        try
        {
            var result = await _productReviewService.AuditAsync(dto.Id, dto.Status);
            return Success(result, "审核成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "审核评价失败");
            return Error<int>("审核评价失败");
        }
    }

    /// <summary>
    /// 隐藏评价
    /// </summary>
    /// <param name="id">评价ID</param>
    /// <returns>影响的行数</returns>
    [HttpPut("hide/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Hide(Guid id)
    {
        try
        {
            var result = await _productReviewService.HideAsync(id);
            return Success(result, "隐藏成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "隐藏评价失败: {Id}", id);
            return Error<int>("隐藏评价失败");
        }
    }

    /// <summary>
    /// 获取评价统计
    /// </summary>
    /// <param name="productId">商品ID（可选）</param>
    /// <returns>评价统计信息</returns>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(ApiResponse<ReviewStatisticsDto>), 200)]
    public async Task<ApiResponse<ReviewStatisticsDto>> GetStatistics([FromQuery] Guid? productId)
    {
        try
        {
            var result = await _productReviewService.GetStatisticsAsync(productId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取评价统计失败");
            return Error<ReviewStatisticsDto>("获取评价统计失败");
        }
    }

    /// <summary>
    /// 删除评价
    /// </summary>
    /// <param name="id">评价ID</param>
    /// <returns>影响的行数</returns>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] Guid id)
    {
        try
        {
            var result = await _productReviewService.DeleteAsync(id);
            return Success(result, "删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除评价失败: {Id}", id);
            return Error<int>("删除评价失败");
        }
    }

    /// <summary>
    /// 批量审核评价
    /// </summary>
    /// <param name="dto">批量审核参数</param>
    /// <returns>影响的行数</returns>
    [HttpPost("batch-audit")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> BatchAudit([FromBody] BatchAuditReviewDto dto)
    {
        try
        {
            var result = await _productReviewService.BatchAuditAsync(dto.Ids, dto.Status);
            return Success(result, "审核成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量审核评价失败");
            return Error<int>("批量审核评价失败");
        }
    }
}