namespace BusinessManager.Tasks.Handlers;

using EasyWeChatModels.Dto;
using Microsoft.Extensions.Logging;

/// <summary>
/// 示例即时任务处理器 - 演示业务触发的即时执行
/// </summary>
public class SampleImmediateTaskHandler
{
    /// <summary>
    /// 日志器（属性注入）
    /// </summary>
    public ILogger<SampleImmediateTaskHandler> _logger { get; set; } = null!;

    /// <summary>
    /// 执行即时任务
    /// </summary>
    public async Task<TaskExecutionResult> ExecuteAsync()
    {
        _logger.LogInformation("执行示例即时任务");

        // 模拟业务逻辑
        await Task.Delay(100);

        return TaskExecutionResult.Success("即时任务执行成功");
    }

    /// <summary>
    /// 执行带参数的即时任务
    /// </summary>
    public async Task<TaskExecutionResult> ExecuteWithParamAsync(string businessData)
    {
        _logger.LogInformation($"执行带参数的即时任务，数据: {businessData}");

        // 模拟业务逻辑
        await Task.Delay(100);

        return TaskExecutionResult.Success($"处理数据成功: {businessData}");
    }
}