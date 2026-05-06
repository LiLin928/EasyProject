using BusinessManager.Buz.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.WeChatPro;

/// <summary>
/// 微信优惠券控制器
/// </summary>
[ApiController]
[Route("api/wechat/coupon")]
public class WeChatCouponController : BaseController
{
    /// <summary>
    /// 优惠券服务
    /// </summary>
    public IWeChatCouponService _couponService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatCouponController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取可领取的优惠券列表
    /// </summary>
    /// <returns>可领取的优惠券列表</returns>
    [HttpGet("claimable")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<WxCouponDto>>), 200)]
    public async Task<ApiResponse<List<WxCouponDto>>> GetClaimableCoupons()
    {
        try
        {
            var result = await _couponService.GetClaimableCouponsAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取可领取优惠券列表失败");
            return Error<List<WxCouponDto>>("获取优惠券列表失败");
        }
    }

    /// <summary>
    /// 领取优惠券
    /// </summary>
    /// <param name="couponId">优惠券ID</param>
    /// <returns>用户优惠券ID</returns>
    [HttpPost("claim/{couponId}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> ClaimCoupon(Guid couponId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<Guid>("请先登录", 401);
            }

            var result = await _couponService.ClaimCouponAsync(userId, couponId);
            return Success(result, "领取成功");
        }
        catch (BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "领取优惠券失败: {CouponId}", couponId);
            return Error<Guid>("领取优惠券失败");
        }
    }

    /// <summary>
    /// 获取用户优惠券列表
    /// </summary>
    /// <param name="status">状态：1-未使用，2-已使用，3-已过期，不传则全部</param>
    /// <returns>用户优惠券列表</returns>
    [HttpGet("my")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<WxUserCouponDto>>), 200)]
    public async Task<ApiResponse<List<WxUserCouponDto>>> GetMyCoupons([FromQuery] int? status = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<List<WxUserCouponDto>>("请先登录", 401);
            }

            var result = await _couponService.GetUserCouponsAsync(userId, status);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户优惠券列表失败");
            return Error<List<WxUserCouponDto>>("获取优惠券列表失败");
        }
    }

    /// <summary>
    /// 获取订单可用优惠券列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>可用优惠券列表</returns>
    [HttpPost("available")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<AvailableCouponDto>>), 200)]
    public async Task<ApiResponse<List<AvailableCouponDto>>> GetAvailableCoupons([FromBody] QueryAvailableCouponDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<List<AvailableCouponDto>>("请先登录", 401);
            }

            var result = await _couponService.GetAvailableCouponsAsync(userId, query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取可用优惠券列表失败");
            return Error<List<AvailableCouponDto>>("获取可用优惠券失败");
        }
    }
}