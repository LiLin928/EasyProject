using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using SqlSugar;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 会员等级服务实现
/// </summary>
public class MemberLevelService : BaseService, IMemberLevelService
{
    /// <summary>
    /// 获取所有会员等级列表
    /// </summary>
    public async Task<List<MemberLevelDto>> GetAllAsync()
    {
        var list = await _db.Queryable<MemberLevel>()
            .Where(l => l.Status == 1)
            .OrderBy(l => l.Sort)
            .ToListAsync();

        return list.Select(l => new MemberLevelDto
        {
            Id = l.Id,
            Name = l.Name,
            MinSpent = l.MinSpent,
            Discount = l.Discount,
            PointsRate = l.PointsRate,
            Icon = l.Icon,
            Sort = l.Sort,
            Status = l.Status,
            CreateTime = l.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();
    }

    /// <summary>
    /// 根据ID获取会员等级详情
    /// </summary>
    public async Task<MemberLevelDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<MemberLevel>()
            .FirstAsync(l => l.Id == id);

        if (entity == null) return null;

        return new MemberLevelDto
        {
            Id = entity.Id,
            Name = entity.Name,
            MinSpent = entity.MinSpent,
            Discount = entity.Discount,
            PointsRate = entity.PointsRate,
            Icon = entity.Icon,
            Sort = entity.Sort,
            Status = entity.Status,
            CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    /// <summary>
    /// 添加会员等级
    /// </summary>
    public async Task<Guid> AddAsync(AddMemberLevelDto dto)
    {
        // 检查名称是否存在
        var exists = await _db.Queryable<MemberLevel>()
            .Where(l => l.Name == dto.Name)
            .AnyAsync();
        if (exists)
        {
            throw new CommonManager.Error.BusinessException("等级名称已存在");
        }

        var entity = new MemberLevel
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            MinSpent = dto.MinSpent,
            Discount = dto.Discount,
            PointsRate = dto.PointsRate,
            Icon = dto.Icon,
            Sort = dto.Sort,
            Status = dto.Status,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <summary>
    /// 更新会员等级
    /// </summary>
    public async Task<int> UpdateAsync(UpdateMemberLevelDto dto)
    {
        var entity = await _db.Queryable<MemberLevel>()
            .FirstAsync(l => l.Id == dto.Id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("会员等级不存在");
        }

        // 检查名称唯一性
        if (!string.IsNullOrEmpty(dto.Name) && dto.Name != entity.Name)
        {
            var exists = await _db.Queryable<MemberLevel>()
                .Where(l => l.Name == dto.Name && l.Id != dto.Id)
                .AnyAsync();
            if (exists)
            {
                throw new CommonManager.Error.BusinessException("等级名称已存在");
            }
            entity.Name = dto.Name;
        }

        if (dto.MinSpent.HasValue) entity.MinSpent = dto.MinSpent.Value;
        if (dto.Discount.HasValue) entity.Discount = dto.Discount.Value;
        if (dto.PointsRate.HasValue) entity.PointsRate = dto.PointsRate.Value;
        if (!string.IsNullOrEmpty(dto.Icon)) entity.Icon = dto.Icon;
        if (dto.Sort.HasValue) entity.Sort = dto.Sort.Value;
        if (dto.Status.HasValue) entity.Status = dto.Status.Value;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除会员等级
    /// </summary>
    public async Task<int> DeleteAsync(Guid id)
    {
        // 检查是否有用户使用此等级
        var usedCount = await _db.Queryable<WeChatUser>()
            .Where(u => u.LevelId == id)
            .CountAsync();
        if (usedCount > 0)
        {
            throw new CommonManager.Error.BusinessException($"有 {usedCount} 个用户使用此等级，无法删除");
        }

        return await _db.Deleteable<MemberLevel>().Where(l => l.Id == id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 批量删除会员等级
    /// </summary>
    public async Task<int> DeleteBatchAsync(List<Guid> ids)
    {
        var count = 0;
        foreach (var id in ids)
        {
            count += await DeleteAsync(id);
        }
        return count;
    }
}

/// <summary>
/// 积分服务实现
/// </summary>
public class PointsService : BaseService, IPointsService
{
    /// <summary>
    /// 获取积分记录分页列表
    /// </summary>
    public async Task<PageResponse<PointsRecordDto>> GetPageListAsync(QueryPointsRecordDto query)
    {
        var queryable = _db.Queryable<PointsRecord>()
            // 条件筛选 - 使用 WhereIF
            .WhereIF(query.UserId.HasValue, r => r.UserId == query.UserId!.Value)
            .WhereIF(!string.IsNullOrEmpty(query.Type), r => r.Type == query.Type)
            .WhereIF(!string.IsNullOrEmpty(query.StartTime), r => r.CreateTime >= DateTime.Parse(query.StartTime!))
            .WhereIF(!string.IsNullOrEmpty(query.EndTime), r => r.CreateTime < DateTime.Parse(query.EndTime!).AddDays(1))
            .OrderByDescending(r => r.CreateTime);

        // 分页
        var total = new RefAsync<int>();
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 获取用户信息
        var userIds = list.Select(r => r.UserId).Distinct().ToList();
        var users = await _db.Queryable<WeChatUser>()
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();

        // 转换 DTO
        var dtoList = list.Select(r =>
        {
            var user = users.FirstOrDefault(u => u.Id == r.UserId);
            return new PointsRecordDto
            {
                Id = r.Id,
                UserId = r.UserId,
                UserName = user?.Nickname,
                UserPhone = user?.Phone,
                Points = r.Points,
                Balance = r.Balance,
                Type = r.Type,
                Reason = r.Reason,
                CreateTime = r.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }).ToList();

        // 用户关键字筛选
        if (!string.IsNullOrEmpty(query.UserKeyword))
        {
            var keyword = query.UserKeyword.ToLower();
            dtoList = dtoList.Where(d =>
                (d.UserName != null && d.UserName.ToLower().Contains(keyword)) ||
                (d.UserPhone != null && d.UserPhone.Contains(query.UserKeyword))
            ).ToList();
        }

        return PageResponse<PointsRecordDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }
}