using EasyWeChatModels.Dto.Etl;

namespace BusinessManager.Buz.Etl.Engine;

/// <summary>
/// ETL 执行引擎接口
/// </summary>
public interface IEtlExecutionEngine
{
    /// <summary>
    /// 执行任务流（同步模式）
    /// 适合小任务，在当前线程执行，阻塞等待结果
    /// </summary>
    /// <param name="executionId">执行记录ID</param>
    /// <returns>执行结果</returns>
    Task<EtlExecutionResultDto> ExecuteSyncAsync(Guid executionId);

    /// <summary>
    /// 执行任务流（异步模式）
    /// 适合大任务，提交到后台执行，立即返回执行ID
    /// </summary>
    /// <param name="executionId">执行记录ID</param>
    /// <returns>执行记录ID（用于后续查询进度）</returns>
    Task<Guid> ExecuteAsyncAsync(Guid executionId);

    /// <summary>
    /// 取消执行
    /// </summary>
    /// <param name="executionId">执行记录ID</param>
    /// <returns>影响的记录数</returns>
    Task<int> CancelAsync(Guid executionId);

    /// <summary>
    /// 重试失败的节点
    /// </summary>
    /// <param name="executionId">执行记录ID</param>
    /// <param name="nodeId">节点ID</param>
    /// <returns>影响的记录数</returns>
    Task<int> RetryNodeAsync(Guid executionId, string nodeId);

    /// <summary>
    /// 获取执行进度
    /// </summary>
    /// <param name="executionId">执行记录ID</param>
    /// <returns>执行进度 DTO</returns>
    Task<EtlExecutionProgressDto> GetProgressAsync(Guid executionId);

    /// <summary>
    /// 获取执行结果
    /// </summary>
    /// <param name="executionId">执行记录ID</param>
    /// <returns>执行结果 DTO</returns>
    Task<EtlExecutionResultDto> GetResultAsync(Guid executionId);
}