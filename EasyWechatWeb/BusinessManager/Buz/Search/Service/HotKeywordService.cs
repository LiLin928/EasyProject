using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using SqlSugar;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 热门关键词服务实现
/// </summary>
public class HotKeywordService : BaseService, IHotKeywordService
{
    /// <summary>
    /// 获取分页列表
    /// </summary>
    public async Task<PageResponse<HotKeywordDto>> GetPageListAsync(QueryHotKeywordDto query)
    {
        var queryable = _db.Queryable<HotKeyword>()
            // 条件筛选 - 使用 WhereIF
            .WhereIF(!string.IsNullOrEmpty(query.Keyword), k => k.Keyword.Contains(query.Keyword!))
            .WhereIF(query.Status.HasValue, k => k.Status == query.Status!.Value)
            .WhereIF(query.IsRecommend.HasValue, k => k.IsRecommend == query.IsRecommend!.Value)
            // 排序（先按排序字段，再按搜索次数）
            .OrderBy(k => k.Sort)
            .OrderByDescending(k => k.SearchCount);

        // 分页
        var total = new RefAsync<int>();
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 转换 DTO
        var dtoList = list.Select(k => new HotKeywordDto
        {
            Id = k.Id,
            Keyword = k.Keyword,
            SearchCount = k.SearchCount,
            Sort = k.Sort,
            Status = k.Status,
            IsRecommend = k.IsRecommend,
            CreateTime = k.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return PageResponse<HotKeywordDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 根据ID获取详情
    /// </summary>
    public async Task<HotKeywordDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<HotKeyword>().FirstAsync(k => k.Id == id);
        if (entity == null) return null;

        return new HotKeywordDto
        {
            Id = entity.Id,
            Keyword = entity.Keyword,
            SearchCount = entity.SearchCount,
            Sort = entity.Sort,
            Status = entity.Status,
            IsRecommend = entity.IsRecommend,
            CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    /// <summary>
    /// 创建
    /// </summary>
    public async Task<Guid> CreateAsync(AddHotKeywordDto dto)
    {
        // 检查关键词是否已存在
        var exists = await _db.Queryable<HotKeyword>()
            .Where(k => k.Keyword == dto.Keyword)
            .FirstAsync();

        if (exists != null)
        {
            throw new CommonManager.Error.BusinessException("关键词已存在");
        }

        var entity = new HotKeyword
        {
            Id = Guid.NewGuid(),
            Keyword = dto.Keyword,
            SearchCount = 0,
            Sort = dto.Sort,
            Status = dto.Status,
            IsRecommend = dto.IsRecommend,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <summary>
    /// 更新
    /// </summary>
    public async Task<int> UpdateAsync(UpdateHotKeywordDto dto)
    {
        var entity = await _db.Queryable<HotKeyword>().FirstAsync(k => k.Id == dto.Id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("关键词不存在");
        }

        if (!string.IsNullOrEmpty(dto.Keyword))
        {
            // 检查新关键词是否已被其他记录使用
            var exists = await _db.Queryable<HotKeyword>()
                .Where(k => k.Keyword == dto.Keyword && k.Id != dto.Id)
                .FirstAsync();

            if (exists != null)
            {
                throw new CommonManager.Error.BusinessException("关键词已存在");
            }
            entity.Keyword = dto.Keyword;
        }
        if (dto.SearchCount.HasValue)
        {
            entity.SearchCount = dto.SearchCount.Value;
        }
        if (dto.Sort.HasValue)
        {
            entity.Sort = dto.Sort.Value;
        }
        if (dto.Status.HasValue)
        {
            entity.Status = dto.Status.Value;
        }
        if (dto.IsRecommend.HasValue)
        {
            entity.IsRecommend = dto.IsRecommend.Value;
        }
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除
    /// </summary>
    public async Task<int> DeleteAsync(Guid id)
    {
        var entity = await _db.Queryable<HotKeyword>().FirstAsync(k => k.Id == id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("关键词不存在");
        }

        return await _db.Deleteable<HotKeyword>().Where(k => k.Id == id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新状态
    /// </summary>
    public async Task<int> UpdateStatusAsync(Guid id, int status)
    {
        var entity = await _db.Queryable<HotKeyword>().FirstAsync(k => k.Id == id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("关键词不存在");
        }

        entity.Status = status;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取推荐关键词列表
    /// </summary>
    public async Task<List<HotKeywordDto>> GetRecommendListAsync(int limit = 10)
    {
        var list = await _db.Queryable<HotKeyword>()
            .Where(k => k.Status == 1 && k.IsRecommend)
            .OrderBy(k => k.Sort)
            .OrderByDescending(k => k.SearchCount)
            .Take(limit)
            .ToListAsync();

        return list.Select(k => new HotKeywordDto
        {
            Id = k.Id,
            Keyword = k.Keyword,
            SearchCount = k.SearchCount,
            Sort = k.Sort,
            Status = k.Status,
            IsRecommend = k.IsRecommend,
            CreateTime = k.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();
    }

    /// <summary>
    /// 获取统计
    /// </summary>
    public async Task<HotKeywordStatisticsDto> GetStatisticsAsync()
    {
        var list = await _db.Queryable<HotKeyword>().ToListAsync();

        return new HotKeywordStatisticsDto
        {
            TotalCount = list.Count,
            ActiveCount = list.Count(k => k.Status == 1),
            RecommendCount = list.Count(k => k.IsRecommend),
            TotalSearchCount = list.Sum(k => k.SearchCount)
        };
    }
}