using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 优惠券管理控制器
/// </summary>
/// <remarks>
/// 提供优惠券管理、用户优惠券查询等功能接口
/// </remarks>
[ApiController]
[Route("api/coupon")]
[Authorize]
public class CouponController : BaseController
{
    /// <summary>
    /// 优惠券服务接口
    /// </summary>
    public ICouponService _couponService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<CouponController> _logger { get; set; } = null!;

    #region 优惠券管理

    /// <summary>
    /// 获取优惠券列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页优惠券列表</returns>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<CouponDto>>), 200)]
    public async Task<ApiResponse<PageResponse<CouponDto>>> GetList([FromBody] QueryCouponDto query)
    {
        try
        {
            var result = await _couponService.GetPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取优惠券列表失败");
            return Error<PageResponse<CouponDto>>("获取优惠券列表失败");
        }
    }

    /// <summary>
    /// 获取优惠券详情
    /// </summary>
    /// <param name="id">优惠券ID</param>
    /// <returns>优惠券详情</returns>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<CouponDto>), 200)]
    public async Task<ApiResponse<CouponDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _couponService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<CouponDto>("优惠券不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取优惠券详情失败: {Id}", id);
            return Error<CouponDto>("获取优惠券详情失败");
        }
    }

    /// <summary>
    /// 创建优惠券
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的优惠券ID</returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Create([FromBody] AddCouponDto dto)
    {
        try
        {
            var result = await _couponService.CreateAsync(dto);
            return Success(result, "创建成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建优惠券失败");
            return Error<Guid>("创建优惠券失败");
        }
    }

    /// <summary>
    /// 更新优惠券
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateCouponDto dto)
    {
        try
        {
            var result = await _couponService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新优惠券失败: {Id}", dto.Id);
            return Error<int>("更新优惠券失败");
        }
    }

    /// <summary>
    /// 删除优惠券
    /// </summary>
    /// <param name="id">优惠券ID</param>
    /// <returns>影响的行数</returns>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] Guid id)
    {
        try
        {
            var result = await _couponService.DeleteAsync(id);
            return Success(result, "删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除优惠券失败: {Id}", id);
            return Error<int>("删除优惠券失败");
        }
    }

    /// <summary>
    /// 更新优惠券状态
    /// </summary>
    /// <param name="id">优惠券ID</param>
    /// <param name="status">状态：1-启用，0-禁用</param>
    /// <returns>影响的行数</returns>
    [HttpPut("status/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> UpdateStatus(Guid id, [FromQuery] int status)
    {
        try
        {
            var result = await _couponService.UpdateStatusAsync(id, status);
            return Success(result, "状态更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新优惠券状态失败: {Id}", id);
            return Error<int>("更新优惠券状态失败");
        }
    }

    /// <summary>
    /// 获取优惠券统计
    /// </summary>
    /// <returns>统计信息</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponse<CouponStatisticsDto>), 200)]
    public async Task<ApiResponse<CouponStatisticsDto>> GetStatistics()
    {
        try
        {
            var result = await _couponService.GetStatisticsAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取优惠券统计失败");
            return Error<CouponStatisticsDto>("获取优惠券统计失败");
        }
    }

    #endregion

    #region 用户优惠券管理

    /// <summary>
    /// 获取用户优惠券列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页用户优惠券列表</returns>
    [HttpGet("user-coupons")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<UserCouponDto>>), 200)]
    public async Task<ApiResponse<PageResponse<UserCouponDto>>> GetUserCoupons([FromQuery] QueryUserCouponDto query)
    {
        try
        {
            var result = await _couponService.GetUserCouponListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户优惠券列表失败");
            return Error<PageResponse<UserCouponDto>>("获取用户优惠券列表失败");
        }
    }

    #endregion
}