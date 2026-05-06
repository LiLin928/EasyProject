namespace BusinessManager.Infrastructure.IService;

using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;

/// <summary>
/// 任务执行日志服务接口
/// </summary>
public interface ITaskExecutionLogService
{
    /// <summary>
    /// 分页查询执行日志
    /// </summary>
    Task<PageResponse<TaskExecutionLogDto>> GetPageListAsync(QueryTaskExecutionLogDto query);

    /// <summary>
    /// 获取日志详情
    /// </summary>
    Task<TaskExecutionLogDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 记录任务执行日志
    /// </summary>
    Task<Guid> LogExecutionAsync(TaskExecutionLog log);

    /// <summary>
    /// 更新任务执行结果
    /// </summary>
    Task UpdateExecutionResultAsync(Guid logId, int status, string? resultMessage, string? exceptionMessage, string? exceptionStackTrace);

    /// <summary>
    /// 清理过期日志
    /// </summary>
    Task<int> ClearAsync(int retentionDays);

    /// <summary>
    /// 获取执行趋势数据
    /// </summary>
    Task<TaskLogTrendDto> GetTrendAsync(int days = 7);

    /// <summary>
    /// 获取今日统计
    /// </summary>
    Task<TaskStatisticsDto> GetTodayStatisticsAsync();
}