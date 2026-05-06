namespace InfrastructureManager.CAP;

using DotNetCore.CAP;
using EasyWeChatModels.Dto;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;

/// <summary>
/// API 回调消费者 - 执行 HTTP 回调请求
/// </summary>
public class ApiCallbackConsumer : ICapSubscribe
{
    /// <summary>
    /// HTTP 客户端（属性注入）
    /// </summary>
    public HttpClient _httpClient { get; set; } = null!;

    /// <summary>
    /// 日志器（属性注入）
    /// </summary>
    public ILogger<ApiCallbackConsumer> _logger { get; set; } = null!;

    /// <summary>
    /// 执行 API 回调
    /// </summary>
    [CapSubscribe("api.callback.execute")]
    public async Task ExecuteCallback(ApiCallbackPayload payload)
    {
        try
        {
            _logger.LogInformation($"开始执行 API 回调: {payload.Endpoint}, TaskId: {payload.TaskId}");

            var request = new HttpRequestMessage
            {
                Method = new HttpMethod(payload.Method ?? "POST"),
                RequestUri = new Uri(payload.Endpoint)
            };

            if (!string.IsNullOrEmpty(payload.Payload))
            {
                request.Content = new StringContent(payload.Payload, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"API 回调执行成功: {payload.Endpoint}, 状态码: {response.StatusCode}");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"API 回调执行失败: {payload.Endpoint}, 状态码: {response.StatusCode}, 错误: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"API 回调执行异常: {payload.Endpoint}");
            throw; // 让 CAP 自动重试
        }
    }
}