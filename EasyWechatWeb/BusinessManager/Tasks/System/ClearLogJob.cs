namespace BusinessManager.Tasks.System;

using BusinessManager.Buz.IService;
using InfrastructureManager.Jobs;
using Microsoft.Extensions.Logging;
using global::Quartz;

/// <summary>
/// 清理操作日志任务
/// </summary>
public class ClearLogJob : IJobBase
{
    /// <summary>
    /// 任务名称
    /// </summary>
    public string JobName => "ClearLogJob";

    /// <summary>
    /// 任务分组
    /// </summary>
    public string JobGroup => "System";

    /// <summary>
    /// Cron 表达式（每天凌晨 2 点）
    /// </summary>
    public string CronExpression => "0 0 2 * * ?";

    /// <summary>
    /// 任务描述
    /// </summary>
    public string Description => "清理超过 30 天的操作日志";

    /// <summary>
    /// 操作日志服务（属性注入）
    /// </summary>
    public IOperateLogService _logService { get; set; } = null!;

    /// <summary>
    /// 日志器（属性注入）
    /// </summary>
    public ILogger<ClearLogJob> _logger { get; set; } = null!;

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("开始清理操作日志...");
        try
        {
            await _logService.ClearAsync(30);
            _logger.LogInformation("操作日志清理完成");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清理操作日志失败");
            throw;
        }
    }
}