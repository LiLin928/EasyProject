namespace InfrastructureManager.CAP;

using DotNetCore.CAP;
using System.Collections.Generic;

/// <summary>
/// 事件总线接口
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// 发布事件
    /// </summary>
    Task PublishAsync<T>(string topic, T message);

    /// <summary>
    /// 发布事件（带附加头信息）
    /// </summary>
    Task PublishAsync<T>(string topic, T message, IDictionary<string, string?>? headers);
}

/// <summary>
/// 事件总线实现
/// </summary>
public class EventBus : IEventBus
{
    /// <summary>
    /// CAP 发布器（属性注入）
    /// </summary>
    public ICapPublisher _capPublisher { get; set; } = null!;

    public async Task PublishAsync<T>(string topic, T message)
    {
        await _capPublisher.PublishAsync(topic, message);
    }

    public async Task PublishAsync<T>(string topic, T message, IDictionary<string, string?>? headers)
    {
        await _capPublisher.PublishAsync(topic, message, headers ?? new Dictionary<string, string?>());
    }
}