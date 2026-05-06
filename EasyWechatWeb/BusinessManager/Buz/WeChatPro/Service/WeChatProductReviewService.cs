using BusinessManager.Buz.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using SqlSugar;
using System.Text.Json;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 微信商品评价服务实现
/// </summary>
public class WeChatProductReviewService : BaseService, IWeChatProductReviewService
{
    /// <summary>
    /// 获取商品评价列表
    /// </summary>
    public async Task<PageResponse<WxProductReviewDto>> GetProductReviewsAsync(Guid productId, QueryWxReviewDto query)
    {
        var queryable = _db.Queryable<ProductReview>()
            .Where(r => r.ProductId == productId && r.Status == "approved")
            // 条件筛选 - 使用 WhereIF
            .WhereIF(query.Rating.HasValue, r => r.Rating == query.Rating!.Value)
            .WhereIF(query.HasImage == true, r => r.Images != null && r.Images != "[]")
            .OrderByDescending(r => r.CreateTime);

        // 分页
        var total = new RefAsync<int>();
        var list = await queryable
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 转换 DTO
        var dtoList = list.Select(r => new WxProductReviewDto
        {
            Id = r.Id,
            ProductId = r.ProductId,
            UserName = r.IsAnonymous ? MaskUserName(r.UserName) : r.UserName,
            UserAvatar = r.IsAnonymous ? null : r.UserAvatar,
            Rating = r.Rating,
            Content = r.Content,
            Images = string.IsNullOrEmpty(r.Images) ? null : JsonSerializer.Deserialize<List<string>>(r.Images),
            Reply = r.Reply,
            ReplyTime = r.ReplyTime.HasValue ? new DateTimeOffset(r.ReplyTime.Value).ToUnixTimeMilliseconds() : null,
            IsAnonymous = r.IsAnonymous,
            CreateTime = new DateTimeOffset(r.CreateTime).ToUnixTimeMilliseconds()
        }).ToList();

        return PageResponse<WxProductReviewDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取商品评价统计
    /// </summary>
    public async Task<WxReviewSummaryDto> GetReviewSummaryAsync(Guid productId)
    {
        var reviews = await _db.Queryable<ProductReview>()
            .Where(r => r.ProductId == productId && r.Status == "approved")
            .ToListAsync();

        if (reviews.Count == 0)
        {
            return new WxReviewSummaryDto
            {
                Total = 0,
                AvgRating = 0,
                GoodRate = 0,
                RatingDistribution = new Dictionary<int, int>
                {
                    { 5, 0 }, { 4, 0 }, { 3, 0 }, { 2, 0 }, { 1, 0 }
                }
            };
        }

        var total = reviews.Count;
        var avgRating = Math.Round((decimal)reviews.Average(r => r.Rating), 1);
        var goodCount = reviews.Count(r => r.Rating >= 4);
        var goodRate = Math.Round((decimal)goodCount / total * 100, 1);

        var distribution = reviews
            .GroupBy(r => r.Rating)
            .ToDictionary(g => g.Key, g => g.Count());

        // 确保所有评分都有
        for (int i = 1; i <= 5; i++)
        {
            if (!distribution.ContainsKey(i))
            {
                distribution[i] = 0;
            }
        }

        return new WxReviewSummaryDto
        {
            Total = total,
            AvgRating = avgRating,
            GoodRate = goodRate,
            RatingDistribution = distribution
        };
    }

    /// <summary>
    /// 提交订单评价
    /// </summary>
    public async Task<Guid> SubmitReviewAsync(Guid userId, SubmitReviewDto dto)
    {
        // 验证订单
        var order = await _db.Queryable<Order>()
            .Where(o => o.Id == dto.OrderId && o.UserId == userId)
            .FirstAsync();

        if (order == null)
        {
            throw new BusinessException("订单不存在");
        }

        if (order.Status != 4) // 已完成
        {
            throw new BusinessException("只能评价已完成的订单");
        }

        // 检查是否已评价
        var existsReview = await _db.Queryable<ProductReview>()
            .Where(r => r.OrderId == dto.OrderId && r.ProductId == dto.ProductId)
            .FirstAsync();

        if (existsReview != null)
        {
            throw new BusinessException("该商品已评价");
        }

        // 获取商品信息
        var product = await _db.Queryable<Product>()
            .Where(p => p.Id == dto.ProductId)
            .FirstAsync();

        // 获取用户信息
        var user = await _db.Queryable<WeChatUser>()
            .Where(u => u.Id == userId)
            .FirstAsync();

        // 创建评价
        var review = new ProductReview
        {
            Id = Guid.NewGuid(),
            ProductId = dto.ProductId,
            ProductName = product?.Name ?? "",
            ProductImage = product?.Image,
            OrderId = dto.OrderId,
            OrderNo = order.OrderNo,
            UserId = userId,
            UserName = user?.Nickname ?? "用户",
            UserAvatar = user?.AvatarUrl,
            Rating = dto.Rating,
            Content = dto.Content,
            Images = dto.Images != null ? JsonSerializer.Serialize(dto.Images) : null,
            IsAnonymous = dto.IsAnonymous,
            Status = "approved", // 自动审核通过
            CreateTime = DateTime.Now
        };

        await _db.Insertable(review).ExecuteCommandAsync();

        return review.Id;
    }

    /// <summary>
    /// 用户名脱敏
    /// </summary>
    private string MaskUserName(string userName)
    {
        if (string.IsNullOrEmpty(userName) || userName.Length <= 2)
        {
            return "用户***";
        }
        return userName.Substring(0, 1) + "***" + userName.Substring(userName.Length - 1);
    }
}