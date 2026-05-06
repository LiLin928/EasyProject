using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 优惠券服务接口
/// </summary>
/// <remarks>
/// 提供优惠券管理、用户优惠券查询等功能
/// </remarks>
public interface ICouponService
{
    /// <summary>
    /// 获取优惠券分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页优惠券列表</returns>
    Task<PageResponse<CouponDto>> GetPageListAsync(QueryCouponDto query);

    /// <summary>
    /// 根据ID获取优惠券详情
    /// </summary>
    /// <param name="id">优惠券ID</param>
    /// <returns>优惠券信息</returns>
    Task<CouponDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建优惠券
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的优惠券ID</returns>
    Task<Guid> CreateAsync(AddCouponDto dto);

    /// <summary>
    /// 更新优惠券
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateAsync(UpdateCouponDto dto);

    /// <summary>
    /// 删除优惠券
    /// </summary>
    /// <param name="id">优惠券ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 更新优惠券状态
    /// </summary>
    /// <param name="id">优惠券ID</param>
    /// <param name="status">状态：1-启用，0-禁用</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateStatusAsync(Guid id, int status);

    /// <summary>
    /// 获取用户优惠券列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页用户优惠券列表</returns>
    Task<PageResponse<UserCouponDto>> GetUserCouponListAsync(QueryUserCouponDto query);

    /// <summary>
    /// 获取优惠券统计
    /// </summary>
    /// <returns>统计信息</returns>
    Task<CouponStatisticsDto> GetStatisticsAsync();
}