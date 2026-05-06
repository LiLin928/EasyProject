using BusinessManager.Buz.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 微信商品服务实现类
/// </summary>
/// <remarks>
/// 实现商品相关的业务逻辑，包括商品查询、分类查询、商品搜索等功能。
/// 继承自<see cref="BaseService{Product}"/>，使用SqlSugar进行数据库操作。
/// </remarks>
public class WeChatProductService : BaseService<Product>, IWeChatProductService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatProductService> _logger { get; set; } = null!;

    /// <summary>
    /// 获取商品列表（分页）
    /// </summary>
    /// <param name="query">查询参数，包含分页、筛选、排序条件</param>
    /// <returns>商品分页列表</returns>
    /// <remarks>
    /// 支持以下筛选条件：
    /// <list type="bullet">
    ///     <item>分类筛选：CategoryId</item>
    ///     <item>关键字搜索：Keyword</item>
    ///     <item>热销筛选：IsHot</item>
    ///     <item>新品筛选：IsNew</item>
    ///     <item>价格区间：MinPrice、MaxPrice</item>
    /// </list>
    /// 支持排序字段：Price、Sales、CreateTime
    /// </remarks>
    public async Task<PageResponse<ProductDto>> GetProductListAsync(QueryProductDto query)
    {
        var total = new RefAsync<int>();
        var queryable = _db.Queryable<Product>()
            .Where(p => p.Status == 1) // 只查询上架商品
            .WhereIF(query.CategoryId.HasValue, p => p.CategoryId == query.CategoryId!.Value)
            .WhereIF(!string.IsNullOrEmpty(query.Keyword), p => p.Name.Contains(query.Keyword!))
            .WhereIF(query.IsHot.HasValue, p => p.IsHot == query.IsHot!.Value)
            .WhereIF(query.IsNew.HasValue, p => p.IsNew == query.IsNew!.Value)
            .WhereIF(query.MinPrice.HasValue, p => p.Price >= query.MinPrice!.Value)
            .WhereIF(query.MaxPrice.HasValue, p => p.Price <= query.MaxPrice!.Value);

        // 排序处理
        queryable = ApplySorting(queryable, query.SortField, query.SortOrder);

        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 手动映射，处理 Images 字段的 JSON 转换
        var productDtos = list.Select(p => {
            var dto = new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                SkuCode = p.SkuCode,
                Description = p.Description,
                Price = p.Price,
                OriginalPrice = p.OriginalPrice,
                Image = p.Image,
                CategoryId = p.CategoryId,
                Stock = p.Stock,
                AlertThreshold = p.AlertThreshold,
                Sales = p.Sales,
                IsHot = p.IsHot,
                IsNew = p.IsNew,
                Detail = p.Detail,
                Status = p.Status,
                CreateTime = p.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdateTime = p.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
            };
            // 解析详情图列表
            if (!string.IsNullOrEmpty(p.Images))
            {
                try
                {
                    dto.Images = System.Text.Json.JsonSerializer.Deserialize<List<string>>(p.Images);
                }
                catch
                {
                    dto.Images = new List<string>();
                }
            }
            return dto;
        }).ToList();

        // 加载分类信息
        await LoadCategoriesAsync(productDtos);

        return PageResponse<ProductDto>.Create(productDtos, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取商品详情
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <returns>商品详细信息；商品不存在或已下架返回null</returns>
    /// <remarks>
    /// 只返回上架状态的商品详情。
    /// 包含商品基本信息、分类信息、详情图列表。
    /// </remarks>
    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        var product = await GetFirstAsync(p => p.Id == id && p.Status == 1);
        if (product == null)
        {
            return null;
        }

        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            SkuCode = product.SkuCode,
            Description = product.Description,
            Price = product.Price,
            OriginalPrice = product.OriginalPrice,
            Image = product.Image,
            CategoryId = product.CategoryId,
            Stock = product.Stock,
            AlertThreshold = product.AlertThreshold,
            Sales = product.Sales,
            IsHot = product.IsHot,
            IsNew = product.IsNew,
            Detail = product.Detail,
            Status = product.Status,
            CreateTime = product.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = product.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
        };

        // 加载分类信息
        var category = await _db.Queryable<Category>()
            .Where(c => c.Id == product.CategoryId)
            .FirstAsync();
        if (category != null)
        {
            dto.Category = category.Adapt<CategoryDto>();
        }

        // 解析详情图列表
        if (!string.IsNullOrEmpty(product.Images))
        {
            try
            {
                dto.Images = System.Text.Json.JsonSerializer.Deserialize<List<string>>(product.Images);
            }
            catch
            {
                dto.Images = new List<string>();
            }
        }

        return dto;
    }

    /// <summary>
    /// 获取分类列表
    /// </summary>
    /// <returns>所有启用的分类列表，按排序升序排列</returns>
    /// <remarks>
    /// 只返回状态为启用（Status=1）的分类。
    /// 结果按Sort字段升序排列。
    /// </remarks>
    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        var categories = await _db.Queryable<Category>()
            .Where(c => c.Status == 1)
            .OrderBy(c => c.Sort)
            .ToListAsync();

        return categories.Adapt<List<CategoryDto>>();
    }

    /// <summary>
    /// 搜索商品
    /// </summary>
    /// <param name="keyword">搜索关键字</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <returns>商品分页列表</returns>
    /// <remarks>
    /// 在商品名称和描述中搜索关键字。
    /// 只搜索上架状态的商品。
    /// 结果按创建时间倒序排列。
    /// </remarks>
    public async Task<PageResponse<ProductDto>> SearchProductsAsync(string keyword, int pageIndex, int pageSize)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            return PageResponse<ProductDto>.Empty(pageIndex, pageSize);
        }

        var total = new RefAsync<int>();
        var list = await _db.Queryable<Product>()
            .Where(p => p.Status == 1)
            .Where(p => p.Name.Contains(keyword) || (p.Description != null && p.Description.Contains(keyword)))
            .OrderByDescending(p => p.CreateTime)
            .ToPageListAsync(pageIndex, pageSize, total);

        // 手动映射，处理 Images 字段的 JSON 转换
        var productDtos = list.Select(p => {
            var dto = new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                SkuCode = p.SkuCode,
                Description = p.Description,
                Price = p.Price,
                OriginalPrice = p.OriginalPrice,
                Image = p.Image,
                CategoryId = p.CategoryId,
                Stock = p.Stock,
                AlertThreshold = p.AlertThreshold,
                Sales = p.Sales,
                IsHot = p.IsHot,
                IsNew = p.IsNew,
                Detail = p.Detail,
                Status = p.Status,
                CreateTime = p.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdateTime = p.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
            };
            // 解析详情图列表
            if (!string.IsNullOrEmpty(p.Images))
            {
                try
                {
                    dto.Images = System.Text.Json.JsonSerializer.Deserialize<List<string>>(p.Images);
                }
                catch
                {
                    dto.Images = new List<string>();
                }
            }
            return dto;
        }).ToList();

        await LoadCategoriesAsync(productDtos);

        return PageResponse<ProductDto>.Create(productDtos, total.Value, pageIndex, pageSize);
    }

    /// <summary>
    /// 应用排序
    /// </summary>
    /// <param name="query">查询表达式</param>
    /// <param name="sortField">排序字段</param>
    /// <param name="sortOrder">排序方式</param>
    /// <returns>排序后的查询表达式</returns>
    private ISugarQueryable<Product> ApplySorting(ISugarQueryable<Product> query, string? sortField, string? sortOrder)
    {
        if (string.IsNullOrEmpty(sortField))
        {
            return query.OrderByDescending(p => p.CreateTime);
        }

        var isAsc = sortOrder?.ToLower() == "asc";

        return sortField.ToLower() switch
        {
            "price" => isAsc ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
            "sales" => isAsc ? query.OrderBy(p => p.Sales) : query.OrderByDescending(p => p.Sales),
            "createtime" => isAsc ? query.OrderBy(p => p.CreateTime) : query.OrderByDescending(p => p.CreateTime),
            _ => query.OrderByDescending(p => p.CreateTime)
        };
    }

    /// <summary>
    /// 加载分类信息
    /// </summary>
    /// <param name="products">商品DTO列表</param>
    private async Task LoadCategoriesAsync(List<ProductDto> products)
    {
        if (products.Count == 0) return;

        var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();
        var categories = await _db.Queryable<Category>()
            .Where(c => categoryIds.Contains(c.Id))
            .ToListAsync();

        var categoryDict = categories.ToDictionary(c => c.Id);
        foreach (var product in products)
        {
            if (categoryDict.TryGetValue(product.CategoryId, out var category))
            {
                product.Category = category.Adapt<CategoryDto>();
            }
        }
    }
}