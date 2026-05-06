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
/// 微信购物车服务实现类
/// </summary>
/// <remarks>
/// 实现购物车相关的业务逻辑，包括购物车查询、添加、更新、删除等功能。
/// 继承自<see cref="BaseService{Cart}"/>，使用SqlSugar进行数据库操作。
/// </remarks>
public class WeChatCartService : BaseService<Cart>, IWeChatCartService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatCartService> _logger { get; set; } = null!;

    /// <summary>
    /// 获取购物车列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>购物车状态，包含商品列表和统计信息</returns>
    /// <remarks>
    /// 关联查询商品信息，计算小计和总价。
    /// 只返回上架状态的商品。
    /// </remarks>
    public async Task<CartStateDto> GetCartListAsync(Guid userId)
    {
        var cartItems = await _db.Queryable<Cart, Product>((c, p) => new JoinQueryInfos(JoinType.Left, c.ProductId == p.Id))
            .Where((c, p) => c.UserId == userId && p.Status == 1)
            .Select((c, p) => new CartDto
            {
                Id = c.Id,
                ProductId = c.ProductId,
                Count = c.Count,
                Selected = c.Selected,
                CreateTime = c.CreateTime,
                Product = new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.Image,
                    Stock = p.Stock
                }
            })
            .ToListAsync();

        // 计算小计
        foreach (var item in cartItems)
        {
            item.Subtotal = item.Count * item.Product.Price;
        }

        // 计算统计信息
        var state = new CartStateDto
        {
            Items = cartItems,
            TotalCount = cartItems.Sum(c => c.Count),
            TotalPrice = cartItems.Sum(c => c.Subtotal),
            SelectedCount = cartItems.Where(c => c.Selected).Sum(c => c.Count),
            SelectedPrice = cartItems.Where(c => c.Selected).Sum(c => c.Subtotal)
        };

        return state;
    }

    /// <summary>
    /// 添加商品到购物车
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="dto">添加信息，包含商品ID和数量</param>
    /// <returns>购物车项ID</returns>
    /// <exception cref="BusinessException">
    /// 商品不存在或已下架时抛出NotFound异常；
    /// 库存不足时抛出BadRequest异常
    /// </exception>
    /// <remarks>
    /// 如果商品已在购物车中，则更新数量；
    /// 否则新增购物车项。
    /// </remarks>
    public async Task<Guid> AddToCartAsync(Guid userId, AddCartDto dto)
    {
        // 检查商品是否存在
        var product = await _db.Queryable<Product>()
            .Where(p => p.Id == dto.ProductId && p.Status == 1)
            .FirstAsync();

        if (product == null)
        {
            throw BusinessException.NotFound("商品不存在或已下架");
        }

        // 检查库存
        var existingCart = await GetFirstAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);
        var totalCount = dto.Count + (existingCart?.Count ?? 0);

        if (totalCount > product.Stock)
        {
            throw BusinessException.BadRequest("库存不足");
        }

        if (existingCart != null)
        {
            // 更新数量
            existingCart.Count = totalCount;
            existingCart.UpdateTime = DateTime.Now;
            await UpdateAsync(existingCart);
            return existingCart.Id;
        }
        else
        {
            // 新增购物车项
            var cart = new Cart
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Count = dto.Count,
                Selected = true,
                CreateTime = DateTime.Now
            };

            return await InsertAsync(cart);
        }
    }

    /// <summary>
    /// 更新购物车项
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="cartId">购物车项ID</param>
    /// <param name="dto">更新信息，包含数量和选中状态</param>
    /// <returns>是否成功</returns>
    /// <exception cref="BusinessException">
    /// 购物车项不存在时抛出NotFound异常；
    /// 库存不足时抛出BadRequest异常
    /// </exception>
    /// <remarks>
    /// 支持更新数量和选中状态。
    /// 更新数量时会检查库存。
    /// </remarks>
    public async Task<bool> UpdateCartItemAsync(Guid userId, Guid cartId, UpdateCartDto dto)
    {
        var cart = await GetFirstAsync(c => c.Id == cartId && c.UserId == userId);
        if (cart == null)
        {
            throw BusinessException.NotFound("购物车项不存在");
        }

        // 检查库存
        if (dto.Count > 0)
        {
            var product = await _db.Queryable<Product>()
                .Where(p => p.Id == cart.ProductId)
                .FirstAsync();

            if (product != null && dto.Count > product.Stock)
            {
                throw BusinessException.BadRequest("库存不足");
            }

            cart.Count = dto.Count;
        }

        if (dto.Selected.HasValue)
        {
            cart.Selected = dto.Selected.Value;
        }

        cart.UpdateTime = DateTime.Now;
        await UpdateAsync(cart);

        return true;
    }

    /// <summary>
    /// 删除购物车项
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="cartId">购物车项ID</param>
    /// <returns>是否成功</returns>
    /// <exception cref="BusinessException">
    /// 购物车项不存在时抛出NotFound异常
    /// </exception>
    public async Task<bool> DeleteCartItemAsync(Guid userId, Guid cartId)
    {
        var cart = await GetFirstAsync(c => c.Id == cartId && c.UserId == userId);
        if (cart == null)
        {
            throw BusinessException.NotFound("购物车项不存在");
        }

        await DeleteAsync(cartId);
        return true;
    }

    /// <summary>
    /// 清空购物车
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除指定用户的所有购物车项。
    /// </remarks>
    public async Task<bool> ClearCartAsync(Guid userId)
    {
        await DeleteAsync(c => c.UserId == userId);
        return true;
    }
}