using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using SqlSugar;
using System.Text.Json;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 订单服务实现
/// </summary>
public class OrderService : BaseService, IOrderService
{
    /// <summary>
    /// 获取订单分页列表
    /// </summary>
    public async Task<PageResponse<OrderDto>> GetPageListAsync(QueryOrderDto query)
    {
        var queryable = _db.Queryable<Order>()
            // 条件筛选 - 使用 WhereIF
            .WhereIF(!string.IsNullOrEmpty(query.OrderNo), o => o.OrderNo.Contains(query.OrderNo!))
            .WhereIF(query.UserId.HasValue, o => o.UserId == query.UserId!.Value)
            .WhereIF(query.Status.HasValue, o => o.Status == query.Status!.Value)
            .WhereIF(!string.IsNullOrEmpty(query.StartTime), o => o.CreateTime >= DateTime.Parse(query.StartTime!))
            .WhereIF(!string.IsNullOrEmpty(query.EndTime), o => o.CreateTime < DateTime.Parse(query.EndTime!).AddDays(1));

        // 如果有用户关键字筛选，先查询符合条件的用户ID
        if (!string.IsNullOrEmpty(query.UserKeyword))
        {
            var matchedUserIds = await _db.Queryable<User>()
                .Where(u => u.UserName.Contains(query.UserKeyword) || (u.Phone != null && u.Phone.Contains(query.UserKeyword)))
                .Select(u => u.Id)
                .ToListAsync();
            if (matchedUserIds.Count > 0)
            {
                queryable = queryable.Where(o => matchedUserIds.Contains(o.UserId));
            }
            else
            {
                // 没有匹配的用户，返回空结果
                return PageResponse<OrderDto>.Create(new List<OrderDto>(), 0, query.PageIndex, query.PageSize);
            }
        }

        // 排序
        queryable = queryable.OrderByDescending(o => o.CreateTime);

        // 分页
        var total = new RefAsync<int>();
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 获取订单商品
        var orderIds = list.Select(o => o.Id).ToList();
        var orderItems = await _db.Queryable<OrderItem>()
            .Where(i => orderIds.Contains(i.OrderId))
            .ToListAsync();

        // 获取用户信息
        var userIds = list.Select(o => o.UserId).Distinct().ToList();
        var users = await _db.Queryable<User>()
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();

        // 获取收货地址信息
        var addressIds = list.Where(o => o.AddressId.HasValue && o.AddressId.Value != Guid.Empty).Select(o => o.AddressId!.Value).Distinct().ToList();
        var addresses = addressIds.Count > 0
            ? await _db.Queryable<Address>()
                .Where(a => addressIds.Contains(a.Id))
                .ToListAsync()
            : new List<Address>();

        // 转换 DTO
        var dtoList = list.Select(o => {
            var user = users.FirstOrDefault(u => u.Id == o.UserId);
            var address = addresses.FirstOrDefault(a => a.Id == o.AddressId);
            return new OrderDto
            {
                Id = o.Id,
                OrderNo = o.OrderNo,
                UserId = o.UserId,
                UserName = user?.RealName ?? user?.UserName ?? "",
                UserPhone = user?.Phone ?? "",
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ReceiverName = o.ReceiverName ?? address?.Name ?? "",
                ReceiverPhone = o.ReceiverPhone ?? address?.Phone ?? "",
                ReceiverAddress = o.ReceiverAddress ?? (address != null ? $"{address.Province} {address.City} {address.District} {address.Detail}" : ""),
                AddressId = o.AddressId,
                Items = orderItems.Where(i => i.OrderId == o.Id).Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductImage = i.ProductImage,
                    Price = i.Price,
                    Quantity = i.Count,
                    Amount = i.Price * i.Count
                }).ToList(),
                PayTime = o.PaymentTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                LogisticsCompany = o.LogisticsCompany,
                LogisticsNumber = o.LogisticsNumber,
                DeliveryTime = o.DeliveryTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                Remark = o.Remark,
                AdminRemark = o.AdminRemark,
                RefundAmount = o.RefundAmount,
                Source = o.Source,
                CreateTime = o.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }).ToList();

        return PageResponse<OrderDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 根据ID获取订单详情
    /// </summary>
    public async Task<OrderDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<Order>()
            .FirstAsync(o => o.Id == id);

        if (entity == null) return null;

        // 获取订单商品
        var orderItems = await _db.Queryable<OrderItem>()
            .Where(i => i.OrderId == id)
            .ToListAsync();

        // 获取用户信息
        var user = await _db.Queryable<User>()
            .FirstAsync(u => u.Id == entity.UserId);

        // 获取收货地址信息（如果有）
        Address? address = null;
        if (entity.AddressId.HasValue && entity.AddressId.Value != Guid.Empty)
        {
            address = await _db.Queryable<Address>()
                .FirstAsync(a => a.Id == entity.AddressId.Value);
        }

        return new OrderDto
        {
            Id = entity.Id,
            OrderNo = entity.OrderNo,
            UserId = entity.UserId,
            UserName = user?.RealName ?? user?.UserName ?? "",
            UserPhone = user?.Phone ?? "",
            Status = entity.Status,
            TotalAmount = entity.TotalAmount,
            ReceiverName = entity.ReceiverName ?? address?.Name ?? "",
            ReceiverPhone = entity.ReceiverPhone ?? address?.Phone ?? "",
            ReceiverAddress = entity.ReceiverAddress ?? (address != null ? $"{address.Province} {address.City} {address.District} {address.Detail}" : ""),
            AddressId = entity.AddressId,
            Items = orderItems.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductImage = i.ProductImage,
                Price = i.Price,
                Quantity = i.Count,
                Amount = i.Price * i.Count
            }).ToList(),
            PayTime = entity.PaymentTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            LogisticsCompany = entity.LogisticsCompany,
            LogisticsNumber = entity.LogisticsNumber,
            DeliveryTime = entity.DeliveryTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            Remark = entity.Remark,
            AdminRemark = entity.AdminRemark,
            RefundAmount = entity.RefundAmount,
            Source = entity.Source,
            CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    /// <summary>
    /// 创建订单（后台代客下单）
    /// </summary>
    public async Task<Guid> CreateAsync(CreateOrderDto dto)
    {
        // 生成订单编号
        var orderNo = GenerateOrderNo();

        // 计算总金额
        var totalAmount = dto.Items?.Sum(i => i.Price * i.Quantity) ?? 0;

        var entity = new Order
        {
            Id = Guid.NewGuid(),
            OrderNo = orderNo,
            UserId = dto.UserId,
            AddressId = dto.AddressId, // 可选地址ID
            ReceiverName = dto.ReceiverName, // 直接保存收货人信息
            ReceiverPhone = dto.ReceiverPhone,
            ReceiverAddress = dto.ReceiverAddress,
            Status = OrderStatus.Paid, // 后台代客下单默认已支付（待发货）
            TotalAmount = totalAmount,
            PaymentTime = DateTime.Now, // 后台下单视为已支付
            Remark = dto.Remark,
            Source = "pc",
            CreateTime = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();

        // 创建订单商品
        foreach (var item in dto.Items ?? new List<CreateOrderItemDto>())
        {
            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = entity.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductImage = item.ProductImage ?? "",
                Price = item.Price,
                Count = item.Quantity
            };
            await _db.Insertable(orderItem).ExecuteCommandAsync();
        }

        return entity.Id;
    }

    /// <summary>
    /// 更新订单
    /// </summary>
    public async Task<int> UpdateAsync(UpdateOrderDto dto)
    {
        var entity = await _db.Queryable<Order>().FirstAsync(o => o.Id == dto.Id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("订单不存在");
        }

        if (!string.IsNullOrEmpty(dto.ReceiverName))
        {
            entity.ReceiverName = dto.ReceiverName;
        }
        if (!string.IsNullOrEmpty(dto.ReceiverPhone))
        {
            entity.ReceiverPhone = dto.ReceiverPhone;
        }
        if (!string.IsNullOrEmpty(dto.ReceiverAddress))
        {
            entity.ReceiverAddress = dto.ReceiverAddress;
        }
        if (!string.IsNullOrEmpty(dto.AdminRemark))
        {
            entity.AdminRemark = dto.AdminRemark;
        }
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 订单发货
    /// </summary>
    public async Task<int> ShipAsync(ShipDto dto)
    {
        var entity = await _db.Queryable<Order>().FirstAsync(o => o.Id == dto.OrderId);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("订单不存在");
        }

        if (entity.Status != 1)
        {
            throw new CommonManager.Error.BusinessException("订单状态不允许发货");
        }

        entity.LogisticsCompany = dto.LogisticsCompany;
        entity.LogisticsNumber = dto.LogisticsNumber;
        entity.DeliveryTime = DateTime.Now;
        entity.Status = 2; // 待收货
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    public async Task<int> CancelAsync(Guid id)
    {
        var entity = await _db.Queryable<Order>().FirstAsync(o => o.Id == id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("订单不存在");
        }

        if (entity.Status != 0 && entity.Status != 1)
        {
            throw new CommonManager.Error.BusinessException("订单状态不允许取消");
        }

        entity.Status = 4; // 已取消
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 确认收货
    /// </summary>
    public async Task<int> ConfirmAsync(Guid id)
    {
        var entity = await _db.Queryable<Order>().FirstAsync(o => o.Id == id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("订单不存在");
        }

        if (entity.Status != 2)
        {
            throw new CommonManager.Error.BusinessException("订单状态不允许确认收货");
        }

        entity.Status = 3; // 已完成
        entity.ConfirmTime = DateTime.Now;
        entity.CompleteTime = DateTime.Now;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取物流轨迹
    /// </summary>
    public async Task<ShipTrackDto> GetShipTrackAsync(Guid id)
    {
        var entity = await _db.Queryable<Order>().FirstAsync(o => o.Id == id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("订单不存在");
        }

        // 模拟物流轨迹数据
        var tracks = new List<ShipTrackItemDto>
        {
            new ShipTrackItemDto
            {
                Time = DateTime.Now.AddHours(-48).ToString("yyyy-MM-dd HH:mm:ss"),
                Status = "已签收",
                Description = "快件已签收，签收人：本人签收"
            },
            new ShipTrackItemDto
            {
                Time = DateTime.Now.AddHours(-50).ToString("yyyy-MM-dd HH:mm:ss"),
                Status = "派送中",
                Location = "北京市朝阳区",
                Description = "快件正在派送中，派送员：张三，电话：13800138000"
            },
            new ShipTrackItemDto
            {
                Time = DateTime.Now.AddHours(-72).ToString("yyyy-MM-dd HH:mm:ss"),
                Status = "运输中",
                Location = "北京市海淀区",
                Description = "快件已到达北京转运中心"
            },
            new ShipTrackItemDto
            {
                Time = DateTime.Now.AddHours(-96).ToString("yyyy-MM-dd HH:mm:ss"),
                Status = "已揽收",
                Location = "上海市浦东新区",
                Description = "快件已揽收"
            }
        };

        return new ShipTrackDto
        {
            Company = entity.LogisticsCompany ?? "",
            ShipNo = entity.LogisticsNumber ?? "",
            Tracks = tracks,
            IsSigned = entity.Status == 3
        };
    }

    /// <summary>
    /// 获取订单统计
    /// </summary>
    public async Task<OrderStatisticsDto> GetStatisticsAsync()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var todayOrders = await _db.Queryable<Order>()
            .Where(o => o.CreateTime >= today && o.CreateTime < tomorrow)
            .ToListAsync();

        var pendingPayment = await _db.Queryable<Order>().Where(o => o.Status == 0).CountAsync();
        var pendingShip = await _db.Queryable<Order>().Where(o => o.Status == 1).CountAsync();
        var pendingReceive = await _db.Queryable<Order>().Where(o => o.Status == 2).CountAsync();

        // 获取售后中的数量
        var refunding = await _db.Queryable<Refund>()
            .Where(r => r.Status == "pending" || r.Status == "approved" || r.Status == "returning" || r.Status == "refunding")
            .CountAsync();

        return new OrderStatisticsDto
        {
            TodayCount = todayOrders.Count,
            TodayAmount = todayOrders.Sum(o => o.TotalAmount),
            PendingPaymentCount = pendingPayment,
            PendingShipCount = pendingShip,
            PendingReceiveCount = pendingReceive,
            RefundingCount = refunding
        };
    }

    /// <summary>
    /// 生成订单编号
    /// </summary>
    private string GenerateOrderNo()
    {
        return $"ORD{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
    }

    /// <summary>
    /// 导出订单
    /// </summary>
    public async Task<byte[]> ExportAsync(QueryOrderDto query)
    {
        // 获取订单列表（不分页）- 使用 WhereIF
        var queryable = _db.Queryable<Order>()
            .WhereIF(!string.IsNullOrEmpty(query.OrderNo), o => o.OrderNo.Contains(query.OrderNo!))
            .WhereIF(query.UserId.HasValue, o => o.UserId == query.UserId!.Value)
            .WhereIF(query.Status.HasValue, o => o.Status == query.Status!.Value)
            .WhereIF(!string.IsNullOrEmpty(query.StartTime), o => o.CreateTime >= DateTime.Parse(query.StartTime!))
            .WhereIF(!string.IsNullOrEmpty(query.EndTime), o => o.CreateTime < DateTime.Parse(query.EndTime!).AddDays(1))
            .OrderByDescending(o => o.CreateTime);

        var list = await queryable.ToListAsync();

        // 使用 CSV 格式导出（实际项目中可使用 NPOI 或 EPPlus 生成 Excel）
        var csvContent = "订单编号,用户ID,订单金额,状态,创建时间\n";
        foreach (var order in list)
        {
            csvContent += $"{order.OrderNo},{order.UserId},{order.TotalAmount},{OrderStatus.GetText(order.Status)},{order.CreateTime:yyyy-MM-dd HH:mm:ss}\n";
        }

        return System.Text.Encoding.UTF8.GetBytes(csvContent);
    }

    /// <summary>
    /// 获取订单评价列表
    /// </summary>
    public async Task<PageResponse<OrderReviewDto>> GetReviewListAsync(QueryOrderReviewDto query)
    {
        var queryable = _db.Queryable<ProductReview>()
            // 条件筛选 - 使用 WhereIF
            .WhereIF(!string.IsNullOrEmpty(query.OrderNo), r => r.OrderNo.Contains(query.OrderNo!))
            .WhereIF(query.ProductId.HasValue, r => r.ProductId == query.ProductId!.Value)
            .WhereIF(!string.IsNullOrEmpty(query.Status), r => r.Status == query.Status)
            .OrderByDescending(r => r.CreateTime);

        var total = new RefAsync<int>();
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        var dtoList = list.Select(r => new OrderReviewDto
        {
            Id = r.Id,
            OrderId = Guid.Empty,
            OrderNo = r.OrderNo,
            ProductId = r.ProductId,
            ProductName = r.ProductName,
            UserId = Guid.Empty,
            UserName = r.IsAnonymous ? "匿名用户" : r.UserName,
            UserAvatar = r.IsAnonymous ? null : r.UserAvatar,
            ProductQuality = r.Rating,
            DescriptionMatch = r.Rating,
            CostPerformance = r.Rating,
            ShippingSpeed = r.Rating,
            LogisticsService = r.Rating,
            CustomerService = r.Rating,
            OverallRating = r.Rating,
            Content = r.Content,
            Images = string.IsNullOrEmpty(r.Images) ? null : JsonSerializer.Deserialize<List<string>>(r.Images),
            IsAnonymous = r.IsAnonymous,
            Status = r.Status,
            CreateTime = r.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return PageResponse<OrderReviewDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取订单评价详情
    /// </summary>
    public async Task<OrderReviewDto?> GetReviewDetailAsync(Guid id)
    {
        var entity = await _db.Queryable<ProductReview>().FirstAsync(r => r.Id == id);
        if (entity == null) return null;

        return new OrderReviewDto
        {
            Id = entity.Id,
            OrderId = Guid.Empty,
            OrderNo = entity.OrderNo,
            ProductId = entity.ProductId,
            ProductName = entity.ProductName,
            UserId = Guid.Empty,
            UserName = entity.IsAnonymous ? "匿名用户" : entity.UserName,
            UserAvatar = entity.IsAnonymous ? null : entity.UserAvatar,
            ProductQuality = entity.Rating,
            DescriptionMatch = entity.Rating,
            CostPerformance = entity.Rating,
            ShippingSpeed = entity.Rating,
            LogisticsService = entity.Rating,
            CustomerService = entity.Rating,
            OverallRating = entity.Rating,
            Content = entity.Content,
            Images = string.IsNullOrEmpty(entity.Images) ? null : JsonSerializer.Deserialize<List<string>>(entity.Images),
            IsAnonymous = entity.IsAnonymous,
            Status = entity.Status,
            CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }
}