using BusinessManager.Buz.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using SqlSugar;
using System.Text.Json;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 微信优惠券服务实现
/// </summary>
public class WeChatCouponService : BaseService, IWeChatCouponService
{
    /// <summary>
    /// 获取可领取的优惠券列表
    /// </summary>
    public async Task<List<WxCouponDto>> GetClaimableCouponsAsync()
    {
        var now = DateTime.Now;
        var coupons = await _db.Queryable<Coupon>()
            .Where(c => c.Status == 1 && c.StartTime <= now && c.EndTime >= now)
            .Where(c => c.ClaimedCount < c.TotalCount)
            .OrderByDescending(c => c.CreateTime)
            .ToListAsync();

        return coupons.Select(c => new WxCouponDto
        {
            Id = c.Id,
            Name = c.Name,
            Type = c.Type,
            Value = c.Value,
            MinAmount = c.MinAmount,
            StartTime = new DateTimeOffset(c.StartTime).ToUnixTimeMilliseconds(),
            EndTime = new DateTimeOffset(c.EndTime).ToUnixTimeMilliseconds(),
            CanClaim = true
        }).ToList();
    }

    /// <summary>
    /// 领取优惠券
    /// </summary>
    public async Task<Guid> ClaimCouponAsync(Guid userId, Guid couponId)
    {
        var coupon = await _db.Queryable<Coupon>()
            .Where(c => c.Id == couponId && c.Status == 1)
            .FirstAsync();

        if (coupon == null)
        {
            throw new BusinessException("优惠券不存在");
        }

        var now = DateTime.Now;
        if (coupon.StartTime > now || coupon.EndTime < now)
        {
            throw new BusinessException("优惠券不在有效期内");
        }

        if (coupon.ClaimedCount >= coupon.TotalCount)
        {
            throw new BusinessException("优惠券已领完");
        }

        // 检查是否已领取
        var exists = await _db.Queryable<UserCoupon>()
            .Where(uc => uc.UserId == userId && uc.CouponId == couponId)
            .FirstAsync();

        if (exists != null)
        {
            throw new BusinessException("您已领取过该优惠券");
        }

        // 创建用户优惠券
        var userCoupon = new UserCoupon
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CouponId = couponId,
            Status = 1,
            ClaimTime = now
        };

        // 更新领取数量
        coupon.ClaimedCount++;

        await _db.Insertable(userCoupon).ExecuteCommandAsync();
        await _db.Updateable(coupon).ExecuteCommandAsync();

        return userCoupon.Id;
    }

    /// <summary>
    /// 获取用户优惠券列表
    /// </summary>
    public async Task<List<WxUserCouponDto>> GetUserCouponsAsync(Guid userId, int? status = null)
    {
        var query = _db.Queryable<UserCoupon, Coupon>((uc, c) => new JoinQueryInfos(
            JoinType.Left, uc.CouponId == c.Id
        ))
        .Where((uc, c) => uc.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where((uc, c) => uc.Status == status.Value);
        }

        var list = await query
            .OrderByDescending((uc, c) => uc.ClaimTime)
            .Select((uc, c) => new WxUserCouponDto
            {
                Id = uc.Id,
                CouponId = uc.CouponId,
                Name = c.Name,
                Type = c.Type,
                Value = c.Value,
                MinAmount = c.MinAmount,
                Status = uc.Status,
                StartTime = new DateTimeOffset(c.StartTime).ToUnixTimeMilliseconds(),
                EndTime = new DateTimeOffset(c.EndTime).ToUnixTimeMilliseconds(),
                ClaimTime = new DateTimeOffset(uc.ClaimTime).ToUnixTimeMilliseconds()
            })
            .ToListAsync();

        // 检查过期状态（使用本地时间保持一致）
        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        foreach (var item in list.Where(i => i.Status == 1 && i.EndTime < now))
        {
            item.Status = 3; // 已过期
        }

        return list;
    }

    /// <summary>
    /// 获取订单可用优惠券列表
    /// </summary>
    public async Task<List<AvailableCouponDto>> GetAvailableCouponsAsync(Guid userId, QueryAvailableCouponDto query)
    {
        var now = DateTime.Now;
        var userCoupons = await _db.Queryable<UserCoupon, Coupon>((uc, c) => new JoinQueryInfos(
            JoinType.Left, uc.CouponId == c.Id
        ))
        .Where((uc, c) => uc.UserId == userId && uc.Status == 1)
        .Where((uc, c) => c.StartTime <= now && c.EndTime >= now)
        .Where((uc, c) => c.MinAmount <= query.OrderAmount)
        .Select((uc, c) => new
        {
            UserCouponId = uc.Id,
            CouponId = c.Id,
            c.Name,
            c.Type,
            c.Value,
            c.MinAmount,
            c.ProductIds
        })
        .ToListAsync();

        var result = new List<AvailableCouponDto>();

        foreach (var uc in userCoupons)
        {
            // 检查商品限制
            if (!string.IsNullOrEmpty(uc.ProductIds) && query.ProductIds != null)
            {
                var couponProductIds = JsonSerializer.Deserialize<List<Guid>>(uc.ProductIds);
                if (couponProductIds != null && couponProductIds.Count > 0)
                {
                    var hasMatch = query.ProductIds.Any(p => couponProductIds.Contains(p));
                    if (!hasMatch)
                    {
                        continue;
                    }
                }
            }

            // 计算优惠金额
            decimal discountAmount = 0;
            if (uc.Type == 1)
            {
                // 满减券
                discountAmount = uc.Value;
            }
            else
            {
                // 折扣券
                discountAmount = Math.Round(query.OrderAmount * (1 - uc.Value), 2);
            }

            result.Add(new AvailableCouponDto
            {
                UserCouponId = uc.UserCouponId,
                CouponId = uc.CouponId,
                Name = uc.Name,
                Type = uc.Type,
                Value = uc.Value,
                MinAmount = uc.MinAmount,
                DiscountAmount = discountAmount,
                Description = uc.Type == 1
                    ? (uc.MinAmount > 0 ? $"满{uc.MinAmount}减{uc.Value}" : $"直减{uc.Value}元")
                    : (uc.MinAmount > 0 ? $"满{uc.MinAmount}打{uc.Value * 10}折" : $"{uc.Value * 10}折券")
            });
        }

        // 按优惠金额降序排序
        return result.OrderByDescending(r => r.DiscountAmount).ToList();
    }

    /// <summary>
    /// 使用优惠券
    /// </summary>
    public async Task<bool> UseCouponAsync(Guid userId, Guid userCouponId, Guid orderId)
    {
        var userCoupon = await _db.Queryable<UserCoupon>()
            .Where(uc => uc.Id == userCouponId && uc.UserId == userId && uc.Status == 1)
            .FirstAsync();

        if (userCoupon == null)
        {
            throw new BusinessException("优惠券不存在或已使用");
        }

        var coupon = await _db.Queryable<Coupon>()
            .Where(c => c.Id == userCoupon.CouponId)
            .FirstAsync();

        if (coupon == null)
        {
            throw new BusinessException("优惠券信息不存在");
        }

        var now = DateTime.Now;
        if (coupon.EndTime < now)
        {
            throw new BusinessException("优惠券已过期");
        }

        userCoupon.Status = 2; // 已使用
        userCoupon.UsedTime = now;
        userCoupon.OrderId = orderId;

        await _db.Updateable(userCoupon).ExecuteCommandAsync();

        return true;
    }
}