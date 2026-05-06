using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using SqlSugar;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 客户服务实现
/// </summary>
public class CustomerService : BaseService, ICustomerService
{
    /// <summary>
    /// 获取客户分页列表
    /// </summary>
    public async Task<PageResponse<CustomerDto>> GetPageListAsync(QueryCustomerDto query)
    {
        var queryable = _db.Queryable<WeChatUser>()
            // 条件筛选 - 使用 WhereIF
            .WhereIF(!string.IsNullOrEmpty(query.Phone), u => u.Phone != null && u.Phone.Contains(query.Phone!))
            .WhereIF(!string.IsNullOrEmpty(query.Nickname), u => u.Nickname != null && u.Nickname.Contains(query.Nickname!))
            .WhereIF(query.LevelId.HasValue, u => u.LevelId == query.LevelId!.Value)
            .WhereIF(query.Status.HasValue, u => u.Status == query.Status!.Value)
            .OrderByDescending(u => u.CreateTime);

        // 分页
        var total = new RefAsync<int>();
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 获取会员等级
        var levelIds = list.Where(u => u.LevelId.HasValue).Select(u => u.LevelId!.Value).Distinct().ToList();
        var levels = await _db.Queryable<MemberLevel>()
            .Where(l => levelIds.Contains(l.Id))
            .ToListAsync();

        // 转换 DTO
        var dtoList = list.Select(u => new CustomerDto
        {
            Id = u.Id,
            Username = u.UserName,
            Nickname = u.Nickname,
            Phone = u.Phone,
            Email = u.Email,
            Avatar = u.AvatarUrl,
            Status = u.Status,
            LevelId = u.LevelId,
            LevelName = u.LevelId.HasValue ? levels.FirstOrDefault(l => l.Id == u.LevelId.Value)?.Name : null,
            Points = u.Points,
            TotalSpent = u.TotalSpent,
            CreateTime = u.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = u.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return PageResponse<CustomerDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 根据ID获取客户详情
    /// </summary>
    public async Task<CustomerDetailDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<WeChatUser>()
            .FirstAsync(u => u.Id == id);

        if (entity == null) return null;

        // 获取会员等级名称
        string? levelName = null;
        if (entity.LevelId.HasValue)
        {
            var level = await _db.Queryable<MemberLevel>()
                .FirstAsync(l => l.Id == entity.LevelId.Value);
            levelName = level?.Name;
        }

        // 统计数量
        var addressCount = await _db.Queryable<Address>().Where(a => a.UserId == id && a.Status == 1).CountAsync();
        var cartCount = await _db.Queryable<Cart>().Where(c => c.UserId == id).CountAsync();
        var favoriteCount = await _db.Queryable<UserFavorite>().Where(f => f.UserId == id).CountAsync();
        var orderCount = await _db.Queryable<Order>().Where(o => o.UserId == id).CountAsync();

        return new CustomerDetailDto
        {
            Id = entity.Id,
            Username = entity.UserName,
            Nickname = entity.Nickname,
            Phone = entity.Phone,
            Email = entity.Email,
            Avatar = entity.AvatarUrl,
            Status = entity.Status,
            LevelId = entity.LevelId,
            LevelName = levelName,
            Points = entity.Points,
            TotalSpent = entity.TotalSpent,
            AddressCount = addressCount,
            CartCount = cartCount,
            FavoriteCount = favoriteCount,
            OrderCount = orderCount,
            CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = entity.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    /// <summary>
    /// 添加客户
    /// </summary>
    public async Task<Guid> AddAsync(AddCustomerDto dto)
    {
        // 检查手机号是否存在
        var exists = await _db.Queryable<WeChatUser>()
            .Where(u => u.Phone == dto.Phone)
            .AnyAsync();
        if (exists)
        {
            throw new CommonManager.Error.BusinessException("手机号已存在");
        }

        // 检查用户名是否存在（如果提供了用户名）
        if (!string.IsNullOrEmpty(dto.Username))
        {
            var usernameExists = await _db.Queryable<WeChatUser>()
                .Where(u => u.UserName == dto.Username)
                .AnyAsync();
            if (usernameExists)
            {
                throw new CommonManager.Error.BusinessException("用户名已存在");
            }
        }

        var entity = new WeChatUser
        {
            Id = Guid.NewGuid(),
            UserName = !string.IsNullOrEmpty(dto.Username) ? dto.Username : dto.Phone,
            Nickname = dto.Nickname,
            Phone = dto.Phone,
            Email = dto.Email,
            AvatarUrl = dto.Avatar,
            Status = dto.Status,
            LevelId = dto.LevelId,
            OpenId = "",
            CreateTime = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <summary>
    /// 更新客户
    /// </summary>
    public async Task<int> UpdateAsync(UpdateCustomerDto dto)
    {
        var entity = await _db.Queryable<WeChatUser>()
            .FirstAsync(u => u.Id == dto.Id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("客户不存在");
        }

        // 检查用户名唯一性
        if (!string.IsNullOrEmpty(dto.Username) && dto.Username != entity.UserName)
        {
            var usernameExists = await _db.Queryable<WeChatUser>()
                .Where(u => u.UserName == dto.Username && u.Id != dto.Id)
                .AnyAsync();
            if (usernameExists)
            {
                throw new CommonManager.Error.BusinessException("用户名已存在");
            }
            entity.UserName = dto.Username;
        }

        // 检查手机号唯一性
        if (!string.IsNullOrEmpty(dto.Phone) && dto.Phone != entity.Phone)
        {
            var exists = await _db.Queryable<WeChatUser>()
                .Where(u => u.Phone == dto.Phone && u.Id != dto.Id)
                .AnyAsync();
            if (exists)
            {
                throw new CommonManager.Error.BusinessException("手机号已存在");
            }
            entity.Phone = dto.Phone;
        }

        if (!string.IsNullOrEmpty(dto.Nickname)) entity.Nickname = dto.Nickname;
        if (!string.IsNullOrEmpty(dto.Email)) entity.Email = dto.Email;
        if (!string.IsNullOrEmpty(dto.Avatar)) entity.AvatarUrl = dto.Avatar;
        if (dto.Status.HasValue) entity.Status = dto.Status.Value;
        if (dto.LevelId.HasValue) entity.LevelId = dto.LevelId.Value;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除客户
    /// </summary>
    public async Task<int> DeleteAsync(List<Guid> ids)
    {
        if (ids == null || ids.Count == 0) return 0;

        // 删除相关数据
        await _db.Deleteable<Address>().Where(a => ids.Contains(a.UserId)).ExecuteCommandAsync();
        await _db.Deleteable<Cart>().Where(c => ids.Contains(c.UserId)).ExecuteCommandAsync();
        await _db.Deleteable<UserFavorite>().Where(f => ids.Contains(f.UserId)).ExecuteCommandAsync();
        await _db.Deleteable<FavoriteGroup>().Where(g => ids.Contains(g.UserId)).ExecuteCommandAsync();

        return await _db.Deleteable<WeChatUser>().Where(u => ids.Contains(u.Id)).ExecuteCommandAsync();
    }

    /// <summary>
    /// 调整客户积分
    /// </summary>
    public async Task<int> AdjustPointsAsync(AdjustPointsDto dto)
    {
        var entity = await _db.Queryable<WeChatUser>()
            .FirstAsync(u => u.Id == dto.UserId);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("客户不存在");
        }

        var beforePoints = entity.Points;
        var changePoints = dto.Type == "add" ? dto.Amount : -dto.Amount;
        var afterPoints = beforePoints + changePoints;

        if (afterPoints < 0)
        {
            throw new CommonManager.Error.BusinessException($"积分不足，当前积分 {beforePoints}");
        }

        entity.Points = afterPoints;
        entity.UpdateTime = DateTime.Now;

        // 记录积分变动
        var record = new PointsRecord
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Points = changePoints,
            Balance = afterPoints,
            Type = "system",
            Reason = dto.Reason,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(record).ExecuteCommandAsync();
        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 调整客户等级
    /// </summary>
    public async Task<int> AdjustLevelAsync(AdjustLevelDto dto)
    {
        var entity = await _db.Queryable<WeChatUser>()
            .FirstAsync(u => u.Id == dto.UserId);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("客户不存在");
        }

        var level = await _db.Queryable<MemberLevel>()
            .FirstAsync(l => l.Id == dto.LevelId);
        if (level == null)
        {
            throw new CommonManager.Error.BusinessException("会员等级不存在");
        }

        entity.LevelId = dto.LevelId;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取客户地址列表
    /// </summary>
    public async Task<List<AddressDto>> GetAddressesAsync(Guid userId)
    {
        var list = await _db.Queryable<Address>()
            .Where(a => a.UserId == userId && a.Status == 1)
            .OrderByDescending(a => a.IsDefault)
            .OrderByDescending(a => a.CreateTime)
            .ToListAsync();

        return list.Select(a => new AddressDto
        {
            Id = a.Id,
            Name = a.Name,
            Phone = a.Phone,
            Province = a.Province,
            City = a.City,
            District = a.District,
            Detail = a.Detail,
            IsDefault = a.IsDefault
        }).ToList();
    }

    /// <summary>
    /// 获取客户购物车列表
    /// </summary>
    public async Task<List<CartDto>> GetCartsAsync(Guid userId)
    {
        var carts = await _db.Queryable<Cart>()
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreateTime)
            .ToListAsync();

        var productIds = carts.Select(c => c.ProductId).Distinct().ToList();
        var products = await _db.Queryable<Product>()
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        return carts.Select(c =>
        {
            var product = products.FirstOrDefault(p => p.Id == c.ProductId);
            return new CartDto
            {
                Id = c.Id,
                ProductId = c.ProductId,
                Product = product != null ? new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Image = product.Image,
                    Price = product.Price
                } : new ProductDto(),
                Count = c.Count,
                Selected = c.Selected,
                Subtotal = (product?.Price ?? 0) * c.Count,
                CreateTime = c.CreateTime
            };
        }).ToList();
    }

    /// <summary>
    /// 获取客户收藏列表
    /// </summary>
    public async Task<List<UserFavoriteDto>> GetFavoritesAsync(Guid userId, Guid? groupId = null)
    {
        var queryable = _db.Queryable<UserFavorite>()
            .Where(f => f.UserId == userId);

        if (groupId.HasValue)
        {
            queryable = queryable.Where(f => f.GroupId == groupId.Value);
        }

        var favorites = await queryable.OrderByDescending(f => f.CreateTime).ToListAsync();

        var productIds = favorites.Select(f => f.ProductId).Distinct().ToList();
        var products = await _db.Queryable<Product>()
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        // 获取分组名称
        var groupIds = favorites.Where(f => f.GroupId.HasValue).Select(f => f.GroupId!.Value).Distinct().ToList();
        var groups = await _db.Queryable<FavoriteGroup>()
            .Where(g => groupIds.Contains(g.Id))
            .ToListAsync();

        return favorites.Select(f =>
        {
            var product = products.FirstOrDefault(p => p.Id == f.ProductId);
            var group = f.GroupId.HasValue ? groups.FirstOrDefault(g => g.Id == f.GroupId.Value) : null;
            return new UserFavoriteDto
            {
                Id = f.Id,
                ProductId = f.ProductId,
                ProductName = product?.Name ?? "",
                ProductImage = product?.Image,
                Price = product?.Price ?? 0,
                GroupId = f.GroupId,
                GroupName = group?.Name,
                CreateTime = f.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }).ToList();
    }

    /// <summary>
    /// 获取客户收藏分组列表
    /// </summary>
    public async Task<List<FavoriteGroupDto>> GetFavoriteGroupsAsync(Guid userId)
    {
        var groups = await _db.Queryable<FavoriteGroup>()
            .Where(g => g.UserId == userId)
            .OrderBy(g => g.Sort)
            .ToListAsync();

        var groupIds = groups.Select(g => g.Id).ToList();
        var favorites = await _db.Queryable<UserFavorite>()
            .Where(f => f.UserId == userId && f.GroupId != null && groupIds.Contains(f.GroupId!.Value))
            .ToListAsync();

        var favoriteCounts = favorites
            .GroupBy(f => f.GroupId)
            .ToDictionary(g => g.Key!.Value, g => g.Count());

        return groups.Select(g => new FavoriteGroupDto
        {
            Id = g.Id,
            Name = g.Name,
            Sort = g.Sort,
            FavoriteCount = favoriteCounts.ContainsKey(g.Id) ? favoriteCounts[g.Id] : 0
        }).ToList();
    }

    /// <summary>
    /// 更新客户状态
    /// </summary>
    public async Task<int> UpdateStatusAsync(Guid id, int status)
    {
        var entity = await _db.Queryable<WeChatUser>()
            .FirstAsync(u => u.Id == id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("客户不存在");
        }

        entity.Status = status;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }
}