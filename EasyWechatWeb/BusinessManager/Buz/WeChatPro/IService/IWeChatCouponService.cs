using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 微信优惠券服务接口
/// </summary>
public interface IWeChatCouponService
{
    /// <summary>
    /// 获取可领取的优惠券列表
    /// </summary>
    /// <returns>可领取的优惠券列表</returns>
    Task<List<WxCouponDto>> GetClaimableCouponsAsync();

    /// <summary>
    /// 领取优惠券
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="couponId">优惠券ID</param>
    /// <returns>用户优惠券ID</returns>
    Task<Guid> ClaimCouponAsync(Guid userId, Guid couponId);

    /// <summary>
    /// 获取用户优惠券列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="status">状态：1-未使用，2-已使用，3-已过期，null-全部</param>
    /// <returns>用户优惠券列表</returns>
    Task<List<WxUserCouponDto>> GetUserCouponsAsync(Guid userId, int? status = null);

    /// <summary>
    /// 获取订单可用优惠券列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="query">查询参数</param>
    /// <returns>可用优惠券列表</returns>
    Task<List<AvailableCouponDto>> GetAvailableCouponsAsync(Guid userId, QueryAvailableCouponDto query);

    /// <summary>
    /// 使用优惠券
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="userCouponId">用户优惠券ID</param>
    /// <param name="orderId">订单ID</param>
    /// <returns>是否成功</returns>
    Task<bool> UseCouponAsync(Guid userId, Guid userCouponId, Guid orderId);
}