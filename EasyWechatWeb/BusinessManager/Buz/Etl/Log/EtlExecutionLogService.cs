using CommonManager.Base;
using EasyWeChatModels.Entitys;
using Newtonsoft.Json;
using SqlSugar;

namespace BusinessManager.Buz.Etl.Log;

/// <summary>
/// ETL 执行日志服务实现
/// </summary>
public class EtlExecutionLogService : BaseService<EtlExecutionLog>, IEtlExecutionLogService
{
    /// <summary>
    /// 记录日志
    /// </summary>
    public async Task LogAsync(Guid executionId, string? nodeId, string? nodeName,
        string level, string message, string? detailData = null, string? step = null)
    {
        var fullMessage = message;
        if (!string.IsNullOrEmpty(detailData))
        {
            fullMessage = $"{message} | {detailData}";
        }
        if (!string.IsNullOrEmpty(step))
        {
            fullMessage = $"{fullMessage} [{step}]";
        }

        var log = new EtlExecutionLog
        {
            ExecutionId = executionId,
            NodeId = nodeId,
            NodeName = nodeName,
            Level = level,
            LogTime = DateTime.Now,
            Message = fullMessage
        };

        await _db.Insertable(log).ExecuteCommandAsync();
    }

    /// <summary>
    /// 记录信息日志
    /// </summary>
    public async Task LogInfoAsync(Guid executionId, string? nodeId, string? nodeName,
        string message, string? detailData = null, string? step = null)
    {
        await LogAsync(executionId, nodeId, nodeName, "info", message, detailData, step);
    }

    /// <summary>
    /// 记录警告日志
    /// </summary>
    public async Task LogWarningAsync(Guid executionId, string? nodeId, string? nodeName,
        string message, string? detailData = null, string? step = null)
    {
        await LogAsync(executionId, nodeId, nodeName, "warn", message, detailData, step);
    }

    /// <summary>
    /// 记录错误日志
    /// </summary>
    public async Task LogErrorAsync(Guid executionId, string? nodeId, string? nodeName,
        string message, string? detailData = null, string? step = null)
    {
        await LogAsync(executionId, nodeId, nodeName, "error", message, detailData, step);
    }

    /// <summary>
    /// 记录调试日志
    /// </summary>
    public async Task LogDebugAsync(Guid executionId, string? nodeId, string? nodeName,
        string message, string? detailData = null, string? step = null)
    {
        await LogAsync(executionId, nodeId, nodeName, "debug", message, detailData, step);
    }

    /// <summary>
    /// 获取日志列表
    /// </summary>
    public async Task<List<EtlExecutionLog>> GetLogsAsync(Guid executionId,
        string? nodeId = null, string? level = null)
    {
        return await _db.Queryable<EtlExecutionLog>()
            .Where(x => x.ExecutionId == executionId)
            .WhereIF(!string.IsNullOrEmpty(nodeId), x => x.NodeId == nodeId)
            .WhereIF(!string.IsNullOrEmpty(level), x => x.Level == level)
            .OrderBy(x => x.LogTime)
            .ToListAsync();
    }

    /// <summary>
    /// 清理过期日志
    /// </summary>
    public async Task<int> CleanExpiredLogsAsync(int retentionDays = 30)
    {
        var cutoffDate = DateTime.Now.AddDays(-retentionDays);

        return await _db.Deleteable<EtlExecutionLog>()
            .Where(x => x.LogTime < cutoffDate)
            .ExecuteCommandAsync();
    }
}