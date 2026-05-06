namespace BusinessManager.Infrastructure.IService;

using EasyWeChatModels.Dto;

/// <summary>
/// 事件总线服务接口 - 增强版 CAP 服务
/// </summary>
public interface IEventBusService
{
    /// <summary>
    /// 发布事件
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="topic">主题</param>
    /// <param name="message">消息内容</param>
    Task PublishAsync<T>(string topic, T message);

    /// <summary>
    /// 发布事件（带任务关联）
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="topic">主题</param>
    /// <param name="message">消息内容</param>
    /// <param name="taskDefinitionId">任务定义ID</param>
    Task PublishWithTaskAsync<T>(string topic, T message, Guid? taskDefinitionId = null);

    /// <summary>
    /// 发布 API 回调事件
    /// </summary>
    /// <param name="payload">API 回调载荷</param>
    Task PublishApiCallbackAsync(ApiCallbackPayload payload);
}