using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using SqlSugar;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 供应商服务实现
/// </summary>
public class SupplierService : BaseService, ISupplierService
{
    #region 供应商管理

    /// <summary>
    /// 获取供应商分页列表
    /// </summary>
    public async Task<PageResponse<SupplierDto>> GetPageListAsync(QuerySupplierDto query)
    {
        var queryable = _db.Queryable<Supplier>()
            // 条件筛选 - 使用 WhereIF
            .WhereIF(!string.IsNullOrEmpty(query.Name), s => s.Name.Contains(query.Name!))
            .WhereIF(query.Status.HasValue, s => s.Status == query.Status!.Value)
            .OrderByDescending(s => s.CreateTime);

        // 分页
        var total = new RefAsync<int>();
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 转换 DTO
        var dtoList = list.Select(s => new SupplierDto
        {
            Id = s.Id,
            Name = s.Name,
            UnifiedCode = s.UnifiedCode,
            Contact = s.Contact,
            Phone = s.Phone,
            Address = s.Address,
            Remark = s.Remark,
            Status = s.Status,
            CreateTime = s.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = s.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return PageResponse<SupplierDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 根据ID获取供应商详情
    /// </summary>
    public async Task<SupplierDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<Supplier>()
            .FirstAsync(s => s.Id == id);

        if (entity == null) return null;

        return new SupplierDto
        {
            Id = entity.Id,
            Name = entity.Name,
            UnifiedCode = entity.UnifiedCode,
            Contact = entity.Contact,
            Phone = entity.Phone,
            Address = entity.Address,
            Remark = entity.Remark,
            Status = entity.Status,
            CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = entity.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    /// <summary>
    /// 添加供应商
    /// </summary>
    public async Task<Guid> AddAsync(AddSupplierDto dto)
    {
        // 检查名称是否已存在
        var exists = await _db.Queryable<Supplier>().AnyAsync(s => s.Name == dto.Name);
        if (exists)
        {
            throw new CommonManager.Error.BusinessException($"供应商名称 {dto.Name} 已存在");
        }

        var entity = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            UnifiedCode = dto.UnifiedCode,
            Contact = dto.Contact,
            Phone = dto.Phone,
            Address = dto.Address,
            Remark = dto.Remark,
            Status = dto.Status,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <summary>
    /// 更新供应商
    /// </summary>
    public async Task<int> UpdateAsync(UpdateSupplierDto dto)
    {
        var entity = await _db.Queryable<Supplier>().FirstAsync(s => s.Id == dto.Id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("供应商不存在");
        }

        // 检查名称是否被其他供应商使用
        if (dto.Name != entity.Name)
        {
            var exists = await _db.Queryable<Supplier>().AnyAsync(s => s.Name == dto.Name && s.Id != dto.Id);
            if (exists)
            {
                throw new CommonManager.Error.BusinessException($"供应商名称 {dto.Name} 已被其他供应商使用");
            }
        }

        // 更新字段
        entity.Name = dto.Name;
        entity.UnifiedCode = dto.UnifiedCode;
        entity.Contact = dto.Contact;
        entity.Phone = dto.Phone;
        entity.Address = dto.Address;
        entity.Remark = dto.Remark;
        entity.Status = dto.Status;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除供应商
    /// </summary>
    public async Task<int> DeleteAsync(Guid id)
    {
        // 检查是否有关联的商品
        var hasProducts = await _db.Queryable<ProductSupplier>().AnyAsync(ps => ps.SupplierId == id);
        if (hasProducts)
        {
            throw new CommonManager.Error.BusinessException("该供应商有关联的商品，无法删除");
        }

        return await _db.Deleteable<Supplier>().Where(s => s.Id == id).ExecuteCommandAsync();
    }

    #endregion

    #region 商品供应商关联

    /// <summary>
    /// 获取商品的供应商列表
    /// </summary>
    public async Task<List<ProductSupplierDto>> GetProductSuppliersAsync(Guid productId)
    {
        var list = await _db.Queryable<ProductSupplier>()
            .Where(ps => ps.ProductId == productId)
            .ToListAsync();

        // 获取供应商信息
        var supplierIds = list.Select(ps => ps.SupplierId).Distinct().ToList();
        var suppliers = await _db.Queryable<Supplier>()
            .Where(s => supplierIds.Contains(s.Id))
            .ToListAsync();

        return list.Select(ps => new ProductSupplierDto
        {
            Id = ps.Id,
            ProductId = ps.ProductId,
            SupplierId = ps.SupplierId,
            SupplierName = suppliers.FirstOrDefault(s => s.Id == ps.SupplierId)?.Name ?? "",
            SkuCode = ps.SkuCode,
            PurchasePrice = ps.PurchasePrice,
            MinOrderQty = ps.MinOrderQty,
            IsDefault = ps.IsDefault,
            Remark = ps.Remark
        }).ToList();
    }

    /// <summary>
    /// 绑定商品供应商
    /// </summary>
    public async Task<Guid> BindProductSupplierAsync(BindProductSupplierDto dto)
    {
        // 检查商品是否存在
        var product = await _db.Queryable<Product>().FirstAsync(p => p.Id == dto.ProductId);
        if (product == null)
        {
            throw new CommonManager.Error.BusinessException("商品不存在");
        }

        // 检查供应商是否存在
        var supplier = await _db.Queryable<Supplier>().FirstAsync(s => s.Id == dto.SupplierId);
        if (supplier == null)
        {
            throw new CommonManager.Error.BusinessException("供应商不存在");
        }

        // 检查是否已绑定
        var exists = await _db.Queryable<ProductSupplier>()
            .AnyAsync(ps => ps.ProductId == dto.ProductId && ps.SupplierId == dto.SupplierId);
        if (exists)
        {
            throw new CommonManager.Error.BusinessException("该商品已绑定此供应商");
        }

        // 如果设置为默认供应商，取消其他默认
        if (dto.IsDefault)
        {
            await _db.Updateable<ProductSupplier>()
                .Where(ps => ps.ProductId == dto.ProductId && ps.IsDefault)
                .SetColumns(ps => ps.IsDefault == false)
                .ExecuteCommandAsync();
        }

        var entity = new ProductSupplier
        {
            Id = Guid.NewGuid(),
            ProductId = dto.ProductId,
            SupplierId = dto.SupplierId,
            SkuCode = dto.SkuCode,
            PurchasePrice = dto.PurchasePrice,
            MinOrderQty = dto.MinOrderQty,
            IsDefault = dto.IsDefault,
            Remark = dto.Remark,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <summary>
    /// 解绑商品供应商
    /// </summary>
    public async Task<int> UnbindProductSupplierAsync(Guid id)
    {
        return await _db.Deleteable<ProductSupplier>().Where(ps => ps.Id == id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 设置默认供应商
    /// </summary>
    public async Task<int> SetDefaultSupplierAsync(Guid id)
    {
        var productSupplier = await _db.Queryable<ProductSupplier>()
            .FirstAsync(ps => ps.Id == id);
        if (productSupplier == null)
        {
            throw new CommonManager.Error.BusinessException("商品供应商关联不存在");
        }

        // 取消该商品的其他默认供应商
        await _db.Updateable<ProductSupplier>()
            .Where(ps => ps.ProductId == productSupplier.ProductId && ps.IsDefault)
            .SetColumns(ps => ps.IsDefault == false)
            .ExecuteCommandAsync();

        // 设置为默认
        productSupplier.IsDefault = true;
        return await _db.Updateable(productSupplier).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取供应商的商品列表（含绑定信息）
    /// </summary>
    public async Task<List<ProductSupplierDto>> GetSupplierProductsAsync(Guid supplierId)
    {
        // 获取供应商关联的商品绑定信息
        var productSuppliers = await _db.Queryable<ProductSupplier>()
            .Where(ps => ps.SupplierId == supplierId)
            .ToListAsync();

        if (productSuppliers.Count == 0)
        {
            return new List<ProductSupplierDto>();
        }

        // 获取商品ID列表
        var productIds = productSuppliers.Select(ps => ps.ProductId).Distinct().ToList();

        // 获取商品信息
        var products = await _db.Queryable<Product>()
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        // 获取分类信息
        var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();
        var categories = await _db.Queryable<Category>()
            .Where(c => categoryIds.Contains(c.Id))
            .ToListAsync();

        // 组装结果
        return productSuppliers.Select(ps =>
        {
            var product = products.FirstOrDefault(p => p.Id == ps.ProductId);
            var category = categories.FirstOrDefault(c => c.Id == product?.CategoryId);
            return new ProductSupplierDto
            {
                Id = ps.Id,
                ProductId = ps.ProductId,
                SupplierId = ps.SupplierId,
                SkuCode = ps.SkuCode,
                PurchasePrice = ps.PurchasePrice,
                MinOrderQty = ps.MinOrderQty,
                IsDefault = ps.IsDefault,
                Remark = ps.Remark,
                Product = product != null ? new ProductDto
                {
                    Id = product.Id,
                    SkuCode = product.SkuCode,
                    Name = product.Name,
                    Image = product.Image,
                    Price = product.Price,
                    OriginalPrice = product.OriginalPrice,
                    Stock = product.Stock,
                    Sales = product.Sales,
                    CategoryId = product.CategoryId,
                    CategoryName = category?.Name,
                    Category = category != null ? new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Icon = category.Icon,
                        Sort = category.Sort,
                        Description = category.Description,
                        Status = category.Status,
                        CreateTime = category.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
                    } : null,
                    Status = product.Status,
                    CreateTime = product.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
                } : null
            };
        }).ToList();
    }

    #endregion
}