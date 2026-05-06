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
/// 微信订单服务实现类
/// </summary>
/// <remarks>
/// 实现订单相关的业务逻辑，包括订单创建、查询、取消等功能。
/// 继承自<see cref="BaseService{Order}"/>，使用SqlSugar进行数据库操作。
/// 订单创建使用事务处理，保证数据一致性。
/// </remarks>
public class WeChatOrderService : BaseService<Order>, IWeChatOrderService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatOrderService> _logger { get; set; } = null!;

    /// <summary>
    /// 创建订单
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="dto">订单信息，包含购物车项ID列表、地址ID、备注</param>
    /// <returns>订单ID</returns>
    /// <exception cref="BusinessException">
    /// 购物车项不存在、商品已下架、库存不足、地址不存在等情况抛出相应异常
    /// </exception>
    /// <remarks>
    /// 事务处理流程：
    /// <list type="number">
    ///     <item>验证购物车项和商品信息</item>
    ///     <item>验证收货地址</item>
    ///     <item>创建订单主表记录</item>
    ///     <item>创建订单项记录</item>
    ///     <item>扣减商品库存</item>
    ///     <item>删除已下单的购物车项</item>
    /// </list>
    /// </remarks>
    public async Task<Guid> CreateOrderAsync(Guid userId, CreateOrderDto dto)
    {
        // 获取购物车项
        var cartItems = await _db.Queryable<Cart, Product>((c, p) => new JoinQueryInfos(JoinType.Left, c.ProductId == p.Id))
            .Where((c, p) => c.UserId == userId && dto.CartItemIds!.Contains(c.Id) && p.Status == 1)
            .Select((c, p) => new { Cart = c, Product = p })
            .ToListAsync();

        if (cartItems.Count == 0)
        {
            throw BusinessException.BadRequest("购物车中没有有效的商品");
        }

        // 验证所有购物车项都属于当前用户
        var invalidIds = dto.CartItemIds!.Where(id => !cartItems.Any(ci => ci.Cart.Id == id)).ToList();
        if (invalidIds.Count > 0)
        {
            throw BusinessException.BadRequest($"购物车项不存在或商品已下架: {string.Join(",", invalidIds)}");
        }

        // 检查库存
        foreach (var item in cartItems)
        {
            if (item.Cart.Count > item.Product.Stock)
            {
                throw BusinessException.BadRequest($"商品 {item.Product.Name} 库存不足");
            }
        }

        // 验证地址
        var address = await _db.Queryable<Address>()
            .Where(a => a.Id == dto.AddressId && a.UserId == userId && a.Status == 1)
            .FirstAsync();

        if (address == null)
        {
            throw BusinessException.NotFound("收货地址不存在");
        }

        // 计算订单总金额
        var totalAmount = cartItems.Sum(ci => ci.Cart.Count * ci.Product.Price);

        // 生成订单编号
        var orderNo = GenerateOrderNo();

        var orderId = Guid.Empty;

        // 使用事务处理
        await ExecuteTransactionAsync(async () =>
        {
            // 创建订单
            var order = new Order
            {
                OrderNo = orderNo,
                UserId = userId,
                AddressId = dto.AddressId ?? Guid.Empty,
                Status = OrderStatus.Pending,
                TotalAmount = totalAmount,
                Remark = dto.Remark,
                CreateTime = DateTime.Now
            };

            await _db.Insertable(order).ExecuteCommandAsync();
            orderId = order.Id;

            // 创建订单项
            var orderItems = cartItems.Select(ci => new OrderItem
            {
                OrderId = orderId,
                ProductId = ci.Cart.ProductId,
                ProductName = ci.Product.Name,
                ProductImage = ci.Product.Image,
                Price = ci.Product.Price,
                Count = ci.Cart.Count,
                Subtotal = ci.Cart.Count * ci.Product.Price,
                CreateTime = DateTime.Now
            }).ToList();

            await _db.Insertable(orderItems).ExecuteCommandAsync();

            // 扣减库存
            foreach (var item in cartItems)
            {
                await _db.Updateable<Product>()
                    .Where(p => p.Id == item.Product.Id)
                    .SetColumns(p => p.Stock == p.Stock - item.Cart.Count)
                    .ExecuteCommandAsync();
            }

            // 删除购物车项
            await _db.Deleteable<Cart>()
                .Where(c => dto.CartItemIds!.Contains(c.Id))
                .ExecuteCommandAsync();
        });

        _logger.LogInformation("用户 {UserId} 创建订单 {OrderId}，订单编号 {OrderNo}", userId, orderId, orderNo);

        return orderId;
    }

    /// <summary>
    /// 获取订单列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="query">查询参数，包含分页和状态筛选</param>
    /// <returns>订单分页列表</returns>
    /// <remarks>
    /// 包含订单项列表和收货地址信息。
    /// 按创建时间倒序排列。
    /// </remarks>
    public async Task<PageResponse<OrderDto>> GetOrderListAsync(Guid userId, QueryOrderDto query)
    {
        var total = new RefAsync<int>();
        var orders = await _db.Queryable<Order>()
            .Where(o => o.UserId == userId)
            .WhereIF(query.Status.HasValue, o => o.Status == query.Status!.Value)
            .OrderByDescending(o => o.CreateTime)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        var orderDtos = orders.Adapt<List<OrderDto>>();

        // 加载订单项和地址信息
        await LoadOrderDetailsAsync(orderDtos);

        return PageResponse<OrderDto>.Create(orderDtos, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取订单详情
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="id">订单ID</param>
    /// <returns>订单详情；订单不存在返回null</returns>
    /// <remarks>
    /// 包含订单项列表、收货地址和物流信息。
    /// </remarks>
    public async Task<OrderDto?> GetOrderByIdAsync(Guid userId, Guid id)
    {
        var order = await GetFirstAsync(o => o.Id == id && o.UserId == userId);
        if (order == null)
        {
            return null;
        }

        var orderDto = order.Adapt<OrderDto>();

        // 加载订单项
        var orderItems = await _db.Queryable<OrderItem>()
            .Where(oi => oi.OrderId == id)
            .ToListAsync();
        orderDto.Items = orderItems.Adapt<List<OrderItemDto>>();

        // 加载地址
        var address = await _db.Queryable<Address>()
            .Where(a => a.Id == order.AddressId)
            .FirstAsync();
        if (address != null)
        {
            orderDto.Address = address.Adapt<AddressDto>();
        }

        // 加载物流信息
        if (!string.IsNullOrEmpty(order.LogisticsCompany) && !string.IsNullOrEmpty(order.LogisticsNumber))
        {
            orderDto.Logistics = new LogisticsDto
            {
                Company = order.LogisticsCompany,
                Number = order.LogisticsNumber,
                Status = "运输中" // Mock 物流状态
            };
        }

        return orderDto;
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="id">订单ID</param>
    /// <returns>是否成功</returns>
    /// <exception cref="BusinessException">
    /// 订单不存在、订单不属于当前用户、订单状态不允许取消时抛出相应异常
    /// </exception>
    /// <remarks>
    /// 只允许取消待支付状态的订单。
    /// 取消后恢复商品库存。
    /// </remarks>
    public async Task<bool> CancelOrderAsync(Guid userId, Guid id)
    {
        var order = await GetFirstAsync(o => o.Id == id && o.UserId == userId);
        if (order == null)
        {
            throw BusinessException.NotFound("订单不存在");
        }

        if (order.Status != OrderStatus.Pending)
        {
            throw BusinessException.BadRequest("只有待支付状态的订单可以取消");
        }

        // 使用事务处理
        await ExecuteTransactionAsync(async () =>
        {
            // 更新订单状态
            order.Status = OrderStatus.Cancelled;
            order.UpdateTime = DateTime.Now;
            await UpdateAsync(order);

            // 恢复库存
            var orderItems = await _db.Queryable<OrderItem>()
                .Where(oi => oi.OrderId == id)
                .ToListAsync();

            foreach (var item in orderItems)
            {
                await _db.Updateable<Product>()
                    .Where(p => p.Id == item.ProductId)
                    .SetColumns(p => p.Stock == p.Stock + item.Count)
                    .ExecuteCommandAsync();
            }
        });

        _logger.LogInformation("用户 {UserId} 取消订单 {OrderId}", userId, id);

        return true;
    }

    /// <summary>
    /// 生成订单编号
    /// </summary>
    /// <returns>订单编号字符串</returns>
    /// <remarks>
    /// 格式：日期(8位) + 时间(6位) + 随机数(6位) = 20位
    /// </remarks>
    private string GenerateOrderNo()
    {
        var now = DateTime.Now;
        var random = new Random();
        return $"{now:yyyyMMddHHmmss}{random.Next(100000, 999999)}";
    }

    /// <summary>
    /// 加载订单详情信息
    /// </summary>
    /// <param name="orderDtos">订单DTO列表</param>
    private async Task LoadOrderDetailsAsync(List<OrderDto> orderDtos)
    {
        if (orderDtos.Count == 0) return;

        var orderIds = orderDtos.Select(o => o.Id).ToList();
        var addressIds = orderDtos.Where(o => o.AddressId.HasValue).Select(o => o.AddressId!.Value).Distinct().ToList();

        // 加载订单项
        var orderItems = await _db.Queryable<OrderItem>()
            .Where(oi => orderIds.Contains(oi.OrderId))
            .ToListAsync();

        var itemsDict = orderItems.GroupBy(oi => oi.OrderId).ToDictionary(g => g.Key, g => g.ToList());

        // 加载地址
        var addresses = await _db.Queryable<Address>()
            .Where(a => addressIds.Contains(a.Id))
            .ToListAsync();

        var addressDict = addresses.ToDictionary(a => a.Id);

        // 重新查询订单实体以获取物流信息
        var orderEntities = await _db.Queryable<Order>()
            .Where(o => orderIds.Contains(o.Id))
            .ToListAsync();

        var orderEntityDict = orderEntities.ToDictionary(o => o.Id);

        foreach (var order in orderDtos)
        {
            // 设置订单项
            if (itemsDict.TryGetValue(order.Id, out var items))
            {
                order.Items = items.Adapt<List<OrderItemDto>>();
            }

            // 设置地址
            if (order.AddressId.HasValue && addressDict.TryGetValue(order.AddressId.Value, out var address))
            {
                order.Address = address.Adapt<AddressDto>();
            }

            // 设置物流信息（如果有）
            if (orderEntityDict.TryGetValue(order.Id, out var orderEntity))
            {
                if (!string.IsNullOrEmpty(orderEntity.LogisticsCompany) && !string.IsNullOrEmpty(orderEntity.LogisticsNumber))
                {
                    order.Logistics = new LogisticsDto
                    {
                        Company = orderEntity.LogisticsCompany,
                        Number = orderEntity.LogisticsNumber,
                        Status = "运输中"
                    };
                }
            }
        }
    }
}