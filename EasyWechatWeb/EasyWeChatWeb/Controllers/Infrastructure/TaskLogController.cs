using BusinessManager.Infrastructure.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Infrastructure;

/// <summary>
/// 任务执行日志控制器
/// </summary>
/// <remarks>
/// 提供任务执行日志的管理功能，包括日志查询、详情查看、清理、趋势分析和统计等。
/// 用于监控任务的执行情况，排查执行异常。
/// 所有接口需要授权访问。
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskLogController : BaseController
{
    /// <summary>
    /// 任务执行日志服务接口
    /// </summary>
    public ITaskExecutionLogService _taskExecutionLogService { get; set; } = null!;
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<TaskLogController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取任务执行日志列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页后的任务执行日志列表</returns>
    /// <response code="200">成功获取任务执行日志列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 支持多种筛选条件进行日志查询：
    /// 查询结果按创建时间倒序排列，最新的日志排在最前面。
    /// </remarks>
    /// <example>
    /// POST /api/tasklog/list
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "pageIndex": 1,
    ///     "pageSize": 10,
    ///     "jobName": "数据同步任务",
    ///     "status": 1,
    ///     "startTimeBegin": "2024-01-01",
    ///     "startTimeEnd": "2024-01-31"
    /// }
    /// </example>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<TaskExecutionLogDto>>), 200)]
    public async Task<ApiResponse<PageResponse<TaskExecutionLogDto>>> GetList([FromBody] QueryTaskExecutionLogDto query)
    {
        try
        {
            var result = await _taskExecutionLogService.GetPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取任务执行日志列表失败");
            return Error<PageResponse<TaskExecutionLogDto>>("获取任务执行日志列表失败");
        }
    }

    /// <summary>
    /// 获取任务执行日志详情
    /// </summary>
    /// <param name="id">任务执行日志ID</param>
    /// <returns>任务执行日志详细信息</returns>
    /// <response code="200">成功获取任务执行日志详情</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">任务执行日志不存在</response>
    /// <remarks>
    /// 根据日志ID获取完整的任务执行日志信息，包括：
    /// <list type="bullet">
    ///     <item>任务信息：任务名称、任务分组</item>
    ///     <item>执行信息：执行状态、开始时间、结束时间、执行时长</item>
    ///     <item>结果详情：执行结果、异常信息</item>
    ///     <item>触发信息：触发类型、执行实例ID</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// GET /api/tasklog/detail/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<TaskExecutionLogDto>), 200)]
    public async Task<ApiResponse<TaskExecutionLogDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _taskExecutionLogService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<TaskExecutionLogDto>("任务执行日志不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取任务执行日志详情失败: {Id}", id);
            return Error<TaskExecutionLogDto>("获取任务执行日志详情失败");
        }
    }

    /// <summary>
    /// 清理过期任务执行日志
    /// </summary>
    /// <param name="retentionDays">日志保留天数，超过此天数的日志将被删除，默认为30天</param>
    /// <returns>删除的日志数量</returns>
    /// <response code="200">清理成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 清理超过指定天数的任务执行日志记录：
    /// <list type="bullet">
    ///     <item>默认保留30天的日志</item>
    ///     <item>清理操作是物理删除，数据将永久移除</item>
    ///     <item>建议定期执行清理，避免数据量过大影响性能</item>
    ///     <item>通常只有管理员才有权限执行清理操作</item>
    /// </list>
    /// 清理流程：
    /// <list type="number">
    ///     <item>计算保留截止时间（当前时间 - retentionDays）</item>
    ///     <item>删除创建时间早于截止时间的所有日志</item>
    ///     <item>返回删除的记录数量</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// POST /api/tasklog/clear?retentionDays=30
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("clear")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Clear([FromQuery] int retentionDays = 30)
    {
        try
        {
            var result = await _taskExecutionLogService.ClearAsync(retentionDays);
            return Success(result, $"清理完成，删除了 {result} 条日志");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            _logger.LogWarning(ex, "清理任务执行日志失败");
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清理任务执行日志失败");
            return Error<int>("清理任务执行日志失败");
        }
    }

    /// <summary>
    /// 获取任务执行趋势数据
    /// </summary>
    /// <param name="days">统计天数，默认为7天</param>
    /// <returns>执行趋势数据</returns>
    /// <response code="200">成功获取执行趋势数据</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取最近指定天数的任务执行趋势数据，用于分析任务执行情况：
    /// <list type="bullet">
    ///     <item>按日期分组统计每日执行次数</item>
    ///     <item>统计每日成功次数和成功率</item>
    ///     <item>数据按日期升序排列</item>
    /// </list>
    /// 适合用于绘制趋势图表。
    /// </remarks>
    /// <example>
    /// GET /api/tasklog/trend?days=7
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("trend")]
    [ProducesResponseType(typeof(ApiResponse<TaskLogTrendDto>), 200)]
    public async Task<ApiResponse<TaskLogTrendDto>> GetTrend([FromQuery] int days = 7)
    {
        try
        {
            var result = await _taskExecutionLogService.GetTrendAsync(days);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取任务执行趋势失败");
            return Error<TaskLogTrendDto>("获取任务执行趋势失败");
        }
    }

    /// <summary>
    /// 获取任务执行统计数据
    /// </summary>
    /// <returns>任务执行统计数据</returns>
    /// <response code="200">成功获取统计数据</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取任务执行的总体统计数据，包括：
    /// <list type="bullet">
    ///     <item>总执行次数</item>
    ///     <item>成功次数</item>
    ///     <item>失败次数</item>
    ///     <item>成功率</item>
    ///     <item>平均执行时长</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// GET /api/tasklog/statistics
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponse<TaskStatisticsDto>), 200)]
    public async Task<ApiResponse<TaskStatisticsDto>> GetStatistics()
    {
        try
        {
            var result = await _taskExecutionLogService.GetTodayStatisticsAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取任务执行统计失败");
            return Error<TaskStatisticsDto>("获取任务执行统计失败");
        }
    }
}