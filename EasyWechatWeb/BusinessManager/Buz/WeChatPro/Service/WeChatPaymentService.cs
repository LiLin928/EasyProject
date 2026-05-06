using BusinessManager.Buz.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 微信支付服务实现类
/// </summary>
/// <remarks>
/// 实现支付相关的业务逻辑，包括创建支付记录等功能。
/// 当前使用Mock实现，模拟支付流程。
/// </remarks>
public class WeChatPaymentService : BaseService<Payment>, IWeChatPaymentService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatPaymentService> _logger { get; set; } = null!;

    /// <summary>
    /// 创建支付
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="dto">支付信息，包含订单ID和支付方式</param>
    /// <returns>支付结果</returns>
    /// <exception cref="BusinessException">
    /// 订单不存在、订单不属于当前用户、订单状态不正确时抛出相应异常
    /// </exception>
    /// <remarks>
    /// Mock实现流程：
    /// <list type="number">
    ///     <item>验证订单存在和状态</item>
    ///     <item>创建支付记录</item>
    ///     <item>模拟支付成功</item>
    ///     <item>更新订单状态为已支付</item>
    /// </list>
    /// </remarks>
    public async Task<PaymentResultDto> CreatePaymentAsync(Guid userId, CreatePaymentDto dto)
    {
        // 验证订单
        var order = await _db.Queryable<Order>()
            .Where(o => o.Id == dto.OrderId && o.UserId == userId)
            .FirstAsync();

        if (order == null)
        {
            throw BusinessException.NotFound("订单不存在");
        }

        if (order.Status != OrderStatus.Pending)
        {
            throw BusinessException.BadRequest("订单状态不正确，无法支付");
        }

        // 生成支付单号
        var paymentNo = GeneratePaymentNo();

        // 使用事务处理
        var paymentId = Guid.Empty;

        await ExecuteTransactionAsync(async () =>
        {
            // 创建支付记录
            var payment = new Payment
            {
                PaymentNo = paymentNo,
                OrderId = dto.OrderId,
                UserId = userId,
                Amount = order.TotalAmount,
                PaymentMethod = dto.PaymentMethod,
                Status = 1, // Mock: 直接设置为支付成功
                ThirdPartyPaymentId = $"MOCK_{Guid.NewGuid():N}",
                PaymentTime = DateTime.Now,
                Message = "Mock支付成功",
                CreateTime = DateTime.Now
            };

            await _db.Insertable(payment).ExecuteCommandAsync();
            paymentId = payment.Id;

            // 更新订单状态
            order.Status = OrderStatus.Paid;
            order.PaymentTime = DateTime.Now;
            order.UpdateTime = DateTime.Now;
            await _db.Updateable(order).ExecuteCommandAsync();
        });

        _logger.LogInformation("用户 {UserId} 支付订单 {OrderId} 成功，支付单号 {PaymentNo}", userId, dto.OrderId, paymentNo);

        return new PaymentResultDto
        {
            Success = true,
            OrderId = dto.OrderId,
            PaymentId = paymentId,
            Message = "支付成功"
        };
    }

    /// <summary>
    /// 生成支付单号
    /// </summary>
    /// <returns>支付单号字符串</returns>
    /// <remarks>
    /// 格式：PAY + 日期(8位) + 时间(6位) + 随机数(6位) = 23位
    /// </remarks>
    private string GeneratePaymentNo()
    {
        var now = DateTime.Now;
        var random = new Random();
        return $"PAY{now:yyyyMMddHHmmss}{random.Next(100000, 999999)}";
    }
}