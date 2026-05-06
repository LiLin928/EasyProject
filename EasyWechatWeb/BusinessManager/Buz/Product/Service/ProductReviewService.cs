using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using SqlSugar;
using System.Text.Json;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 商品评价服务实现
/// </summary>
public class ProductReviewService : BaseService, IProductReviewService
{
    /// <summary>
    /// 获取评价分页列表
    /// </summary>
    public async Task<PageResponse<ProductReviewDto>> GetPageListAsync(QueryProductReviewDto query)
    {
        var queryable = _db.Queryable<ProductReview>()
            // 条件筛选 - 使用 WhereIF
            .WhereIF(query.ProductId.HasValue, r => r.ProductId == query.ProductId!.Value)
            .WhereIF(!string.IsNullOrEmpty(query.ProductName), r => r.ProductName.Contains(query.ProductName!))
            .WhereIF(!string.IsNullOrEmpty(query.UserName), r => r.UserName.Contains(query.UserName!))
            .WhereIF(query.Rating.HasValue, r => r.Rating == query.Rating!.Value)
            .WhereIF(!string.IsNullOrEmpty(query.Status), r => r.Status == query.Status)
            .WhereIF(!string.IsNullOrEmpty(query.StartDate), r => r.CreateTime >= DateTime.Parse(query.StartDate!))
            .WhereIF(!string.IsNullOrEmpty(query.EndDate), r => r.CreateTime < DateTime.Parse(query.EndDate!).AddDays(1))
            .OrderByDescending(r => r.CreateTime);

        // 分页
        var total = new RefAsync<int>();
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 转换 DTO
        var dtoList = list.Select(r => new ProductReviewDto
        {
            Id = r.Id,
            ProductId = r.ProductId,
            ProductName = r.ProductName,
            ProductImage = r.ProductImage,
            OrderNo = r.OrderNo,
            UserName = r.IsAnonymous ? "匿名用户" : r.UserName,
            UserAvatar = r.IsAnonymous ? null : r.UserAvatar,
            Rating = r.Rating,
            Content = r.Content,
            Images = string.IsNullOrEmpty(r.Images) ? null : JsonSerializer.Deserialize<List<string>>(r.Images),
            Reply = r.Reply,
            ReplyTime = r.ReplyTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            Status = r.Status,
            IsAnonymous = r.IsAnonymous,
            CreateTime = r.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return PageResponse<ProductReviewDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 根据ID获取评价详情
    /// </summary>
    public async Task<ProductReviewDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<ProductReview>()
            .FirstAsync(r => r.Id == id);

        if (entity == null) return null;

        return new ProductReviewDto
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            ProductName = entity.ProductName,
            ProductImage = entity.ProductImage,
            OrderNo = entity.OrderNo,
            UserName = entity.IsAnonymous ? "匿名用户" : entity.UserName,
            UserAvatar = entity.IsAnonymous ? null : entity.UserAvatar,
            Rating = entity.Rating,
            Content = entity.Content,
            Images = string.IsNullOrEmpty(entity.Images) ? null : JsonSerializer.Deserialize<List<string>>(entity.Images),
            Reply = entity.Reply,
            ReplyTime = entity.ReplyTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            Status = entity.Status,
            IsAnonymous = entity.IsAnonymous,
            CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    /// <summary>
    /// 回复评价
    /// </summary>
    public async Task<int> ReplyAsync(ReplyReviewDto dto)
    {
        var entity = await _db.Queryable<ProductReview>().FirstAsync(r => r.Id == dto.Id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("评价不存在");
        }

        entity.Reply = dto.Reply;
        entity.ReplyTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 审核评价
    /// </summary>
    public async Task<int> AuditAsync(Guid id, string status)
    {
        var entity = await _db.Queryable<ProductReview>().FirstAsync(r => r.Id == id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("评价不存在");
        }

        if (status != "approved" && status != "rejected")
        {
            throw new CommonManager.Error.BusinessException("无效的审核状态");
        }

        entity.Status = status;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 隐藏评价
    /// </summary>
    public async Task<int> HideAsync(Guid id)
    {
        var entity = await _db.Queryable<ProductReview>().FirstAsync(r => r.Id == id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("评价不存在");
        }

        entity.Status = "hidden";

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取评价统计
    /// </summary>
    public async Task<ReviewStatisticsDto> GetStatisticsAsync(Guid? productId)
    {
        var queryable = _db.Queryable<ProductReview>()
            .WhereIF(productId.HasValue, r => r.ProductId == productId!.Value);

        var list = await queryable.ToListAsync();

        return new ReviewStatisticsDto
        {
            TotalCount = list.Count,
            AvgRating = list.Count > 0 ? Math.Round((decimal)list.Average(r => r.Rating), 1) : 0,
            FiveStarCount = list.Count(r => r.Rating == 5),
            FourStarCount = list.Count(r => r.Rating == 4),
            ThreeStarCount = list.Count(r => r.Rating == 3),
            TwoStarCount = list.Count(r => r.Rating == 2),
            OneStarCount = list.Count(r => r.Rating == 1),
            PendingCount = list.Count(r => r.Status == "pending"),
            RepliedCount = list.Count(r => !string.IsNullOrEmpty(r.Reply))
        };
    }

    /// <summary>
    /// 删除评价
    /// </summary>
    public async Task<int> DeleteAsync(Guid id)
    {
        var entity = await _db.Queryable<ProductReview>().FirstAsync(r => r.Id == id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("评价不存在");
        }

        return await _db.Deleteable<ProductReview>().Where(r => r.Id == id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 批量审核评价
    /// </summary>
    public async Task<int> BatchAuditAsync(List<Guid> ids, string status)
    {
        if (ids == null || ids.Count == 0)
        {
            return 0;
        }

        if (status != "approved" && status != "rejected")
        {
            throw new CommonManager.Error.BusinessException("无效的审核状态");
        }

        return await _db.Updateable<ProductReview>()
            .Where(r => ids.Contains(r.Id))
            .SetColumns(r => r.Status == status)
            .ExecuteCommandAsync();
    }
}