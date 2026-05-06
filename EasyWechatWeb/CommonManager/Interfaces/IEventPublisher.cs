namespace CommonManager.Interfaces;

/// <summary>
/// 事件发布接口，用于分布式系统中的事件通信（CAP 预留）
/// </summary>
/// <remarks>
/// 该接口定义了事件发布的基本方法，为后续集成 DotNetCore.CAP 分布式事务框架预留。
/// CAP 是一个分布式事务解决方案，基于 EventBus 和数据库消息表实现最终一致性。
///
/// 应用场景：
/// - 微服务间的异步通信
/// - 分布式事务的最终一致性保证
/// - 系统解耦和事件驱动架构
/// - 消息通知和广播
///
/// 实现后可支持：
/// - RabbitMQ、Kafka 等消息队列
/// - 消息持久化和重试机制
/// - 消息消费确认和补偿
/// - 分布式事务协调
///
/// 预留此接口便于后续集成 CAP 框架，实现完整的分布式事件发布订阅能力。
/// </remarks>
/// <example>
/// <code>
/// // 发布用户注册事件（后续实现）
/// public class UserService : IUserService
/// {
///     private readonly IEventPublisher _eventPublisher;
///
///     public async Task RegisterUser(UserDto dto)
///     {
///         // 创建用户
///         var user = await CreateUserAsync(dto);
///
///         // 发布用户注册事件，通知其他服务
///         await _eventPublisher.PublishAsync("user.registered", new UserRegisteredEvent
///         {
///             UserId = user.Id,
///             UserName = user.UserName,
///             RegisterTime = DateTime.Now
///         });
///     }
/// }
///
/// // 消费端订阅事件
/// [CapSubscribe("user.registered")]
/// public async Task HandleUserRegistered(UserRegisteredEvent event)
/// {
///     // 处理用户注册事件
///     await SendWelcomeEmail(event.UserId);
/// }
/// </code>
/// </example>
public interface IEventPublisher
{
    /// <summary>
    /// 发布事件到指定主题
    /// </summary>
    /// <typeparam name="T">事件消息类型</typeparam>
    /// <param name="topic">主题名称，用于区分不同类型的事件，如 "user.registered"、"order.created"</param>
    /// <param name="message">事件消息内容，通常为包含事件数据的 DTO 对象</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// 事件发布后，订阅该主题的消费者会收到消息并处理。
    /// CAP 保证消息的可靠传递：
    /// - 消息持久化到数据库消息表
    /// - 发送失败时自动重试
    /// - 支持消息确认和补偿机制
    ///
    /// 主题命名建议：
    /// - 使用有意义的命名，如 "模块.事件名"
    /// - 示例："user.registered"、"order.paid"、"payment.completed"
    /// - 保持一致性，便于管理和监控
    /// </remarks>
    /// <example>
    /// <code>
    /// // 发布用户注册事件
    /// await _eventPublisher.PublishAsync("user.registered", new UserEvent
    /// {
    ///     UserId = 1,
    ///     UserName = "张三",
    ///     Action = "注册"
    /// });
    ///
    /// // 发布订单创建事件
    /// await _eventPublisher.PublishAsync("order.created", new OrderEvent
    /// {
    ///     OrderId = "12345",
    ///     UserId = 1,
    ///     Amount = 100.00
    /// });
    /// </code>
    /// </example>
    Task PublishAsync<T>(string topic, T message);

    /// <summary>
    /// 发布延迟事件，消息将在指定时间后送达
    /// </summary>
    /// <typeparam name="T">事件消息类型</typeparam>
    /// <param name="topic">主题名称</param>
    /// <param name="message">事件消息内容</param>
    /// <param name="delaySeconds">延迟时间（秒），消息将在此时间后发送</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// 延迟事件适用于需要定时执行的场景：
    /// - 定时任务调度（如订单超时取消）
    /// - 延迟通知（如注册后延迟发送欢迎邮件）
    /// - 状态检查（如支付后延迟检查支付结果）
    ///
    /// 实现原理：
    /// - RabbitMQ 使用 x-delayed-message 插件
    /// - Kafka 使用时间戳和消费者轮询
    /// - CAP 封装不同消息队列的实现细节
    /// </remarks>
    /// <example>
    /// <code>
    /// // 订单创建后 30 分钟检查支付状态
    /// await _eventPublisher.PublishDelayAsync("order.timeout.check", new OrderTimeoutEvent
    /// {
    ///     OrderId = "12345"
    /// }, 1800);  // 30 分钟 = 1800 秒
    ///
    /// // 用户注册后 5 分钟发送欢迎邮件
    /// await _eventPublisher.PublishDelayAsync("user.welcome.email", new UserEvent
    /// {
    ///     UserId = 1
    /// }, 300);  // 5 分钟 = 300 秒
    /// </code>
    /// </example>
    Task PublishDelayAsync<T>(string topic, T message, int delaySeconds);
}