namespace BusinessManager.Tasks.Handlers;

using BusinessManager.Infrastructure.IService;
using EasyWeChatModels.Dto;
using Microsoft.Extensions.Logging;

/// <summary>
/// 示例周期任务处理器 - 演示每天定时执行
/// </summary>
public class SamplePeriodicTaskHandler
{
    /// <summary>
    /// 日志器（属性注入）
    /// </summary>
    public ILogger<SamplePeriodicTaskHandler> _logger { get; set; } = null!;

    /// <summary>
    /// 任务定义服务（属性注入）
    /// </summary>
    public ITaskDefinitionService _taskService { get; set; } = null!;

    /// <summary>
    /// 执行周期任务
    /// </summary>
    public async Task<TaskExecutionResult> ExecuteAsync()
    {
        _logger.LogInformation("执行示例周期任务 - 每天清理过期数据");

        // 模拟清理过期数据
        // 实际业务中可以调用: await _taskService.ClearExpiredTasksAsync(30);

        await Task.Delay(200);

        _logger.LogInformation("周期任务执行完成");

        return TaskExecutionResult.Success("清理过期数据成功");
    }
}