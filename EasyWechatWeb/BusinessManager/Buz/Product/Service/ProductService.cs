using BusinessManager.Buz.IService;
using BusinessManager.Buz.AntWorkflow.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Dto.AntWorkflow;
using EasyWeChatModels.Entitys;
using SqlSugar;
using System.Text.Json;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 商品服务实现
/// </summary>
public class ProductService : BaseService, IProductService
{
    /// <summary>业务审核点服务（Autofac 属性注入）</summary>
    public IBusinessAuditPointService _businessAuditPointService { get; set; } = null!;

    /// <summary>工作流运行时服务（Autofac 属性注入）</summary>
    public IAntWorkflowRuntimeService _workflowRuntimeService { get; set; } = null!;
    #region 商品管理

    /// <summary>
    /// 获取商品分页列表
    /// </summary>
    public async Task<PageResponse<ProductDto>> GetPageListAsync(QueryProductDto query)
    {
        var queryable = _db.Queryable<Product>()
            // 条件筛选 - 使用 WhereIF
            .WhereIF(!string.IsNullOrEmpty(query.Name), p => p.Name.Contains(query.Name!))
            .WhereIF(!string.IsNullOrEmpty(query.SkuCode), p => p.SkuCode.Contains(query.SkuCode!))
            .WhereIF(!string.IsNullOrEmpty(query.Keyword), p => p.Name.Contains(query.Keyword!) || p.SkuCode.Contains(query.Keyword!))
            .WhereIF(query.CategoryId.HasValue, p => p.CategoryId == query.CategoryId!.Value)
            .WhereIF(query.IsHot.HasValue, p => p.IsHot == query.IsHot!.Value)
            .WhereIF(query.IsNew.HasValue, p => p.IsNew == query.IsNew!.Value)
            .WhereIF(query.Status.HasValue, p => p.Status == query.Status!.Value)
            .WhereIF(query.MinPrice.HasValue, p => p.Price >= query.MinPrice!.Value)
            .WhereIF(query.MaxPrice.HasValue, p => p.Price <= query.MaxPrice!.Value)
            .OrderByDescending(p => p.CreateTime);

        // 分页
        var total = new RefAsync<int>();
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 获取分类信息
        var categoryIds = list.Select(p => p.CategoryId).Distinct().ToList();
        var categories = await _db.Queryable<Category>()
            .Where(c => categoryIds.Contains(c.Id))
            .ToListAsync();

        // 转换 DTO
        var dtoList = list.Select(p => new ProductDto
        {
            Id = p.Id,
            SkuCode = p.SkuCode,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            OriginalPrice = p.OriginalPrice,
            Image = p.Image,
            Images = string.IsNullOrEmpty(p.Images) ? null : JsonSerializer.Deserialize<List<string>>(p.Images),
            CategoryId = p.CategoryId,
            CategoryName = categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name,
            Stock = p.Stock,
            AlertThreshold = p.AlertThreshold,
            Sales = p.Sales,
            IsHot = p.IsHot,
            IsNew = p.IsNew,
            Detail = p.Detail,
            Status = p.Status,
            CreateTime = p.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = p.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            AuditStatus = p.AuditStatus,
            WorkflowInstanceId = p.WorkflowInstanceId,
            AuditPointCode = p.AuditPointCode,
            AuditTime = p.AuditTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            AuditorId = p.AuditorId,
            AuditRemark = p.AuditRemark
        }).ToList();

        return PageResponse<ProductDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 根据ID获取商品详情
    /// </summary>
    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<Product>()
            .FirstAsync(p => p.Id == id);

        if (entity == null) return null;

        var category = await _db.Queryable<Category>()
            .FirstAsync(c => c.Id == entity.CategoryId);

        return new ProductDto
        {
            Id = entity.Id,
            SkuCode = entity.SkuCode,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            OriginalPrice = entity.OriginalPrice,
            Image = entity.Image,
            Images = string.IsNullOrEmpty(entity.Images) ? null : JsonSerializer.Deserialize<List<string>>(entity.Images),
            CategoryId = entity.CategoryId,
            CategoryName = category?.Name,
            Stock = entity.Stock,
            AlertThreshold = entity.AlertThreshold,
            Sales = entity.Sales,
            IsHot = entity.IsHot,
            IsNew = entity.IsNew,
            Detail = entity.Detail,
            Status = entity.Status,
            CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = entity.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            AuditStatus = entity.AuditStatus,
            WorkflowInstanceId = entity.WorkflowInstanceId,
            AuditPointCode = entity.AuditPointCode,
            AuditTime = entity.AuditTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            AuditorId = entity.AuditorId,
            AuditRemark = entity.AuditRemark
        };
    }

    /// <summary>
    /// 添加商品
    /// </summary>
    public async Task<Guid> AddAsync(AddProductDto dto)
    {
        // 检查SKU码是否已存在
        if (!string.IsNullOrEmpty(dto.SkuCode))
        {
            var exists = await _db.Queryable<Product>().AnyAsync(p => p.SkuCode == dto.SkuCode);
            if (exists)
            {
                throw new CommonManager.Error.BusinessException($"SKU码 {dto.SkuCode} 已存在");
            }
        }

        var entity = new Product
        {
            Id = Guid.NewGuid(),
            SkuCode = dto.SkuCode,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            OriginalPrice = dto.OriginalPrice,
            Image = dto.Image,
            Images = dto.Images != null ? JsonSerializer.Serialize(dto.Images) : null,
            CategoryId = dto.CategoryId,
            Stock = dto.Stock,
            AlertThreshold = dto.AlertThreshold,
            IsHot = dto.IsHot,
            IsNew = dto.IsNew,
            Detail = dto.Detail,
            Status = 1,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <summary>
    /// 更新商品
    /// </summary>
    public async Task<int> UpdateAsync(UpdateProductDto dto)
    {
        var entity = await _db.Queryable<Product>().FirstAsync(p => p.Id == dto.Id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("商品不存在");
        }

        // 检查SKU码是否被其他商品使用
        if (!string.IsNullOrEmpty(dto.SkuCode) && dto.SkuCode != entity.SkuCode)
        {
            var exists = await _db.Queryable<Product>().AnyAsync(p => p.SkuCode == dto.SkuCode && p.Id != dto.Id);
            if (exists)
            {
                throw new CommonManager.Error.BusinessException($"SKU码 {dto.SkuCode} 已被其他商品使用");
            }
        }

        // 更新字段
        entity.SkuCode = dto.SkuCode;
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Price = dto.Price;
        entity.OriginalPrice = dto.OriginalPrice;
        entity.Image = dto.Image;
        entity.Images = dto.Images != null ? JsonSerializer.Serialize(dto.Images) : null;
        entity.CategoryId = dto.CategoryId;
        entity.AlertThreshold = dto.AlertThreshold;
        entity.IsHot = dto.IsHot;
        entity.IsNew = dto.IsNew;
        entity.Detail = dto.Detail;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除商品
    /// </summary>
    public async Task<int> DeleteAsync(Guid id)
    {
        // 删除关联的库存记录
        await _db.Deleteable<StockRecord>().Where(r => r.ProductId == id).ExecuteCommandAsync();

        // 删除关联的商品供应商
        await _db.Deleteable<ProductSupplier>().Where(ps => ps.ProductId == id).ExecuteCommandAsync();

        // 删除商品
        return await _db.Deleteable<Product>().Where(p => p.Id == id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 批量删除商品
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

    /// <summary>
    /// 批量更新商品状态
    /// </summary>
    public async Task<int> BatchUpdateStatusAsync(List<Guid> ids, int status)
    {
        return await _db.Updateable<Product>()
            .Where(p => ids.Contains(p.Id))
            .SetColumns(p => p.Status == status)
            .SetColumns(p => p.UpdateTime == DateTime.Now)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 批量更新商品分类
    /// </summary>
    public async Task<int> BatchUpdateCategoryAsync(List<Guid> ids, Guid categoryId)
    {
        return await _db.Updateable<Product>()
            .Where(p => ids.Contains(p.Id))
            .SetColumns(p => p.CategoryId == categoryId)
            .SetColumns(p => p.UpdateTime == DateTime.Now)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 批量更新商品标签
    /// </summary>
    public async Task<int> BatchUpdateTagAsync(List<Guid> ids, bool? isHot, bool? isNew)
    {
        var updateable = _db.Updateable<Product>()
            .Where(p => ids.Contains(p.Id))
            .SetColumns(p => p.UpdateTime == DateTime.Now);

        if (isHot.HasValue)
        {
            updateable = updateable.SetColumns(p => p.IsHot == isHot.Value);
        }

        if (isNew.HasValue)
        {
            updateable = updateable.SetColumns(p => p.IsNew == isNew.Value);
        }

        return await updateable.ExecuteCommandAsync();
    }

    /// <summary>
    /// 批量添加商品
    /// </summary>
    public async Task<List<Guid>> BatchAddAsync(List<AddProductDto> products)
    {
        var ids = new List<Guid>();
        foreach (var dto in products)
        {
            var id = await AddAsync(dto);
            ids.Add(id);
        }
        return ids;
    }

    #endregion

    #region 分类管理

    /// <summary>
    /// 获取分类树形列表
    /// </summary>
    public async Task<List<CategoryDto>> GetCategoryTreeAsync()
    {
        var list = await _db.Queryable<Category>()
            .Where(c => c.Status == 1)
            .OrderBy(c => c.Sort)
            .ToListAsync();

        // 转换 DTO
        var dtoList = list.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Icon = c.Icon,
            Sort = c.Sort,
            Description = c.Description,
            Status = c.Status,
            CreateTime = c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            ParentId = c.ParentId
        }).ToList();

        // 构建树形结构
        return BuildCategoryTree(dtoList, null);
    }

    /// <summary>
    /// 构建分类树形结构
    /// </summary>
    private List<CategoryDto> BuildCategoryTree(List<CategoryDto> all, Guid? parentId)
    {
        return all.Where(c => c.ParentId == parentId)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Icon = c.Icon,
                Sort = c.Sort,
                Description = c.Description,
                Status = c.Status,
                CreateTime = c.CreateTime,
                Children = BuildCategoryTree(all, c.Id)
            })
            .ToList();
    }

    /// <summary>
    /// 添加分类
    /// </summary>
    public async Task<Guid> AddCategoryAsync(AddCategoryDto dto)
    {
        var entity = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Icon = dto.Icon,
            Sort = dto.Sort,
            Description = dto.Description,
            Status = 1,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <summary>
    /// 更新分类
    /// </summary>
    public async Task<int> UpdateCategoryAsync(UpdateCategoryDto dto)
    {
        var entity = await _db.Queryable<Category>().FirstAsync(c => c.Id == dto.Id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("分类不存在");
        }

        entity.Name = dto.Name;
        entity.Icon = dto.Icon;
        entity.Sort = dto.Sort;
        entity.Description = dto.Description;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除分类
    /// </summary>
    public async Task<int> DeleteCategoryAsync(Guid id)
    {
        // 检查是否有子分类
        var hasChildren = await _db.Queryable<Category>().AnyAsync(c => c.ParentId == id);
        if (hasChildren)
        {
            throw new CommonManager.Error.BusinessException("该分类下存在子分类，无法删除");
        }

        // 检查是否有商品
        var hasProducts = await _db.Queryable<Product>().AnyAsync(p => p.CategoryId == id);
        if (hasProducts)
        {
            throw new CommonManager.Error.BusinessException("该分类下存在商品，无法删除");
        }

        return await _db.Deleteable<Category>().Where(c => c.Id == id).ExecuteCommandAsync();
    }

    #endregion

    #region 库存管理

    /// <summary>
    /// 获取库存分页列表
    /// </summary>
    public async Task<PageResponse<StockDto>> GetStockListAsync(QueryStockDto query)
    {
        var queryable = _db.Queryable<Product>()
            // 条件筛选 - 使用 WhereIF
            .WhereIF(!string.IsNullOrEmpty(query.Name), p => p.Name.Contains(query.Name!))
            .WhereIF(query.CategoryId.HasValue, p => p.CategoryId == query.CategoryId!.Value)
            .OrderBy(p => p.Stock);

        // 分页
        var total = new RefAsync<int>();
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 获取分类信息
        var categoryIds = list.Select(p => p.CategoryId).Distinct().ToList();
        var categories = await _db.Queryable<Category>()
            .Where(c => categoryIds.Contains(c.Id))
            .ToListAsync();

        // 转换 DTO
        var dtoList = list.Select(p => new StockDto
        {
            ProductId = p.Id,
            SkuCode = p.SkuCode,
            ProductName = p.Name,
            ProductImage = p.Image,
            CategoryName = categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name,
            Stock = p.Stock,
            AlertThreshold = p.AlertThreshold,
            IsLowStock = p.Stock <= p.AlertThreshold,
            UpdateTime = p.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? p.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        // 低库存筛选
        if (query.LowStock == true)
        {
            dtoList = dtoList.Where(s => s.IsLowStock).ToList();
        }

        return PageResponse<StockDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 库存调整
    /// </summary>
    public async Task<int> AdjustStockAsync(StockAdjustDto dto)
    {
        // 获取商品信息
        var product = await _db.Queryable<Product>().FirstAsync(p => p.Id == dto.ProductId);
        if (product == null)
        {
            throw new CommonManager.Error.BusinessException("商品不存在");
        }

        var beforeStock = product.Stock;
        var afterStock = beforeStock;

        // 计算变动后库存
        switch (dto.Type)
        {
            case "in":
                // 入库：在原有库存上增加变动数量
                afterStock = beforeStock + dto.Quantity;
                break;
            case "out":
                // 出库：在原有库存上减少变动数量
                afterStock = beforeStock - dto.Quantity;
                if (afterStock < 0)
                {
                    throw new CommonManager.Error.BusinessException("库存不足，无法出库");
                }
                break;
            case "adjust":
                // 调整：在原有库存上调整数量（可正可负）
                // dto.Quantity 为正数时增加库存，为负数时减少库存
                afterStock = beforeStock + dto.Quantity;
                if (afterStock < 0)
                {
                    throw new CommonManager.Error.BusinessException("调整后库存不能为负数");
                }
                break;
            default:
                throw new CommonManager.Error.BusinessException("无效的调整类型");
        }

        // 获取供应商信息
        string? supplierName = null;
        if (dto.SupplierId.HasValue)
        {
            var supplier = await _db.Queryable<Supplier>().FirstAsync(s => s.Id == dto.SupplierId.Value);
            supplierName = supplier?.Name;
        }

        // 更新商品库存
        product.Stock = afterStock;
        product.UpdateTime = DateTime.Now;
        await _db.Updateable(product).ExecuteCommandAsync();

        // 创建库存变动记录
        var record = new StockRecord
        {
            Id = Guid.NewGuid(),
            ProductId = dto.ProductId,
            SkuCode = product.SkuCode,
            ProductName = product.Name,
            //ProductImage = product.Image,
            Type = dto.Type,
            Quantity = Math.Abs(afterStock - beforeStock),
            BeforeStock = beforeStock,
            AfterStock = afterStock,
            SupplierId = dto.SupplierId,
            SupplierName = supplierName,
            PurchasePrice = dto.PurchasePrice,
            Remark = dto.Remark,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(record).ExecuteCommandAsync();
        return 1;
    }

    /// <summary>
    /// 获取库存变动记录分页列表
    /// </summary>
    public async Task<PageResponse<StockRecordDto>> GetStockRecordListAsync(QueryStockRecordDto query)
    {
        var queryable = _db.Queryable<StockRecord>()
            // 条件筛选 - 使用 WhereIF
            .WhereIF(query.ProductId.HasValue, r => r.ProductId == query.ProductId!.Value)
            .WhereIF(!string.IsNullOrEmpty(query.ProductName), r => r.ProductName.Contains(query.ProductName!))
            .WhereIF(!string.IsNullOrEmpty(query.Type), r => r.Type == query.Type)
            .WhereIF(!string.IsNullOrEmpty(query.StartDate), r => r.CreateTime >= DateTime.Parse(query.StartDate!))
            .WhereIF(!string.IsNullOrEmpty(query.EndDate), r => r.CreateTime < DateTime.Parse(query.EndDate!).AddDays(1))
            .OrderByDescending(r => r.CreateTime);

        // 分页
        var total = new RefAsync<int>();
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 转换 DTO
        var dtoList = list.Select(r => new StockRecordDto
        {
            Id = r.Id,
            ProductId = r.ProductId,
            SkuCode = r.SkuCode,
            ProductName = r.ProductName,
            ProductImage = r.ProductImage,
            Type = r.Type,
            Quantity = r.Quantity,
            BeforeStock = r.BeforeStock,
            AfterStock = r.AfterStock,
            SupplierName = r.SupplierName,
            PurchasePrice = r.PurchasePrice,
            Operator = r.Operator,
            Remark = r.Remark,
            CreateTime = r.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return PageResponse<StockRecordDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取低库存预警列表
    /// </summary>
    public async Task<List<StockDto>> GetLowStockListAsync()
    {
        var list = await _db.Queryable<Product>()
            .Where(p => p.Stock <= p.AlertThreshold && p.Status == 1)
            .OrderBy(p => p.Stock)
            .ToListAsync();

        // 获取分类信息
        var categoryIds = list.Select(p => p.CategoryId).Distinct().ToList();
        var categories = await _db.Queryable<Category>()
            .Where(c => categoryIds.Contains(c.Id))
            .ToListAsync();

        return list.Select(p => new StockDto
        {
            ProductId = p.Id,
            SkuCode = p.SkuCode,
            ProductName = p.Name,
            ProductImage = p.Image,
            CategoryName = categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name,
            Stock = p.Stock,
            AlertThreshold = p.AlertThreshold,
            IsLowStock = true,
            UpdateTime = p.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? p.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();
    }

    #endregion

    #region 统计报表

    /// <summary>
    /// 获取商品销量统计
    /// </summary>
    public async Task<List<ProductSalesStatsDto>> GetSalesStatsAsync(ProductStatsQueryDto query)
    {
        // 简化实现：返回商品销量排名
        var top = query.Top ?? 10;

        var list = await _db.Queryable<Product>()
            .Where(p => p.Status == 1)
            .OrderByDescending(p => p.Sales)
            .Take(top)
            .ToListAsync();

        return list.Select(p => new ProductSalesStatsDto
        {
            ProductId = p.Id,
            ProductName = p.Name,
            ProductImage = p.Image,
            SkuCode = p.SkuCode,
            SalesCount = p.Sales,
            SalesAmount = p.Sales * p.Price,
            OrderCount = 0, // 需要关联订单表计算
            AvgPrice = p.Price
        }).ToList();
    }

    /// <summary>
    /// 获取销量趋势
    /// </summary>
    public async Task<List<SalesTrendDto>> GetSalesTrendAsync(ProductStatsQueryDto query)
    {
        // 简化实现：返回最近7天的趋势
        var trend = new List<SalesTrendDto>();
        for (int i = 6; i >= 0; i--)
        {
            var date = DateTime.Now.AddDays(-i);
            trend.Add(new SalesTrendDto
            {
                Date = date.ToString("yyyy-MM-dd"),
                SalesCount = new Random().Next(10, 100),
                SalesAmount = new Random().Next(1000, 10000),
                OrderCount = new Random().Next(5, 50)
            });
        }
        return await Task.FromResult(trend);
    }

    /// <summary>
    /// 获取库存统计
    /// </summary>
    public async Task<StockStatisticsDto> GetStockStatisticsAsync()
    {
        var products = await _db.Queryable<Product>()
            .Where(p => p.Status == 1)
            .ToListAsync();

        return new StockStatisticsDto
        {
            TotalProducts = products.Count,
            TotalStock = products.Sum(p => p.Stock),
            LowStockCount = products.Count(p => p.Stock <= p.AlertThreshold && p.Stock > 0),
            OutOfStockCount = products.Count(p => p.Stock == 0),
            TotalValue = products.Sum(p => p.Stock * p.Price)
        };
    }

    /// <summary>
    /// 获取商品概览统计
    /// </summary>
    public async Task<ProductOverviewStatsDto> GetOverviewStatsAsync()
    {
        var today = DateTime.Today;
        var yesterday = today.AddDays(-1);

        // 获取今日销售数据（简化实现）
        var products = await _db.Queryable<Product>().Where(p => p.Status == 1).ToListAsync();

        return new ProductOverviewStatsDto
        {
            Today = new TodayStatsDto
            {
                SalesCount = products.Sum(p => p.Sales),
                SalesAmount = products.Sum(p => p.Sales * p.Price),
                OrderCount = 0 // 需要关联订单表计算
            },
            Growth = new GrowthStatsDto
            {
                SalesCountGrowth = 0,
                SalesAmountGrowth = 0,
                OrderCountGrowth = 0
            }
        };
    }

    /// <summary>
    /// 获取销量排行
    /// </summary>
    public async Task<List<ProductSalesStatsDto>> GetSalesRankingAsync(int limit = 10, string sortBy = "sales")
    {
        var queryable = _db.Queryable<Product>().Where(p => p.Status == 1);

        if (sortBy == "amount")
        {
            queryable = queryable.OrderByDescending(p => p.Sales * p.Price);
        }
        else
        {
            queryable = queryable.OrderByDescending(p => p.Sales);
        }

        var list = await queryable.Take(limit).ToListAsync();

        return list.Select(p => new ProductSalesStatsDto
        {
            ProductId = p.Id,
            ProductName = p.Name,
            ProductImage = p.Image,
            SkuCode = p.SkuCode,
            SalesCount = p.Sales,
            SalesAmount = p.Sales * p.Price,
            OrderCount = 0,
            AvgPrice = p.Price
        }).ToList();
    }

    /// <summary>
    /// 获取分类销量统计
    /// </summary>
    public async Task<List<CategorySalesStatsDto>> GetCategorySalesStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var products = await _db.Queryable<Product>().Where(p => p.Status == 1).ToListAsync();
        var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();
        var categories = await _db.Queryable<Category>().Where(c => categoryIds.Contains(c.Id)).ToListAsync();

        return products.GroupBy(p => p.CategoryId)
            .Select(g => new CategorySalesStatsDto
            {
                CategoryName = categories.FirstOrDefault(c => c.Id == g.Key)?.Name ?? "未分类",
                SalesCount = g.Sum(p => p.Sales),
                SalesAmount = g.Sum(p => p.Sales * p.Price)
            })
            .OrderByDescending(s => s.SalesAmount)
            .ToList();
    }

    /// <summary>
    /// 获取分类详情
    /// </summary>
    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<Category>().FirstAsync(c => c.Id == id);
        if (entity == null) return null;

        return new CategoryDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Icon = entity.Icon,
            Sort = entity.Sort,
            Description = entity.Description,
            Status = entity.Status,
            CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            ParentId = entity.ParentId
        };
    }

    /// <summary>
    /// 库存入库
    /// </summary>
    public async Task<int> StockInAsync(StockInDto dto)
    {
        var adjustDto = new StockAdjustDto
        {
            ProductId = dto.ProductId,
            Type = "in",
            Quantity = dto.Quantity,
            Remark = dto.Remark
        };
        return await AdjustStockAsync(adjustDto);
    }

    #endregion

    #region 审核管理

    /// <summary>
    /// 提交审核
    /// </summary>
    public async Task<Guid> SubmitAuditAsync(SubmitAuditDto dto, Guid userId, string userName)
    {
        // 1. 获取商品
        var product = await _db.Queryable<Product>().FirstAsync(p => p.Id == dto.ProductId);
        if (product == null)
        {
            throw new BusinessException("商品不存在");
        }

        // 2. 检查状态（待提交、已驳回、已撤回状态都可以重新提交审核）
        // 状态说明：0=待提交, 1=待审核, 2=已驳回, 3=已通过, 4=已撤回
        if (product.AuditStatus != 0 && product.AuditStatus != 2 && product.AuditStatus != 4)
        {
            var statusDesc = product.AuditStatus == 1 ? "待审核" : product.AuditStatus == 3 ? "已通过" : $"状态{product.AuditStatus}";
            throw new BusinessException($"商品当前审核状态为{statusDesc}，无法提交审核。只有待提交、已驳回或已撤回状态的商品才能提交审核");
        }

        // 3. 获取审核点配置
        var auditPoint = await _businessAuditPointService.GetByCodeAsync(dto.AuditPointCode);
        if (auditPoint == null)
        {
            throw new BusinessException($"审核点配置 '{dto.AuditPointCode}' 不存在");
        }

        if (auditPoint.Status != 1)
        {
            throw new BusinessException($"审核点配置 '{dto.AuditPointCode}' 已禁用");
        }

        // 4. 渲染审核标题
        var title = auditPoint.TitleTemplate ?? $"{product.Name}审核申请";
        if (!string.IsNullOrEmpty(auditPoint.CodeField))
        {
            // 替换模板中的字段占位符
            title = title.Replace($"{{{auditPoint.CodeField}}}", product.SkuCode);
        }
        title = title.Replace("{Name}", product.Name);

        // 5. 创建工作流实例
        var startDto = new StartAntWorkflowDto
        {
            WorkflowId = auditPoint.WorkflowId,
            Title = title,
            BusinessType = auditPoint.TableName,  // 使用表名作为业务类型，如 "Product"
            BusinessId = product.Id.ToString(),
            BusinessData = JsonSerializer.Serialize(new { ProductId = product.Id, ProductName = product.Name, SkuCode = product.SkuCode })
        };

        var instanceId = await _workflowRuntimeService.StartAsync(startDto, userId, userName);

        // 6. 更新商品审核状态
        product.AuditStatus = auditPoint.AuditStatusValue;
        product.WorkflowInstanceId = instanceId;
        product.AuditPointCode = auditPoint.Code;
        product.AuditTime = DateTime.Now;
        product.UpdateTime = DateTime.Now;

        await _db.Updateable(product).ExecuteCommandAsync();

        return instanceId;
    }

    /// <summary>
    /// 撤回审核
    /// </summary>
    public async Task<int> WithdrawAuditAsync(Guid productId, Guid userId, string? reason = null)
    {
        // 1. 获取商品
        var product = await _db.Queryable<Product>().FirstAsync(p => p.Id == productId);
        if (product == null)
        {
            throw new BusinessException("商品不存在");
        }

        // 2. 检查状态（只有待审核状态才能撤回）
        if (product.AuditStatus != 1)
        {
            throw new BusinessException($"商品当前审核状态为{product.AuditStatus}，无法撤回。只有待审核状态的商品才能撤回");
        }

        // 3. 获取审核点配置
        var auditPoint = await _businessAuditPointService.GetByCodeAsync(product.AuditPointCode!);
        if (auditPoint == null)
        {
            throw new BusinessException("审核点配置不存在");
        }

        // 4. 终止工作流实例
        if (product.WorkflowInstanceId.HasValue)
        {
            await _workflowRuntimeService.CancelAsync(product.WorkflowInstanceId.Value, userId, reason);
        }

        // 5. 更新商品状态
        product.AuditStatus = auditPoint.WithdrawStatusValue;
        product.WorkflowInstanceId = null;
        product.AuditTime = DateTime.Now;
        product.UpdateTime = DateTime.Now;

        return await _db.Updateable(product).ExecuteCommandAsync();
    }

    /// <summary>
    /// 审核通过
    /// </summary>
    public async Task<int> AuditPassAsync(Guid productId, Guid auditorId)
    {
        // 1. 获取商品
        var product = await _db.Queryable<Product>().FirstAsync(p => p.Id == productId);
        if (product == null)
        {
            throw new BusinessException("商品不存在");
        }

        // 2. 获取审核点配置
        var auditPoint = await _businessAuditPointService.GetByCodeAsync(product.AuditPointCode!);
        if (auditPoint == null)
        {
            throw new BusinessException("审核点配置不存在");
        }

        // 3. 更新商品为已通过并上架
        product.AuditStatus = auditPoint.PassStatusValue;
        product.Status = 1; // 上架
        product.AuditorId = auditorId;
        product.AuditTime = DateTime.Now;
        product.UpdateTime = DateTime.Now;

        return await _db.Updateable(product).ExecuteCommandAsync();
    }

    /// <summary>
    /// 审核驳回
    /// </summary>
    public async Task<int> AuditRejectAsync(Guid productId, Guid auditorId, AuditRejectDto dto)
    {
        // 1. 获取商品
        var product = await _db.Queryable<Product>().FirstAsync(p => p.Id == productId);
        if (product == null)
        {
            throw new BusinessException("商品不存在");
        }

        // 2. 获取审核点配置
        var auditPoint = await _businessAuditPointService.GetByCodeAsync(product.AuditPointCode!);
        if (auditPoint == null)
        {
            throw new BusinessException("审核点配置不存在");
        }

        // 3. 更新商品为已驳回并记录驳回原因
        product.AuditStatus = auditPoint.RejectStatusValue;
        product.AuditorId = auditorId;
        product.AuditRemark = dto.RejectReason;
        product.AuditTime = DateTime.Now;
        product.UpdateTime = DateTime.Now;

        return await _db.Updateable(product).ExecuteCommandAsync();
    }

    #endregion
}