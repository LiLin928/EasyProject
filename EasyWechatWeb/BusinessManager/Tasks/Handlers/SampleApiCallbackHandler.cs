namespace BusinessManager.Tasks.Handlers;

using EasyWeChatModels.Dto;
using Microsoft.Extensions.Logging;
using global::System.Net.Http;
using global::System.Text;

/// <summary>
/// API 回调任务处理器 - 演示任务完成后调用外部 API
/// </summary>
public class SampleApiCallbackHandler
{
    /// <summary>
    /// 日志器（属性注入）
    /// </summary>
    public ILogger<SampleApiCallbackHandler> _logger { get; set; } = null!;

    /// <summary>
    /// HTTP 客户端工厂（属性注入）
    /// </summary>
    public IHttpClientFactory _httpClientFactory { get; set; } = null!;

    /// <summary>
    /// 执行任务并调用 API 回调
    /// </summary>
    public async Task<TaskExecutionResult> ExecuteAsync(string businessData)
    {
        _logger.LogInformation($"执行 API 回调任务，数据: {businessData}");

        try
        {
            // 1. 执行业务逻辑
            await Task.Delay(100);

            // 2. 调用外部 API（示例）
            // 实际使用时，可以从 businessData 中解析 API 地址和参数
            var httpClient = _httpClientFactory.CreateClient();
            var payload = new { Message = "任务完成", Timestamp = DateTime.Now };
            var content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(payload),
                Encoding.UTF8,
                "application/json"
            );

            // 示例：调用一个假设的 API
            // var response = await httpClient.PostAsync("https://example.com/api/callback", content);

            _logger.LogInformation("API 回调任务执行成功");

            return TaskExecutionResult.Success("任务执行并调用 API 成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API 回调任务执行失败");
            return TaskExecutionResult.Failed(ex.Message);
        }
    }
}