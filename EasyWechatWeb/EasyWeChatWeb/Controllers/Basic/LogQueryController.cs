using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessManager.Basic.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto.LogQuery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EasyWeChatWeb.Controllers.Basic;

/// <summary>
/// 日志查询控制器
/// 提供 Elasticsearch 日志的查询、详情查看功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LogQueryController : BaseController
{
    private readonly ILogQueryService _logQueryService;
    private readonly ILogger<LogQueryController> _logger;

    public LogQueryController(
        ILogQueryService logQueryService,
        ILogger<LogQueryController> logger)
    {
        _logQueryService = logQueryService;
        _logger = logger;
    }

    /// <summary>
    /// 查询日志列表
    /// </summary>
    /// <param name="request">查询请求参数</param>
    /// <returns>分页日志列表</returns>
    [HttpPost("query")]
    [ProducesResponseType(typeof(ApiResponse<LogQueryResponseDto>), 200)]
    public async Task<ApiResponse<LogQueryResponseDto>> QueryLogs([FromBody] LogQueryRequestDto request)
    {
        try
        {
            var result = await _logQueryService.QueryLogsAsync(request);
            return Success(result);
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            _logger.LogWarning(ex, "业务异常：{Message}", ex.Message);
            return Error<LogQueryResponseDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询日志失败，环境: {Environment}", request.Environment);
            return Error<LogQueryResponseDto>("查询日志失败: " + ex.Message);
        }
    }

    /// <summary>
    /// 获取日志详情
    /// </summary>
    /// <param name="environment">环境标识</param>
    /// <param name="id">ES 文档 ID</param>
    /// <returns>日志详情</returns>
    [HttpGet("detail/{environment}/{id}")]
    [ProducesResponseType(typeof(ApiResponse<LogDetailDto>), 200)]
    public async Task<ApiResponse<LogDetailDto>> GetLogDetail(string environment, string id)
    {
        try
        {
            var result = await _logQueryService.GetLogDetailAsync(environment, id);
            if (result == null)
            {
                return Error<LogDetailDto>("日志不存在");
            }
            return Success(result);
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            _logger.LogWarning(ex, "业务异常：{Message}", ex.Message);
            return Error<LogDetailDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取日志详情失败，ID: {Id}", id);
            return Error<LogDetailDto>("获取日志详情失败: " + ex.Message);
        }
    }

    /// <summary>
    /// 获取可用环境列表
    /// </summary>
    /// <returns>环境名称列表</returns>
    [HttpPost("environments")]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), 200)]
    public async Task<ApiResponse<List<string>>> GetEnvironments()
    {
        try
        {
            var result = await _logQueryService.GetAvailableEnvironmentsAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取环境列表失败");
            return Error<List<string>>("获取环境列表失败");
        }
    }
}