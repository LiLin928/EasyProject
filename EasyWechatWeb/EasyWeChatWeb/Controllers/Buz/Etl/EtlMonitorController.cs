using BusinessManager.Etl.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Etl;

/// <summary>
/// ETL执行监控控制器
/// </summary>
[ApiController]
[Route("api/etl/execution")]
[Authorize]
public class EtlExecutionController : BaseController
{
    public IEtlExecutionService _executionService { get; set; } = null!;
    public ILogger<EtlExecutionController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取执行记录分页列表
    /// </summary>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<EtlExecutionDto>>), 200)]
    public async Task<ApiResponse<PageResponse<EtlExecutionDto>>> GetList([FromBody] QueryEtlExecutionDto query)
    {
        try
        {
            var data = await _executionService.GetPageListAsync(query);
            return Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取执行记录列表失败");
            return Error<PageResponse<EtlExecutionDto>>("获取执行记录列表失败");
        }
    }

    /// <summary>
    /// 获取执行记录详情
    /// </summary>
    [HttpGet("detail")]
    [ProducesResponseType(typeof(ApiResponse<EtlExecutionDto>), 200)]
    public async Task<ApiResponse<EtlExecutionDto>> GetDetail([FromQuery] Guid id)
    {
        try
        {
            var data = await _executionService.GetByIdAsync(id);
            if (data == null)
            {
                return Error<EtlExecutionDto>("执行记录不存在");
            }
            return Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取执行记录详情失败");
            return Error<EtlExecutionDto>("获取执行记录详情失败");
        }
    }

    /// <summary>
    /// 获取执行统计信息
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponse<EtlExecutionStatisticsDto>), 200)]
    public async Task<ApiResponse<EtlExecutionStatisticsDto>> GetStatistics([FromQuery] string? dateStart, [FromQuery] string? dateEnd)
    {
        try
        {
            var data = await _executionService.GetStatisticsAsync(dateStart, dateEnd);
            return Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取执行统计失败");
            return Error<EtlExecutionStatisticsDto>("获取执行统计失败");
        }
    }
}

/// <summary>
/// ETL监控控制器（监控相关接口）
/// </summary>
[ApiController]
[Route("api/etl/monitor")]
[Authorize]
public class EtlMonitorController : BaseController
{
    public IEtlExecutionService _executionService { get; set; } = null!;
    public ILogger<EtlMonitorController> _logger { get; set; } = null!;

    /// <summary>
    /// 取消执行
    /// </summary>
    [HttpPost("execution/cancel")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Cancel([FromBody] IdRequestDto dto)
    {
        try
        {
            var result = await _executionService.CancelAsync(dto.Id);
            return Success(result, "取消成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取消执行失败");
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 重试执行
    /// </summary>
    [HttpPost("execution/retry")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Retry([FromBody] IdRequestDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userName = GetCurrentUserName();
            var executionId = await _executionService.RetryAsync(dto.Id, userId, userName);
            return Success(executionId, "重试任务已创建");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重试执行失败");
            return Error<Guid>(ex.Message);
        }
    }

    /// <summary>
    /// 获取执行日志
    /// </summary>
    [HttpGet("execution/logs")]
    [ProducesResponseType(typeof(ApiResponse<ExecutionLogsResponseDto>), 200)]
    public async Task<ApiResponse<ExecutionLogsResponseDto>> GetLogs([FromQuery] Guid id, [FromQuery] string? nodeId, [FromQuery] string? level)
    {
        try
        {
            var logs = await _executionService.GetLogsAsync(id, nodeId, level);
            return Success(new ExecutionLogsResponseDto { Logs = logs });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取执行日志失败");
            return Error<ExecutionLogsResponseDto>("获取执行日志失败");
        }
    }

    /// <summary>
    /// 获取节点执行详情
    /// </summary>
    [HttpGet("execution/node")]
    [ProducesResponseType(typeof(ApiResponse<EtlNodeExecutionDto>), 200)]
    public async Task<ApiResponse<EtlNodeExecutionDto>> GetNodeExecution([FromQuery] Guid id, [FromQuery] string nodeId)
    {
        try
        {
            var data = await _executionService.GetNodeExecutionAsync(id, nodeId);
            if (data == null)
            {
                return Error<EtlNodeExecutionDto>("节点执行记录不存在");
            }
            return Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取节点执行详情失败");
            return Error<EtlNodeExecutionDto>("获取节点执行详情失败");
        }
    }

    /// <summary>
    /// 获取执行的所有节点执行记录
    /// </summary>
    [HttpGet("execution/nodes")]
    [ProducesResponseType(typeof(ApiResponse<List<EtlNodeExecutionDto>>), 200)]
    public async Task<ApiResponse<List<EtlNodeExecutionDto>>> GetNodeExecutions([FromQuery] Guid id)
    {
        try
        {
            var data = await _executionService.GetNodeExecutionsAsync(id);
            return Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取节点执行列表失败");
            return Error<List<EtlNodeExecutionDto>>("获取节点执行列表失败");
        }
    }

    /// <summary>
    /// 获取正在运行的执行列表
    /// </summary>
    [HttpPost("execution/running")]
    [ProducesResponseType(typeof(ApiResponse<List<EtlExecutionDto>>), 200)]
    public async Task<ApiResponse<List<EtlExecutionDto>>> GetRunning()
    {
        try
        {
            var data = await _executionService.GetRunningExecutionsAsync();
            return Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取运行中执行列表失败");
            return Error<List<EtlExecutionDto>>("获取运行中执行列表失败");
        }
    }

    /// <summary>
    /// 获取今日统计
    /// </summary>
    [HttpGet("statistics/today")]
    [ProducesResponseType(typeof(ApiResponse<EtlTodayStatisticsDto>), 200)]
    public async Task<ApiResponse<EtlTodayStatisticsDto>> GetTodayStatistics()
    {
        try
        {
            var data = await _executionService.GetTodayStatisticsAsync();
            return Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取今日统计失败");
            return Error<EtlTodayStatisticsDto>("获取今日统计失败");
        }
    }

    /// <summary>
    /// 获取执行状态（用于实时更新）
    /// </summary>
    [HttpGet("execution/status")]
    [ProducesResponseType(typeof(ApiResponse<EtlExecutionStatusDto>), 200)]
    public async Task<ApiResponse<EtlExecutionStatusDto>> GetStatus([FromQuery] Guid id)
    {
        try
        {
            var data = await _executionService.GetStatusAsync(id);
            if (data == null)
            {
                return Error<EtlExecutionStatusDto>("执行记录不存在");
            }
            return Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取执行状态失败");
            return Error<EtlExecutionStatusDto>("获取执行状态失败");
        }
    }

    /// <summary>
    /// 获取监控仪表盘数据
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(ApiResponse<EtlDashboardDto>), 200)]
    public async Task<ApiResponse<EtlDashboardDto>> GetDashboard()
    {
        try
        {
            var todayStats = await _executionService.GetTodayStatisticsAsync();
            var runningExecutions = await _executionService.GetRunningExecutionsAsync();
            var overallStats = await _executionService.GetStatisticsAsync(null, null);

            var dashboard = new EtlDashboardDto
            {
                TodayTotal = todayStats.Total,
                TodaySuccess = todayStats.Success,
                TodayFailure = todayStats.Failure,
                RunningCount = todayStats.Running,
                PendingCount = todayStats.Pending,
                RunningExecutions = runningExecutions,
                OverallSuccessRate = overallStats.SuccessRate,
                OverallAvgDuration = overallStats.AvgDuration
            };

            return Success(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取监控仪表盘数据失败");
            return Error<EtlDashboardDto>("获取监控仪表盘数据失败");
        }
    }
}

/// <summary>
/// 执行日志响应DTO
/// </summary>
public class ExecutionLogsResponseDto
{
    /// <summary>
    /// 日志列表
    /// </summary>
    public List<EtlExecutionLogDto> Logs { get; set; } = new();
}

/// <summary>
/// ETL监控仪表盘DTO
/// </summary>
public class EtlDashboardDto
{
    /// <summary>
    /// 今日总执行数
    /// </summary>
    public int TodayTotal { get; set; }

    /// <summary>
    /// 今日成功数
    /// </summary>
    public int TodaySuccess { get; set; }

    /// <summary>
    /// 今日失败数
    /// </summary>
    public int TodayFailure { get; set; }

    /// <summary>
    /// 正在运行数
    /// </summary>
    public int RunningCount { get; set; }

    /// <summary>
    /// 待执行数
    /// </summary>
    public int PendingCount { get; set; }

    /// <summary>
    /// 正在运行的执行列表
    /// </summary>
    public List<EtlExecutionDto> RunningExecutions { get; set; } = new();

    /// <summary>
    /// 总体成功率
    /// </summary>
    public double OverallSuccessRate { get; set; }

    /// <summary>
    /// 总体平均耗时
    /// </summary>
    public long OverallAvgDuration { get; set; }
}