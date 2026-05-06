using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using SqlSugar;
using System.Text.Json;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 优惠券服务实现
/// </summary>
public class CouponService : BaseService, ICouponService
{
    /// <summary>
    /// 获取优惠券分页列表
    /// </summary>
    public async Task<PageResponse<CouponDto>> GetPageListAsync(QueryCouponDto query)
    {
        var queryable = _db.Queryable<Coupon>()
            // 条件筛选 - 使用 WhereIF
            .WhereIF(!string.IsNullOrEmpty(query.Name), c => c.Name.Contains(query.Name!))
            .WhereIF(query.Type.HasValue, c => c.Type == query.Type!.Value)
            .WhereIF(query.Status.HasValue, c => c.Status == query.Status!.Value)
            .WhereIF(!string.IsNullOrEmpty(query.StartTime), c => c.StartTime >= DateTime.Parse(query.StartTime!))
            .WhereIF(!string.IsNullOrEmpty(query.EndTime), c => c.EndTime < DateTime.Parse(query.EndTime!).AddDays(1))
            .OrderByDescending(c => c.CreateTime);

        // 分页
        var total = new RefAsync<int>();
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 转换 DTO
        var dtoList = list.Select(c => new CouponDto
        {
            Id = c.Id,
            Name = c.Name,
            Type = c.Type,
            Value = c.Value,
            MinAmount = c.MinAmount,
            StartTime = c.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
            EndTime = c.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
            TotalCount = c.TotalCount,
            ClaimedCount = c.ClaimedCount,
            ProductIds = string.IsNullOrEmpty(c.ProductIds) ? null : JsonSerializer.Deserialize<List<Guid>>(c.ProductIds),
            Status = c.Status,
            CreateTime = c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return PageResponse<CouponDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 根据ID获取优惠券详情
    /// </summary>
    public async Task<CouponDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<Coupon>().FirstAsync(c => c.Id == id);
        if (entity == null) return null;

        return new CouponDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Type = entity.Type,
            Value = entity.Value,
            MinAmount = entity.MinAmount,
            StartTime = entity.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
            EndTime = entity.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
            TotalCount = entity.TotalCount,
            ClaimedCount = entity.ClaimedCount,
            ProductIds = string.IsNullOrEmpty(entity.ProductIds) ? null : JsonSerializer.Deserialize<List<Guid>>(entity.ProductIds),
            Status = entity.Status,
            CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    /// <summary>
    /// 创建优惠券
    /// </summary>
    public async Task<Guid> CreateAsync(AddCouponDto dto)
    {
        // 验证时间
        if (dto.EndTime <= dto.StartTime)
        {
            throw new CommonManager.Error.BusinessException("结束时间必须大于开始时间");
        }

        // 验证优惠值
        if (dto.Type == 1 && dto.Value <= 0)
        {
            throw new CommonManager.Error.BusinessException("满减券优惠金额必须大于0");
        }
        if (dto.Type == 2 && (dto.Value <= 0 || dto.Value > 1))
        {
            throw new CommonManager.Error.BusinessException("折扣券折扣比例必须在0-1之间");
        }

        var entity = new Coupon
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Type = dto.Type,
            Value = dto.Value,
            MinAmount = dto.MinAmount,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            TotalCount = dto.TotalCount,
            ClaimedCount = 0,
            ProductIds = dto.ProductIds != null && dto.ProductIds.Count > 0
                ? JsonSerializer.Serialize(dto.ProductIds)
                : null,
            Status = dto.Status,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <summary>
    /// 更新优惠券
    /// </summary>
    public async Task<int> UpdateAsync(UpdateCouponDto dto)
    {
        var entity = await _db.Queryable<Coupon>().FirstAsync(c => c.Id == dto.Id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("优惠券不存在");
        }

        if (!string.IsNullOrEmpty(dto.Name))
        {
            entity.Name = dto.Name;
        }
        if (dto.Value.HasValue)
        {
            if (entity.Type == 1 && dto.Value.Value <= 0)
            {
                throw new CommonManager.Error.BusinessException("满减券优惠金额必须大于0");
            }
            if (entity.Type == 2 && (dto.Value.Value <= 0 || dto.Value.Value > 1))
            {
                throw new CommonManager.Error.BusinessException("折扣券折扣比例必须在0-1之间");
            }
            entity.Value = dto.Value.Value;
        }
        if (dto.MinAmount.HasValue)
        {
            entity.MinAmount = dto.MinAmount.Value;
        }
        if (dto.StartTime.HasValue)
        {
            entity.StartTime = dto.StartTime.Value;
        }
        if (dto.EndTime.HasValue)
        {
            entity.EndTime = dto.EndTime.Value;
        }
        if (dto.TotalCount.HasValue)
        {
            if (dto.TotalCount.Value < entity.ClaimedCount)
            {
                throw new CommonManager.Error.BusinessException("发放总数不能小于已领取数量");
            }
            entity.TotalCount = dto.TotalCount.Value;
        }
        if (dto.ProductIds != null)
        {
            entity.ProductIds = dto.ProductIds.Count > 0
                ? JsonSerializer.Serialize(dto.ProductIds)
                : null;
        }
        if (dto.Status.HasValue)
        {
            entity.Status = dto.Status.Value;
        }

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除优惠券
    /// </summary>
    public async Task<int> DeleteAsync(Guid id)
    {
        var entity = await _db.Queryable<Coupon>().FirstAsync(c => c.Id == id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("优惠券不存在");
        }

        // 检查是否有用户已领取
        var claimedCount = await _db.Queryable<UserCoupon>()
            .Where(uc => uc.CouponId == id)
            .CountAsync();

        if (claimedCount > 0)
        {
            throw new CommonManager.Error.BusinessException("该优惠券已被用户领取，无法删除");
        }

        return await _db.Deleteable<Coupon>().Where(c => c.Id == id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新优惠券状态
    /// </summary>
    public async Task<int> UpdateStatusAsync(Guid id, int status)
    {
        var entity = await _db.Queryable<Coupon>().FirstAsync(c => c.Id == id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("优惠券不存在");
        }

        entity.Status = status;
        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取用户优惠券列表
    /// </summary>
    public async Task<PageResponse<UserCouponDto>> GetUserCouponListAsync(QueryUserCouponDto query)
    {
        var queryable = _db.Queryable<UserCoupon, Coupon, WeChatUser>(
            (uc, c, u) => new JoinQueryInfos(
                JoinType.Left, uc.CouponId == c.Id,
                JoinType.Left, uc.UserId == u.Id
            ))
            // 条件筛选 - 使用 WhereIF
            .WhereIF(query.UserId.HasValue, (uc, c, u) => uc.UserId == query.UserId!.Value)
            .WhereIF(query.CouponId.HasValue, (uc, c, u) => uc.CouponId == query.CouponId!.Value)
            .WhereIF(query.Status.HasValue, (uc, c, u) => uc.Status == query.Status!.Value)
            .WhereIF(!string.IsNullOrEmpty(query.CouponName), (uc, c, u) => c.Name.Contains(query.CouponName!))
            .OrderByDescending((uc, c, u) => uc.ClaimTime);

        // 分页
        var total = new RefAsync<int>();
        var list = await queryable
            .Select((uc, c, u) => new UserCouponDto
            {
                Id = uc.Id,
                UserId = uc.UserId,
                UserName = u.Nickname,
                CouponId = uc.CouponId,
                CouponName = c.Name,
                Type = c.Type,
                Value = c.Value,
                MinAmount = c.MinAmount,
                Status = uc.Status,
                UsedTime = uc.UsedTime != null ? uc.UsedTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                OrderId = uc.OrderId,
                ClaimTime = uc.ClaimTime.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<UserCouponDto>.Create(list, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取优惠券统计
    /// </summary>
    public async Task<CouponStatisticsDto> GetStatisticsAsync()
    {
        var coupons = await _db.Queryable<Coupon>().ToListAsync();
        var userCoupons = await _db.Queryable<UserCoupon>().ToListAsync();

        return new CouponStatisticsDto
        {
            TotalCount = coupons.Count,
            ActiveCount = coupons.Count(c => c.Status == 1),
            InactiveCount = coupons.Count(c => c.Status == 0),
            TotalClaimedCount = userCoupons.Count,
            UnusedCount = userCoupons.Count(uc => uc.Status == 1),
            UsedCount = userCoupons.Count(uc => uc.Status == 2),
            ExpiredCount = userCoupons.Count(uc => uc.Status == 3)
        };
    }
}