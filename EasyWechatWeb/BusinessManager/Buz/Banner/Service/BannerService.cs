using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using SqlSugar;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 轮播图服务实现
/// </summary>
public class BannerService : BaseService, IBannerService
{
    /// <summary>
    /// 获取轮播图分页列表
    /// </summary>
    public async Task<PageResponse<BannerDto>> GetPageListAsync(QueryBannerDto query)
    {
        var queryable = _db.Queryable<Banner>()
            // 条件筛选 - 使用 WhereIF
            .WhereIF(query.Status.HasValue, b => b.Status == query.Status!.Value)
            .OrderBy(b => b.Sort)
            .OrderByDescending(b => b.CreateTime);

        // 分页
        var total = new RefAsync<int>();
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 转换 DTO
        var dtoList = list.Select(b => new BannerDto
        {
            Id = b.Id,
            Image = b.Image,
            LinkType = b.LinkType,
            LinkValue = b.LinkValue,
            Sort = b.Sort,
            Status = b.Status,
            CreateTime = b.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = b.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return PageResponse<BannerDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取所有启用的轮播图列表（用于前端展示）
    /// </summary>
    public async Task<List<BannerDto>> GetActiveListAsync()
    {
        var list = await _db.Queryable<Banner>()
            .Where(b => b.Status == 1)
            .OrderBy(b => b.Sort)
            .ToListAsync();

        // 转换 DTO
        var dtoList = list.Select(b => new BannerDto
        {
            Id = b.Id,
            Image = b.Image,
            LinkType = b.LinkType,
            LinkValue = b.LinkValue,
            Sort = b.Sort,
            Status = b.Status,
            CreateTime = b.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = b.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return dtoList;
    }

    /// <summary>
    /// 根据ID获取轮播图详情
    /// </summary>
    public async Task<BannerDto?> GetByIdAsync(Guid id)
    {
        var banner = await _db.Queryable<Banner>()
            .Where(b => b.Id == id)
            .FirstAsync();

        if (banner == null)
        {
            return null;
        }

        return new BannerDto
        {
            Id = banner.Id,
            Image = banner.Image,
            LinkType = banner.LinkType,
            LinkValue = banner.LinkValue,
            Sort = banner.Sort,
            Status = banner.Status,
            CreateTime = banner.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = banner.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    /// <summary>
    /// 添加轮播图
    /// </summary>
    public async Task<Guid> AddAsync(AddBannerDto dto)
    {
        var banner = new Banner
        {
            Id = Guid.NewGuid(),
            Image = dto.Image,
            LinkType = dto.LinkType,
            LinkValue = dto.LinkValue,
            Sort = dto.Sort,
            Status = dto.Status,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(banner).ExecuteCommandAsync();
        return banner.Id;
    }

    /// <summary>
    /// 更新轮播图
    /// </summary>
    public async Task<int> UpdateAsync(UpdateBannerDto dto)
    {
        var banner = await _db.Queryable<Banner>()
            .Where(b => b.Id == dto.Id)
            .FirstAsync();

        if (banner == null)
        {
            return 0;
        }

        banner.Image = dto.Image;
        banner.LinkType = dto.LinkType;
        banner.LinkValue = dto.LinkValue;
        banner.Sort = dto.Sort;
        banner.Status = dto.Status;
        banner.UpdateTime = DateTime.Now;

        return await _db.Updateable(banner).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除轮播图
    /// </summary>
    public async Task<int> DeleteAsync(Guid id)
    {
        return await _db.Deleteable<Banner>()
            .Where(b => b.Id == id)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新轮播图状态
    /// </summary>
    public async Task<int> UpdateStatusAsync(Guid id, int status)
    {
        return await _db.Updateable<Banner>()
            .SetColumns(b => new Banner
            {
                Status = status,
                UpdateTime = DateTime.Now
            })
            .Where(b => b.Id == id)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 批量更新排序
    /// </summary>
    public async Task<int> BatchSortAsync(SortBannerDto dto)
    {
        var count = 0;
        foreach (var item in dto.Items)
        {
            var result = await _db.Updateable<Banner>()
                .SetColumns(b => new Banner
                {
                    Sort = item.Sort,
                    UpdateTime = DateTime.Now
                })
                .Where(b => b.Id == item.Id)
                .ExecuteCommandAsync();
            count += result;
        }
        return count;
    }
}