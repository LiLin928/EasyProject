namespace BusinessManager.Events;

using BusinessManager.Infrastructure.IService;
using DotNetCore.CAP;
using EasyWeChatModels.Dto.Infrastructure;
using Microsoft.Extensions.Logging;

/// <summary>
/// 业务撤销处理器 - 处理任务失败后的业务撤销操作
/// </summary>
/// <remarks>
/// 具体业务模块可以订阅 task.business.rollback 事件执行自定义撤销逻辑
/// </remarks>
public class BusinessRollbackHandler : ICapSubscribe
{
    /// <summary>
    /// 日志器（属性注入）
    /// </summary>
    public ILogger<BusinessRollbackHandler> _logger { get; set; } = null!;

    /// <summary>
    /// 处理业务撤销事件
    /// </summary>
    /// <remarks>
    /// 这是一个通用的撤销处理器，具体业务模块应该实现自己的撤销逻辑
    /// </remarks>
    [CapSubscribe("task.business.rollback")]
    public async Task HandleBusinessRollback(TaskCompensationDto compensation)
    {
        _logger.LogInformation(
            "收到业务撤销请求 - 任务: {TaskName}, 任务ID: {TaskId}, 补偿类型: {CompensationType}",
            compensation.TaskName, compensation.TaskId, compensation.CompensationType
        );

        // 通用撤销逻辑示例：
        // 1. 检查业务数据中的操作类型
        // 2. 执行对应的撤销操作
        // 3. 记录撤销结果

        // 具体业务模块应该创建自己的处理器，例如：
        // - OrderRollbackHandler: 处理订单相关任务的撤销
        // - PaymentRollbackHandler: 处理支付相关任务的撤销
        // - InventoryRollbackHandler: 处理库存相关任务的撤销

        await Task.CompletedTask;
    }
}