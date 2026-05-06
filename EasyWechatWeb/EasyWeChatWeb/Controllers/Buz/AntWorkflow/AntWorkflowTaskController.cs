using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto.AntWorkflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// Ant流程任务控制器
/// </summary>
/// <remarks>
/// 提供任务管理API接口。
/// 支持待办、已办、抄送的查询和处理。
/// 所有接口需要认证访问，通过JWT Token验证。
/// </remarks>
[ApiController]
[Route("api/ant-workflow/task")]
[Authorize]
public class AntWorkflowTaskController : BaseController
{
    private readonly IAntWorkflowTaskService _taskService;
    private readonly ILogger<AntWorkflowTaskController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AntWorkflowTaskController(
        IAntWorkflowTaskService taskService,
        ILogger<AntWorkflowTaskController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    #region 任务查询

    /// <summary>
    /// 查询待办任务
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页待办任务列表</returns>
    /// <response code="200">返回待办列表</response>
    /// <response code="401">未授权</response>
    [HttpPost("todo")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<AntWorkflowTaskDto>>), 200)]
    public async Task<ApiResponse<PageResponse<AntWorkflowTaskDto>>> GetTodoList([FromBody] QueryTodoTaskDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            var isAdmin = IsAdmin(); // 管理员可查看所有待办任务
            var result = await _taskService.GetTodoTasksAsync(query, userId, isAdmin);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询待办任务失败");
            return Error<PageResponse<AntWorkflowTaskDto>>("查询待办任务失败");
        }
    }

    /// <summary>
    /// 查询已办任务
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页已办任务列表</returns>
    /// <response code="200">返回已办列表</response>
    /// <response code="401">未授权</response>
    [HttpPost("done")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<AntWorkflowTaskDto>>), 200)]
    public async Task<ApiResponse<PageResponse<AntWorkflowTaskDto>>> GetDoneList([FromBody] QueryDoneTaskDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            var isAdmin = IsAdmin(); // 管理员可查看所有已办任务
            var result = await _taskService.GetDoneTasksAsync(query, userId, isAdmin);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询已办任务失败");
            return Error<PageResponse<AntWorkflowTaskDto>>("查询已办任务失败");
        }
    }

    /// <summary>
    /// 查询抄送列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页抄送列表</returns>
    /// <response code="200">返回抄送列表</response>
    /// <response code="401">未授权</response>
    [HttpPost("cc")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<AntWorkflowCcDto>>), 200)]
    public async Task<ApiResponse<PageResponse<AntWorkflowCcDto>>> GetCcList([FromBody] QueryCcTaskDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            var isAdmin = IsAdmin(); // 管理员可查看所有抄送
            var result = await _taskService.GetCcTasksAsync(query, userId, isAdmin);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询抄送列表失败");
            return Error<PageResponse<AntWorkflowCcDto>>("查询抄送列表失败");
        }
    }

    /// <summary>
    /// 查询任务详情
    /// </summary>
    /// <param name="id">任务ID</param>
    /// <returns>任务详细信息</returns>
    /// <response code="200">返回任务详情</response>
    /// <response code="401">未授权</response>
    /// <response code="404">任务不存在</response>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<AntWorkflowTaskDto>), 200)]
    public async Task<ApiResponse<AntWorkflowTaskDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _taskService.GetTaskDetailAsync(id);
            if (result == null)
            {
                return Error<AntWorkflowTaskDto>("任务不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询任务详情失败，任务ID：{TaskId}", id);
            return Error<AntWorkflowTaskDto>("查询任务详情失败");
        }
    }

    #endregion

    #region 任务操作

    /// <summary>
    /// 审批通过
    /// </summary>
    /// <param name="dto">审批参数</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">审批成功</response>
    /// <response code="401">未授权</response>
    /// <response code="404">任务不存在</response>
    /// <response code="400">任务状态不允许审批</response>
    [HttpPost("approve")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Approve([FromBody] ApproveAntTaskDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userName = GetCurrentUserName();
            var result = await _taskService.ApproveAsync(dto, userId, userName ?? "系统");
            if (result == 0)
            {
                return Error<int>("任务不存在或无权限处理", 404);
            }
            return Success(result, "审批通过成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "审批通过失败，任务ID：{TaskId}", dto.TaskId);
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 驳回任务
    /// </summary>
    /// <param name="dto">驳回参数</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">驳回成功</response>
    /// <response code="401">未授权</response>
    /// <response code="404">任务不存在</response>
    /// <response code="400">任务状态不允许驳回</response>
    [HttpPost("reject")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Reject([FromBody] RejectAntTaskDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userName = GetCurrentUserName();
            var result = await _taskService.RejectAsync(dto, userId, userName ?? "系统");
            if (result == 0)
            {
                return Error<int>("任务不存在或无权限处理", 404);
            }
            return Success(result, "驳回成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "驳回任务失败，任务ID：{TaskId}", dto.TaskId);
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 转交任务
    /// </summary>
    /// <param name="dto">转交参数</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">转交成功</response>
    /// <response code="401">未授权</response>
    /// <response code="404">任务不存在</response>
    /// <response code="400">任务状态不允许转交</response>
    [HttpPost("transfer")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Transfer([FromBody] TransferAntTaskDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userName = GetCurrentUserName();
            var result = await _taskService.TransferAsync(dto, userId, userName ?? "系统");
            if (result == 0)
            {
                return Error<int>("任务不存在或无权限处理", 404);
            }
            return Success(result, "转交成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "转交任务失败，任务ID：{TaskId}", dto.TaskId);
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 加签
    /// </summary>
    /// <param name="dto">加签参数</param>
    /// <returns>新增任务数量</returns>
    /// <response code="200">加签成功</response>
    /// <response code="401">未授权</response>
    /// <response code="404">任务不存在</response>
    /// <response code="400">流程状态不允许加签</response>
    [HttpPost("add-signer")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> AddSigner([FromBody] AddSignerAntTaskDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userName = GetCurrentUserName();
            var result = await _taskService.AddSignerAsync(dto, userId, userName ?? "系统");
            if (result == 0)
            {
                return Error<int>("任务不存在或无权限处理", 404);
            }
            return Success(result, "加签成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加签失败，任务ID：{TaskId}", dto.TaskId);
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 标记抄送已读
    /// </summary>
    /// <param name="ids">抄送记录ID列表</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">标记成功</response>
    /// <response code="401">未授权</response>
    [HttpPost("cc-read")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> MarkCcRead([FromBody] List<Guid> ids)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.MarkCcReadAsync(ids, userId);
            return Success(result, "标记已读成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "标记抄送已读失败");
            return Error<int>("标记抄送已读失败");
        }
    }

    #endregion
}