using EasyWeChatModels.Entitys;

namespace BusinessManager.Buz.Etl.Log;

/// <summary>
/// ETL 执行日志服务接口
/// </summary>
public interface IEtlExecutionLogService
{
    /// <summary>
    /// 记录日志
    /// </summary>
    /// <param name="executionId">执行记录ID</param>
    /// <param name="nodeId">节点ID</param>
    /// <param name="nodeName">节点名称</param>
    /// <param name="level">日志级别</param>
    /// <param name="message">日志消息</param>
    /// <param name="detailData">详细数据（可选）</param>
    /// <param name="step">执行步骤（可选）</param>
    Task LogAsync(Guid executionId, string? nodeId, string? nodeName,
        string level, string message, string? detailData = null, string? step = null);

    /// <summary>
    /// 记录信息日志
    /// </summary>
    Task LogInfoAsync(Guid executionId, string? nodeId, string? nodeName,
        string message, string? detailData = null, string? step = null);

    /// <summary>
    /// 记录警告日志
    /// </summary>
    Task LogWarningAsync(Guid executionId, string? nodeId, string? nodeName,
        string message, string? detailData = null, string? step = null);

    /// <summary>
    /// 记录错误日志
    /// </summary>
    Task LogErrorAsync(Guid executionId, string? nodeId, string? nodeName,
        string message, string? detailData = null, string? step = null);

    /// <summary>
    /// 记录调试日志
    /// </summary>
    Task LogDebugAsync(Guid executionId, string? nodeId, string? nodeName,
        string message, string? detailData = null, string? step = null);

    /// <summary>
    /// 获取日志列表
    /// </summary>
    /// <param name="executionId">执行记录ID</param>
    /// <param name="nodeId">节点ID（可选）</param>
    /// <param name="level">日志级别（可选）</param>
    /// <returns>日志列表</returns>
    Task<List<EtlExecutionLog>> GetLogsAsync(Guid executionId,
        string? nodeId = null, string? level = null);

    /// <summary>
    /// 清理过期日志
    /// </summary>
    /// <param name="retentionDays">保留天数</param>
    /// <returns>删除的记录数</returns>
    Task<int> CleanExpiredLogsAsync(int retentionDays = 30);
}