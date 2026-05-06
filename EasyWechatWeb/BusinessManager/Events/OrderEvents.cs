namespace BusinessManager.Events;

using DotNetCore.CAP;
using Microsoft.Extensions.Logging;

/// <summary>
/// 订单相关事件处理器（示例）
/// </summary>
public class OrderEvents : ICapSubscribe
{
    /// <summary>
    /// 日志器（属性注入）
    /// </summary>
    public ILogger<OrderEvents> _logger { get; set; } = null!;

    /// <summary>
    /// 订单创建事件处理（示例）
    /// </summary>
    [CapSubscribe("order.created")]
    public async Task OnOrderCreated(string message)
    {
        _logger.LogInformation($"收到订单创建事件: {message}");
        // TODO: 实现具体的业务逻辑，如扣减库存等
        await Task.CompletedTask;
    }

    /// <summary>
    /// 订单支付成功事件处理（示例）
    /// </summary>
    [CapSubscribe("order.paid")]
    public async Task OnOrderPaid(string message)
    {
        _logger.LogInformation($"收到订单支付事件: {message}");
        // TODO: 实现具体的业务逻辑，如发放积分等
        await Task.CompletedTask;
    }
}