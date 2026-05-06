using BusinessManager.Etl.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Etl;

/// <summary>
/// ETL调度任务控制器
/// </summary>
[ApiController]
[Route("api/etl/schedule")]
[Authorize]
public class EtlScheduleController : BaseController
{
    public IEtlScheduleService _scheduleService { get; set; } = null!;
    public ILogger<EtlScheduleController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取调度任务分页列表
    /// </summary>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<EtlScheduleDto>>), 200)]
    public async Task<ApiResponse<PageResponse<EtlScheduleDto>>> GetList([FromBody] QueryEtlScheduleDto query)
    {
        try
        {
            var data = await _scheduleService.GetPageListAsync(query);
            return Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取调度任务列表失败");
            return Error<PageResponse<EtlScheduleDto>>("获取调度任务列表失败");
        }
    }

    /// <summary>
    /// 获取调度任务详情
    /// </summary>
    [HttpGet("detail")]
    [ProducesResponseType(typeof(ApiResponse<EtlScheduleDto>), 200)]
    public async Task<ApiResponse<EtlScheduleDto>> GetDetail([FromQuery] Guid id)
    {
        try
        {
            var data = await _scheduleService.GetByIdAsync(id);
            if (data == null)
            {
                return Error<EtlScheduleDto>("调度任务不存在");
            }
            return Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取调度任务详情失败");
            return Error<EtlScheduleDto>("获取调度任务详情失败");
        }
    }

    /// <summary>
    /// 创建调度任务
    /// </summary>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Create([FromBody] CreateEtlScheduleDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userName = GetCurrentUserName();
            var id = await _scheduleService.AddAsync(dto, userId, userName);
            return Success(id, "创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建调度任务失败");
            return Error<Guid>(ex.Message);
        }
    }

    /// <summary>
    /// 更新调度任务
    /// </summary>
    [HttpPost("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateEtlScheduleDto dto)
    {
        try
        {
            var result = await _scheduleService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新调度任务失败");
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 删除调度任务
    /// </summary>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] DeleteRequestDto dto)
    {
        try
        {
            var result = await _scheduleService.DeleteAsync(dto.Ids);
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除调度任务失败");
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 启用调度任务
    /// </summary>
    [HttpPost("enable")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Enable([FromBody] IdRequestDto dto)
    {
        try
        {
            var result = await _scheduleService.EnableAsync(dto.Id);
            return Success(result, "启用成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启用调度任务失败");
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 禁用调度任务
    /// </summary>
    [HttpPost("disable")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Disable([FromBody] IdRequestDto dto)
    {
        try
        {
            var result = await _scheduleService.DisableAsync(dto.Id);
            return Success(result, "禁用成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "禁用调度任务失败");
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 立即执行调度任务
    /// </summary>
    [HttpPost("execute-now")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> ExecuteNow([FromBody] IdRequestDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userName = GetCurrentUserName();
            var executionId = await _scheduleService.ExecuteNowAsync(dto.Id, userId, userName);
            return Success(executionId, "执行任务已创建");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "立即执行调度任务失败");
            return Error<Guid>(ex.Message);
        }
    }

    /// <summary>
    /// 获取调度任务执行历史
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<EtlExecutionDto>>), 200)]
    public async Task<ApiResponse<PageResponse<EtlExecutionDto>>> GetHistory([FromQuery] Guid id, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var data = await _scheduleService.GetExecutionHistoryAsync(id, pageIndex, pageSize);
            return Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取执行历史失败");
            return Error<PageResponse<EtlExecutionDto>>("获取执行历史失败");
        }
    }

    /// <summary>
    /// 获取调度任务统计信息
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponse<EtlScheduleStatisticsDto>), 200)]
    public async Task<ApiResponse<EtlScheduleStatisticsDto>> GetStatistics([FromQuery] Guid id)
    {
        try
        {
            var data = await _scheduleService.GetStatisticsAsync(id);
            if (data == null)
            {
                return Error<EtlScheduleStatisticsDto>("调度任务不存在");
            }
            return Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取统计信息失败");
            return Error<EtlScheduleStatisticsDto>("获取统计信息失败");
        }
    }
}