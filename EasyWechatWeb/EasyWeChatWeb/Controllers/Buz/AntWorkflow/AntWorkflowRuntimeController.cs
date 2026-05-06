using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto.AntWorkflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// Ant流程运行时控制器
/// </summary>
/// <remarks>
/// 提供流程实例管理API接口。
/// 支持流程的启动、查询、撤回等操作。
/// 所有接口需要认证访问，通过JWT Token验证。
/// </remarks>
[ApiController]
[Route("api/ant-workflow/runtime")]
[Authorize]
public class AntWorkflowRuntimeController : BaseController
{
    private readonly IAntWorkflowRuntimeService _runtimeService;
    private readonly ILogger<AntWorkflowRuntimeController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AntWorkflowRuntimeController(
        IAntWorkflowRuntimeService runtimeService,
        ILogger<AntWorkflowRuntimeController> logger)
    {
        _runtimeService = runtimeService;
        _logger = logger;
    }

    #region 流程实例管理

    /// <summary>
    /// 启动流程
    /// </summary>
    /// <param name="dto">启动流程参数</param>
    /// <returns>流程实例ID</returns>
    /// <response code="200">启动成功，返回实例ID</response>
    /// <response code="401">未授权</response>
    /// <response code="400">参数验证失败或流程状态不允许启动</response>
    [HttpPost("start")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Start([FromBody] StartAntWorkflowDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userName = GetCurrentUserName();
            var instanceId = await _runtimeService.StartAsync(dto, userId, userName ?? "系统");
            return Success(instanceId, "启动流程成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启动流程失败，流程ID：{WorkflowId}", dto.WorkflowId);
            return Error<Guid>(ex.Message);
        }
    }

    /// <summary>
    /// 查询我的流程实例
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页实例列表</returns>
    /// <response code="200">返回实例列表</response>
    /// <response code="401">未授权</response>
    [HttpPost("my-instances")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<AntWorkflowInstanceDto>>), 200)]
    public async Task<ApiResponse<PageResponse<AntWorkflowInstanceDto>>> GetMyInstances([FromBody] QueryMyInstanceDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _runtimeService.GetMyInstancesAsync(query, userId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询我的流程实例失败");
            return Error<PageResponse<AntWorkflowInstanceDto>>("查询我的流程实例失败");
        }
    }

    /// <summary>
    /// 查询流程实例详情
    /// </summary>
    /// <param name="id">实例ID</param>
    /// <returns>实例详细信息</returns>
    /// <response code="200">返回实例详情</response>
    /// <response code="401">未授权</response>
    /// <response code="404">实例不存在</response>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<AntWorkflowInstanceDetailDto>), 200)]
    public async Task<ApiResponse<AntWorkflowInstanceDetailDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _runtimeService.GetInstanceDetailAsync(id);
            if (result == null)
            {
                return Error<AntWorkflowInstanceDetailDto>("流程实例不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询流程实例详情失败，实例ID：{InstanceId}", id);
            return Error<AntWorkflowInstanceDetailDto>("查询流程实例详情失败");
        }
    }

    /// <summary>
    /// 撤回流程
    /// </summary>
    /// <param name="id">实例ID</param>
    /// <param name="reason">撤回原因（可选）</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">撤回成功</response>
    /// <response code="401">未授权</response>
    /// <response code="404">实例不存在</response>
    /// <response code="400">流程状态不允许撤回</response>
    [HttpPost("cancel/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Cancel(Guid id, [FromQuery] string? reason)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _runtimeService.CancelAsync(id, userId, reason);
            if (result == 0)
            {
                return Error<int>("流程实例不存在或无权限撤回", 404);
            }
            return Success(result, "撤回流程成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "撤回流程失败，实例ID：{InstanceId}", id);
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 查询审批日志
    /// </summary>
    /// <param name="instanceId">实例ID</param>
    /// <returns>审批日志列表</returns>
    /// <response code="200">返回审批日志</response>
    /// <response code="401">未授权</response>
    [HttpPost("logs/{instanceId}")]
    [ProducesResponseType(typeof(ApiResponse<List<AntExecutionLogDto>>), 200)]
    public async Task<ApiResponse<List<AntExecutionLogDto>>> GetLogs(Guid instanceId)
    {
        try
        {
            var result = await _runtimeService.GetLogsAsync(instanceId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询审批日志失败，实例ID：{InstanceId}", instanceId);
            return Error<List<AntExecutionLogDto>>("查询审批日志失败");
        }
    }

    #endregion
}