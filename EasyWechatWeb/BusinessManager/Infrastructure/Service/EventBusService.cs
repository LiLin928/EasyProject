namespace BusinessManager.Infrastructure.Service;

using BusinessManager.Infrastructure.IService;
using DotNetCore.CAP;
using EasyWeChatModels.Dto;
using Microsoft.Extensions.Logging;

/// <summary>
/// 事件总线服务实现 - 增强版 CAP 服务
/// </summary>
/// <remarks>
/// 当 CAP 禁用时（Cap.Enabled = false），_capPublisher 为 null，
/// 此时 PublishAsync 方法会跳过发布，仅记录日志。
/// </remarks>
public class EventBusService : IEventBusService
{
    /// <summary>
    /// CAP 发布器（属性注入，可能为 null 如果 CAP 未启用）
    /// </summary>
    public ICapPublisher? _capPublisher { get; set; }

    /// <summary>
    /// 日志器（属性注入，可能为 null）
    /// </summary>
    public ILogger<EventBusService>? _logger { get; set; }

    /// <summary>
    /// 发布事件
    /// </summary>
    public async Task PublishAsync<T>(string topic, T message)
    {
        // CAP 未启用时跳过发布
        if (_capPublisher == null)
        {
            _logger?.LogInformation("CAP 未启用，跳过事件发布: {Topic}", topic);
            return;
        }

        _logger?.LogInformation("发布事件: {Topic}", topic);
        await _capPublisher.PublishAsync(topic, message);
    }

    /// <summary>
    /// 发布事件（带任务关联）
    /// </summary>
    public async Task PublishWithTaskAsync<T>(string topic, T message, Guid? taskDefinitionId = null)
    {
        // CAP 未启用时跳过发布
        if (_capPublisher == null)
        {
            _logger?.LogInformation("CAP 未启用，跳过任务事件发布: {Topic}, TaskId: {TaskId}", topic, taskDefinitionId);
            return;
        }

        var headers = new Dictionary<string, string?>();

        if (taskDefinitionId.HasValue)
        {
            headers["TaskDefinitionId"] = taskDefinitionId.Value.ToString();
        }

        _logger?.LogInformation("发布任务事件: {Topic}, TaskId: {TaskId}", topic, taskDefinitionId);
        await _capPublisher.PublishAsync(topic, message, headers);
    }

    /// <summary>
    /// 发布 API 回调事件
    /// </summary>
    public async Task PublishApiCallbackAsync(ApiCallbackPayload payload)
    {
        // CAP 未启用时跳过发布
        if (_capPublisher == null)
        {
            _logger?.LogInformation("CAP 未启用，跳过 API 回调事件: {Endpoint}", payload.Endpoint);
            return;
        }

        _logger?.LogInformation("发布 API 回调事件: {Endpoint}", payload.Endpoint);
        await _capPublisher.PublishAsync("api.callback.execute", payload);
    }
}