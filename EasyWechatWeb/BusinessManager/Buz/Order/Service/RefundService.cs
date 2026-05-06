using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using SqlSugar;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 售后服务实现
/// </summary>
public class RefundService : BaseService, IRefundService
{
    /// <summary>
    /// 获取售后分页列表
    /// </summary>
    public async Task<PageResponse<RefundDto>> GetPageListAsync(QueryRefundDto query)
    {
        var queryable = _db.Queryable<Refund>()
            // 条件筛选 - 使用 WhereIF
            .WhereIF(!string.IsNullOrEmpty(query.RefundNo), r => r.RefundNo.Contains(query.RefundNo!))
            .WhereIF(query.OrderId.HasValue, r => r.OrderId == query.OrderId!.Value)
            .WhereIF(!string.IsNullOrEmpty(query.OrderNo), r => r.OrderNo.Contains(query.OrderNo!))
            .WhereIF(!string.IsNullOrEmpty(query.Type), r => r.Type == query.Type)
            .WhereIF(!string.IsNullOrEmpty(query.Status), r => r.Status == query.Status)
            .WhereIF(!string.IsNullOrEmpty(query.StartTime), r => r.CreateTime >= DateTime.Parse(query.StartTime!))
            .WhereIF(!string.IsNullOrEmpty(query.EndTime), r => r.CreateTime < DateTime.Parse(query.EndTime!).AddDays(1))
            .OrderByDescending(r => r.CreateTime);

        // 分页
        var total = new RefAsync<int>();
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 获取售后商品
        var refundIds = list.Select(r => r.Id).ToList();
        var refundItems = await _db.Queryable<RefundItem>()
            .Where(i => refundIds.Contains(i.RefundId))
            .ToListAsync();
        var exchangeItems = await _db.Queryable<ExchangeItem>()
            .Where(i => refundIds.Contains(i.RefundId))
            .ToListAsync();

        // 转换 DTO
        var dtoList = list.Select(r => new RefundDto
        {
            Id = r.Id,
            RefundNo = r.RefundNo,
            OrderId = r.OrderId,
            OrderNo = r.OrderNo,
            Type = r.Type,
            RefundAmount = r.RefundAmount,
            Reason = r.Reason,
            Description = r.Description,
            Status = r.Status,
            Items = refundItems.Where(i => i.RefundId == r.Id).Select(i => new RefundItemDto
            {
                Id = i.Id,
                RefundId = i.RefundId,
                OrderItemId = i.OrderItemId,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductImage = i.ProductImage,
                Price = i.Price,
                Quantity = i.Quantity,
                Amount = i.Amount
            }).ToList(),
            ExchangeItems = exchangeItems.Where(i => i.RefundId == r.Id).Select(i => new ExchangeItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductImage = i.ProductImage,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList(),
            ReturnShipCompany = r.ReturnShipCompany,
            ReturnShipNo = r.ReturnShipNo,
            ReturnShipTime = r.ReturnShipTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            ExchangeShipCompany = r.ExchangeShipCompany,
            ExchangeShipNo = r.ExchangeShipNo,
            ExchangeShipTime = r.ExchangeShipTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            Approver = r.Approver,
            ApproveTime = r.ApproveTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            ApproveRemark = r.ApproveRemark,
            RefundTime = r.RefundTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            CompleteTime = r.CompleteTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            CreateTime = r.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return PageResponse<RefundDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 根据ID获取售后详情
    /// </summary>
    public async Task<RefundDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<Refund>()
            .FirstAsync(r => r.Id == id);

        if (entity == null) return null;

        // 获取售后商品
        var refundItems = await _db.Queryable<RefundItem>()
            .Where(i => i.RefundId == id)
            .ToListAsync();
        var exchangeItems = await _db.Queryable<ExchangeItem>()
            .Where(i => i.RefundId == id)
            .ToListAsync();

        return new RefundDto
        {
            Id = entity.Id,
            RefundNo = entity.RefundNo,
            OrderId = entity.OrderId,
            OrderNo = entity.OrderNo,
            Type = entity.Type,
            RefundAmount = entity.RefundAmount,
            Reason = entity.Reason,
            Description = entity.Description,
            Status = entity.Status,
            Items = refundItems.Select(i => new RefundItemDto
            {
                Id = i.Id,
                RefundId = i.RefundId,
                OrderItemId = i.OrderItemId,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductImage = i.ProductImage,
                Price = i.Price,
                Quantity = i.Quantity,
                Amount = i.Amount
            }).ToList(),
            ExchangeItems = exchangeItems.Select(i => new ExchangeItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductImage = i.ProductImage,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList(),
            ReturnShipCompany = entity.ReturnShipCompany,
            ReturnShipNo = entity.ReturnShipNo,
            ReturnShipTime = entity.ReturnShipTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            ExchangeShipCompany = entity.ExchangeShipCompany,
            ExchangeShipNo = entity.ExchangeShipNo,
            ExchangeShipTime = entity.ExchangeShipTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            Approver = entity.Approver,
            ApproveTime = entity.ApproveTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            ApproveRemark = entity.ApproveRemark,
            RefundTime = entity.RefundTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            CompleteTime = entity.CompleteTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    /// <summary>
    /// 通过审核
    /// </summary>
    public async Task<int> ApproveAsync(ApproveRefundDto dto)
    {
        var entity = await _db.Queryable<Refund>().FirstAsync(r => r.Id == dto.Id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("售后记录不存在");
        }

        if (entity.Status != "pending")
        {
            throw new CommonManager.Error.BusinessException("售后状态不允许审核");
        }

        entity.Status = entity.Type == "refund_only" ? "refunding" : "approved";
        entity.Approver = "admin";
        entity.ApproveTime = DateTime.Now;
        entity.ApproveRemark = dto.Remark;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 拒绝审核
    /// </summary>
    public async Task<int> RejectAsync(RejectRefundDto dto)
    {
        var entity = await _db.Queryable<Refund>().FirstAsync(r => r.Id == dto.Id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("售后记录不存在");
        }

        if (entity.Status != "pending")
        {
            throw new CommonManager.Error.BusinessException("售后状态不允许审核");
        }

        entity.Status = "rejected";
        entity.Approver = "admin";
        entity.ApproveTime = DateTime.Now;
        entity.ApproveRemark = dto.Remark;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 确认收到退货
    /// </summary>
    public async Task<int> ConfirmReceiveAsync(Guid id, string? remark)
    {
        var entity = await _db.Queryable<Refund>().FirstAsync(r => r.Id == id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("售后记录不存在");
        }

        if (entity.Status != "returning")
        {
            throw new CommonManager.Error.BusinessException("售后状态不允许确认收货");
        }

        entity.Status = entity.Type == "exchange" ? "exchanging" : "refunding";
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 换货发货
    /// </summary>
    public async Task<int> ExchangeShipAsync(ExchangeShipDto dto)
    {
        var entity = await _db.Queryable<Refund>().FirstAsync(r => r.Id == dto.Id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("售后记录不存在");
        }

        if (entity.Status != "exchanging")
        {
            throw new CommonManager.Error.BusinessException("售后状态不允许换货发货");
        }

        entity.ExchangeShipCompany = dto.ShipCompany;
        entity.ExchangeShipNo = dto.ShipNo;
        entity.ExchangeShipTime = DateTime.Now;
        entity.Status = "completing";
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 完成售后
    /// </summary>
    public async Task<int> CompleteAsync(Guid id)
    {
        var entity = await _db.Queryable<Refund>().FirstAsync(r => r.Id == id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("售后记录不存在");
        }

        if (entity.Status != "refunding" && entity.Status != "completing")
        {
            throw new CommonManager.Error.BusinessException("售后状态不允许完成");
        }

        entity.Status = "completed";
        entity.CompleteTime = DateTime.Now;
        entity.UpdateTime = DateTime.Now;

        // 更新订单退款金额
        if (entity.Type != "exchange")
        {
            var order = await _db.Queryable<Order>().FirstAsync(o => o.Id == entity.OrderId);
            if (order != null)
            {
                order.RefundAmount = (order.RefundAmount ?? 0) + entity.RefundAmount;
                order.Status = 5; // 已退款
                order.UpdateTime = DateTime.Now;
                await _db.Updateable(order).ExecuteCommandAsync();
            }
        }

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 创建售后申请（后台代客申请）
    /// </summary>
    public async Task<Guid> CreateAsync(CreateRefundDto dto)
    {
        // 检查订单是否存在
        var order = await _db.Queryable<Order>().FirstAsync(o => o.Id == dto.OrderId);
        if (order == null)
        {
            throw new CommonManager.Error.BusinessException("订单不存在");
        }

        // 检查订单状态是否允许售后
        if (order.Status != 1 && order.Status != 2 && order.Status != 3)
        {
            throw new CommonManager.Error.BusinessException("订单状态不允许申请售后");
        }

        // 生成售后编号
        var refundNo = $"RF{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100, 999)}";

        // 创建售后记录
        var refund = new Refund
        {
            Id = Guid.NewGuid(),
            RefundNo = refundNo,
            OrderId = dto.OrderId,
            OrderNo = order.OrderNo,
            Type = dto.Type,
            RefundAmount = dto.RefundAmount ?? 0,
            Reason = dto.Reason,
            Description = dto.Description,
            Status = "pending",
            CreateTime = DateTime.Now
        };

        // 插入售后记录
        await _db.Insertable(refund).ExecuteCommandAsync();

        // 创建售后商品明细
        var refundItems = dto.Items.Select(item => new RefundItem
        {
            Id = Guid.NewGuid(),
            RefundId = refund.Id,
            OrderItemId = item.OrderItemId,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            ProductImage = item.ProductImage,
            Price = item.Price,
            Quantity = item.Quantity,
            Amount = item.Price * item.Quantity
        }).ToList();

        if (refundItems.Count > 0)
        {
            await _db.Insertable(refundItems).ExecuteCommandAsync();
        }

        // 创建换货商品（换货类型时）
        if (dto.Type == "exchange" && dto.ExchangeItems != null && dto.ExchangeItems.Count > 0)
        {
            var exchangeItems = dto.ExchangeItems.Select(item => new ExchangeItem
            {
                Id = Guid.NewGuid(),
                RefundId = refund.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductImage = item.ProductImage,
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList();

            await _db.Insertable(exchangeItems).ExecuteCommandAsync();
        }

        return refund.Id;
    }

    /// <summary>
    /// 获取换货物流详情
    /// </summary>
    public async Task<ExchangeShipDetailDto> GetExchangeShipDetailAsync(Guid id)
    {
        var entity = await _db.Queryable<Refund>().FirstAsync(r => r.Id == id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("售后记录不存在");
        }

        if (entity.Type != "exchange")
        {
            throw new CommonManager.Error.BusinessException("该售后记录不是换货类型");
        }

        // 获取换货商品列表
        var exchangeItems = await _db.Queryable<ExchangeItem>()
            .Where(i => i.RefundId == id)
            .ToListAsync();

        // 构造物流详情（实际项目中可对接物流查询API）
        var detail = new ExchangeShipDetailDto
        {
            RefundId = entity.Id,
            RefundNo = entity.RefundNo,
            ShipCompany = entity.ExchangeShipCompany ?? string.Empty,
            ShipNo = entity.ExchangeShipNo ?? string.Empty,
            ShipTime = entity.ExchangeShipTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            Items = exchangeItems.Select(i => new ExchangeItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductImage = i.ProductImage,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList(),
            Tracks = new List<ShipTrackItemDto>() // 实际项目中对接物流查询API获取轨迹
        };

        // 如果已发货，添加物流轨迹模拟数据
        if (!string.IsNullOrEmpty(entity.ExchangeShipNo))
        {
            detail.Tracks = new List<ShipTrackItemDto>
            {
                new ShipTrackItemDto
                {
                    Time = entity.ExchangeShipTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty,
                    Status = "已发货",
                    Description = $"已从发货地发出，快递公司：{entity.ExchangeShipCompany}，单号：{entity.ExchangeShipNo}"
                }
            };
        }

        return detail;
    }
}