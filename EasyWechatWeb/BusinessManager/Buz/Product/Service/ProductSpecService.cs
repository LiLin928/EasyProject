using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models.Product;
using SqlSugar;
using System.Text.Json;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 商品规格服务实现
/// </summary>
public class ProductSpecService : BaseService, IProductSpecService
{
    #region 规格管理

    /// <summary>
    /// 获取商品规格列表
    /// </summary>
    public async Task<List<ProductSpecDto>> GetSpecListAsync(Guid productId)
    {
        var specs = await _db.Queryable<ProductSpec>()
            .Where(s => s.ProductId == productId)
            .OrderBy(s => s.Sort)
            .ToListAsync();

        var specIds = specs.Select(s => s.Id).ToList();
        var options = await _db.Queryable<ProductSpecOption>()
            .Where(o => specIds.Contains(o.SpecId))
            .OrderBy(o => o.Sort)
            .ToListAsync();

        return specs.Select(s => new ProductSpecDto
        {
            Id = s.Id,
            ProductId = s.ProductId,
            Name = s.Name,
            Required = s.Required,
            Sort = s.Sort,
            Options = options.Where(o => o.SpecId == s.Id).Select(o => new ProductSpecOptionDto
            {
                Id = o.Id,
                SpecId = o.SpecId,
                Name = o.Name,
                Value = o.Value,
                PriceAdjust = o.PriceAdjust,
                Stock = o.Stock,
                Sort = o.Sort
            }).ToList()
        }).ToList();
    }

    /// <summary>
    /// 获取规格详情
    /// </summary>
    public async Task<ProductSpecDto?> GetSpecByIdAsync(Guid id)
    {
        var spec = await _db.Queryable<ProductSpec>().FirstAsync(s => s.Id == id);
        if (spec == null) return null;

        var options = await _db.Queryable<ProductSpecOption>()
            .Where(o => o.SpecId == id)
            .OrderBy(o => o.Sort)
            .ToListAsync();

        return new ProductSpecDto
        {
            Id = spec.Id,
            ProductId = spec.ProductId,
            Name = spec.Name,
            Required = spec.Required,
            Sort = spec.Sort,
            Options = options.Select(o => new ProductSpecOptionDto
            {
                Id = o.Id,
                SpecId = o.SpecId,
                Name = o.Name,
                Value = o.Value,
                PriceAdjust = o.PriceAdjust,
                Stock = o.Stock,
                Sort = o.Sort
            }).ToList()
        };
    }

    /// <summary>
    /// 创建规格
    /// </summary>
    public async Task<Guid> CreateSpecAsync(AddProductSpecDto dto)
    {
        // 检查商品是否存在
        var product = await _db.Queryable<Product>().FirstAsync(p => p.Id == dto.ProductId);
        if (product == null)
        {
            throw new CommonManager.Error.BusinessException("商品不存在");
        }

        var spec = new ProductSpec
        {
            Id = Guid.NewGuid(),
            ProductId = dto.ProductId,
            Name = dto.Name,
            Required = dto.Required,
            Sort = dto.Sort,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(spec).ExecuteCommandAsync();

        // 创建选项
        if (dto.Options != null && dto.Options.Count > 0)
        {
            var options = dto.Options.Select((o, index) => new ProductSpecOption
            {
                Id = Guid.NewGuid(),
                SpecId = spec.Id,
                Name = o.Name,
                Value = o.Value ?? o.Name,
                PriceAdjust = o.PriceAdjust,
                Stock = o.Stock,
                Sort = o.Sort > 0 ? o.Sort : index,
                CreateTime = DateTime.Now
            }).ToList();

            await _db.Insertable(options).ExecuteCommandAsync();
        }

        return spec.Id;
    }

    /// <summary>
    /// 更新规格
    /// </summary>
    public async Task<int> UpdateSpecAsync(UpdateProductSpecDto dto)
    {
        var spec = await _db.Queryable<ProductSpec>().FirstAsync(s => s.Id == dto.Id);
        if (spec == null)
        {
            throw new CommonManager.Error.BusinessException("规格不存在");
        }

        if (!string.IsNullOrEmpty(dto.Name))
        {
            spec.Name = dto.Name;
        }
        if (dto.Required.HasValue)
        {
            spec.Required = dto.Required.Value;
        }
        if (dto.Sort.HasValue)
        {
            spec.Sort = dto.Sort.Value;
        }

        return await _db.Updateable(spec).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除规格
    /// </summary>
    public async Task<int> DeleteSpecAsync(Guid id)
    {
        var spec = await _db.Queryable<ProductSpec>().FirstAsync(s => s.Id == id);
        if (spec == null)
        {
            throw new CommonManager.Error.BusinessException("规格不存在");
        }

        // 删除关联的SKU
        var skus = await _db.Queryable<ProductSku>()
            .Where(s => s.ProductId == spec.ProductId)
            .ToListAsync();

        foreach (var sku in skus)
        {
            var specCombination = JsonSerializer.Deserialize<List<SkuSpecItem>>(sku.SpecCombination);
            if (specCombination != null && specCombination.Any(i => i.SpecId == id))
            {
                await _db.Deleteable<ProductSku>().Where(s => s.Id == sku.Id).ExecuteCommandAsync();
            }
        }

        // 删除选项
        await _db.Deleteable<ProductSpecOption>().Where(o => o.SpecId == id).ExecuteCommandAsync();

        // 删除规格
        return await _db.Deleteable<ProductSpec>().Where(s => s.Id == id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 添加规格选项
    /// </summary>
    public async Task<Guid> AddSpecOptionAsync(Guid specId, AddSpecOptionDto option)
    {
        var spec = await _db.Queryable<ProductSpec>().FirstAsync(s => s.Id == specId);
        if (spec == null)
        {
            throw new CommonManager.Error.BusinessException("规格不存在");
        }

        var entity = new ProductSpecOption
        {
            Id = Guid.NewGuid(),
            SpecId = specId,
            Name = option.Name,
            Value = option.Value ?? option.Name,
            PriceAdjust = option.PriceAdjust,
            Stock = option.Stock,
            Sort = option.Sort,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <summary>
    /// 删除规格选项
    /// </summary>
    public async Task<int> DeleteSpecOptionAsync(Guid optionId)
    {
        var option = await _db.Queryable<ProductSpecOption>().FirstAsync(o => o.Id == optionId);
        if (option == null)
        {
            throw new CommonManager.Error.BusinessException("选项不存在");
        }

        // 删除关联的SKU
        var spec = await _db.Queryable<ProductSpec>().FirstAsync(s => s.Id == option.SpecId);
        if (spec != null)
        {
            var skus = await _db.Queryable<ProductSku>()
                .Where(s => s.ProductId == spec.ProductId)
                .ToListAsync();

            foreach (var sku in skus)
            {
                var specCombination = JsonSerializer.Deserialize<List<SkuSpecItem>>(sku.SpecCombination);
                if (specCombination != null && specCombination.Any(i => i.OptionId == optionId))
                {
                    await _db.Deleteable<ProductSku>().Where(s => s.Id == sku.Id).ExecuteCommandAsync();
                }
            }
        }

        return await _db.Deleteable<ProductSpecOption>().Where(o => o.Id == optionId).ExecuteCommandAsync();
    }

    #endregion

    #region SKU管理

    /// <summary>
    /// 获取商品SKU列表
    /// </summary>
    public async Task<List<ProductSkuDto>> GetSkuListAsync(Guid productId)
    {
        var skus = await _db.Queryable<ProductSku>()
            .Where(s => s.ProductId == productId)
            .OrderBy(s => s.CreateTime)
            .ToListAsync();

        // 获取规格和选项
        var specs = await GetSpecListAsync(productId);

        var result = new List<ProductSkuDto>();
        foreach (var sku in skus)
        {
            var specCombination = JsonSerializer.Deserialize<List<SkuSpecItem>>(sku.SpecCombination) ?? new();
            var specItems = new List<SkuSpecItemDto>();
            var specTexts = new List<string>();

            foreach (var item in specCombination)
            {
                var spec = specs.FirstOrDefault(s => s.Id == item.SpecId);
                var option = spec?.Options?.FirstOrDefault(o => o.Id == item.OptionId);
                if (spec != null && option != null)
                {
                    specItems.Add(new SkuSpecItemDto
                    {
                        SpecId = spec.Id,
                        SpecName = spec.Name,
                        OptionId = option.Id,
                        OptionName = option.Name
                    });
                    specTexts.Add(option.Name);
                }
            }

            result.Add(new ProductSkuDto
            {
                Id = sku.Id,
                ProductId = sku.ProductId,
                SkuCode = sku.SkuCode,
                SpecText = string.Join("-", specTexts),
                Price = sku.Price,
                Stock = sku.Stock,
                Image = sku.Image,
                Status = sku.Status,
                SpecItems = specItems
            });
        }

        return result;
    }

    /// <summary>
    /// 获取SKU详情
    /// </summary>
    public async Task<ProductSkuDto?> GetSkuByIdAsync(Guid id)
    {
        var sku = await _db.Queryable<ProductSku>().FirstAsync(s => s.Id == id);
        if (sku == null) return null;

        var specs = await GetSpecListAsync(sku.ProductId);
        var specCombination = JsonSerializer.Deserialize<List<SkuSpecItem>>(sku.SpecCombination) ?? new();
        var specItems = new List<SkuSpecItemDto>();
        var specTexts = new List<string>();

        foreach (var item in specCombination)
        {
            var spec = specs.FirstOrDefault(s => s.Id == item.SpecId);
            var option = spec?.Options?.FirstOrDefault(o => o.Id == item.OptionId);
            if (spec != null && option != null)
            {
                specItems.Add(new SkuSpecItemDto
                {
                    SpecId = spec.Id,
                    SpecName = spec.Name,
                    OptionId = option.Id,
                    OptionName = option.Name
                });
                specTexts.Add(option.Name);
            }
        }

        return new ProductSkuDto
        {
            Id = sku.Id,
            ProductId = sku.ProductId,
            SkuCode = sku.SkuCode,
            SpecText = string.Join("-", specTexts),
            Price = sku.Price,
            Stock = sku.Stock,
            Image = sku.Image,
            Status = sku.Status,
            SpecItems = specItems
        };
    }

    /// <summary>
    /// 保存SKU
    /// </summary>
    public async Task<Guid> SaveSkuAsync(SaveProductSkuDto dto)
    {
        ProductSku entity;

        if (dto.Id.HasValue)
        {
            // 更新
            entity = await _db.Queryable<ProductSku>().FirstAsync(s => s.Id == dto.Id.Value);
            if (entity == null)
            {
                throw new CommonManager.Error.BusinessException("SKU不存在");
            }
            entity.Price = dto.Price;
            entity.Stock = dto.Stock;
            entity.Image = dto.Image;
            entity.SkuCode = dto.SkuCode ?? entity.SkuCode;
            entity.UpdateTime = DateTime.Now;

            await _db.Updateable(entity).ExecuteCommandAsync();
        }
        else
        {
            // 创建
            // 构建规格组合
            var specCombination = new List<SkuSpecItem>();
            var specs = await GetSpecListAsync(dto.ProductId);

            foreach (var optionId in dto.OptionIds)
            {
                foreach (var spec in specs)
                {
                    var option = spec.Options?.FirstOrDefault(o => o.Id == optionId);
                    if (option != null)
                    {
                        specCombination.Add(new SkuSpecItem
                        {
                            SpecId = spec.Id,
                            OptionId = option.Id
                        });
                        break;
                    }
                }
            }

            // 检查是否已存在相同规格组合的SKU
            var existingSku = await _db.Queryable<ProductSku>()
                .Where(s => s.ProductId == dto.ProductId)
                .ToListAsync();

            var newCombinationJson = JsonSerializer.Serialize(specCombination);
            if (existingSku.Any(s => s.SpecCombination == newCombinationJson))
            {
                throw new CommonManager.Error.BusinessException("该规格组合的SKU已存在");
            }

            entity = new ProductSku
            {
                Id = Guid.NewGuid(),
                ProductId = dto.ProductId,
                SkuCode = dto.SkuCode ?? $"SKU{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100, 999)}",
                SpecCombination = newCombinationJson,
                Price = dto.Price,
                Stock = dto.Stock,
                Image = dto.Image,
                Status = 1,
                CreateTime = DateTime.Now
            };

            await _db.Insertable(entity).ExecuteCommandAsync();
        }

        return entity.Id;
    }

    /// <summary>
    /// 删除SKU
    /// </summary>
    public async Task<int> DeleteSkuAsync(Guid id)
    {
        var sku = await _db.Queryable<ProductSku>().FirstAsync(s => s.Id == id);
        if (sku == null)
        {
            throw new CommonManager.Error.BusinessException("SKU不存在");
        }

        return await _db.Deleteable<ProductSku>().Where(s => s.Id == id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 批量生成SKU
    /// </summary>
    public async Task<int> GenerateSkusAsync(GenerateSkuDto dto)
    {
        var specs = await GetSpecListAsync(dto.ProductId);
        if (specs.Count == 0)
        {
            throw new CommonManager.Error.BusinessException("该商品没有设置规格");
        }

        // 获取所有选项组合
        var allOptions = specs.Select(s => s.Options?.Select(o => new { SpecId = s.Id, OptionId = o.Id, o.Name }).ToList() ?? new()).ToList();

        // 计算笛卡尔积
        var combinations = allOptions.Aggregate(
            Enumerable.Repeat(new List<object>(), 1),
            (acc, options) => acc.SelectMany(c => options.Select(o => c.Append(o).ToList()))
        ).ToList();

        // 删除现有SKU
        await _db.Deleteable<ProductSku>().Where(s => s.ProductId == dto.ProductId).ExecuteCommandAsync();

        // 创建新SKU
        var skus = new List<ProductSku>();
        int index = 1;
        foreach (var combination in combinations)
        {
            var specCombination = new List<SkuSpecItem>();
            foreach (dynamic item in combination)
            {
                specCombination.Add(new SkuSpecItem
                {
                    SpecId = item.SpecId,
                    OptionId = item.OptionId
                });
            }

            skus.Add(new ProductSku
            {
                Id = Guid.NewGuid(),
                ProductId = dto.ProductId,
                SkuCode = $"SKU{DateTime.Now:yyyyMMddHHmmss}{index++:D3}",
                SpecCombination = JsonSerializer.Serialize(specCombination),
                Price = dto.BasePrice,
                Stock = dto.DefaultStock,
                Status = 1,
                CreateTime = DateTime.Now
            });
        }

        if (skus.Count > 0)
        {
            await _db.Insertable(skus).ExecuteCommandAsync();
        }

        return skus.Count;
    }

    /// <summary>
    /// 更新SKU库存
    /// </summary>
    public async Task<int> UpdateSkuStockAsync(Guid id, int stock)
    {
        var sku = await _db.Queryable<ProductSku>().FirstAsync(s => s.Id == id);
        if (sku == null)
        {
            throw new CommonManager.Error.BusinessException("SKU不存在");
        }

        var newStock = sku.Stock + stock;
        if (newStock < 0)
        {
            throw new CommonManager.Error.BusinessException("库存不足");
        }

        sku.Stock = newStock;
        sku.UpdateTime = DateTime.Now;

        return await _db.Updateable(sku).ExecuteCommandAsync();
    }

    #endregion
}