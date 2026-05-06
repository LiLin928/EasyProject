using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto.AntWorkflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// Ant流程管理控制器
/// </summary>
/// <remarks>
/// 提供流程管理、版本管理等API接口。
/// 支持流程的完整生命周期管理：创建 -> 编辑 -> 发布 -> 停用/启用。
/// 所有接口需要认证访问，通过JWT Token验证。
/// </remarks>
[ApiController]
[Route("api/ant-workflow/workflow")]
[Authorize]
public class AntWorkflowController : BaseController
{
    private readonly IAntWorkflowService _workflowService;
    private readonly IAntWorkflowVersionService _versionService;
    private readonly ILogger<AntWorkflowController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AntWorkflowController(
        IAntWorkflowService workflowService,
        IAntWorkflowVersionService versionService,
        ILogger<AntWorkflowController> logger)
    {
        _workflowService = workflowService;
        _versionService = versionService;
        _logger = logger;
    }

    #region 流程管理 - 查询

    /// <summary>
    /// 获取流程列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页流程列表</returns>
    /// <response code="200">返回流程列表</response>
    /// <response code="401">未授权，需要先登录</response>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<AntWorkflowDto>>), 200)]
    public async Task<ApiResponse<PageResponse<AntWorkflowDto>>> GetList([FromBody] QueryAntWorkflowDto query)
    {
        try
        {
            var result = await _workflowService.GetPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取流程列表失败");
            return Error<PageResponse<AntWorkflowDto>>("获取流程列表失败");
        }
    }

    /// <summary>
    /// 获取流程详情
    /// </summary>
    /// <param name="id">流程ID</param>
    /// <returns>流程详细信息</returns>
    /// <response code="200">返回流程详情</response>
    /// <response code="401">未授权</response>
    /// <response code="404">流程不存在</response>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<AntWorkflowDto>), 200)]
    public async Task<ApiResponse<AntWorkflowDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _workflowService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<AntWorkflowDto>("流程不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取流程详情失败，ID：{Id}", id);
            return Error<AntWorkflowDto>("获取流程详情失败");
        }
    }

    /// <summary>
    /// 获取已发布的流程列表
    /// </summary>
    /// <param name="categoryCode">分类编码（可选）</param>
    /// <returns>已发布流程列表</returns>
    /// <response code="200">返回流程列表</response>
    /// <response code="401">未授权</response>
    [HttpGet("published")]
    [ProducesResponseType(typeof(ApiResponse<List<AntWorkflowDto>>), 200)]
    public async Task<ApiResponse<List<AntWorkflowDto>>> GetPublishedList([FromQuery] string? categoryCode)
    {
        try
        {
            var result = await _workflowService.GetPublishedListAsync(categoryCode);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取已发布流程列表失败");
            return Error<List<AntWorkflowDto>>("获取已发布流程列表失败");
        }
    }

    #endregion

    #region 流程管理 - 操作

    /// <summary>
    /// 创建流程
    /// </summary>
    /// <param name="dto">创建流程参数</param>
    /// <returns>新创建的流程ID</returns>
    /// <response code="200">创建成功，返回流程ID</response>
    /// <response code="401">未授权</response>
    /// <response code="400">参数验证失败</response>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Create([FromBody] CreateAntWorkflowDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var id = await _workflowService.CreateAsync(dto, userId);
            return Success(id, "创建流程成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建流程失败");
            return Error<Guid>(ex.Message);
        }
    }

    /// <summary>
    /// 更新流程
    /// </summary>
    /// <param name="dto">更新流程参数</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">更新成功</response>
    /// <response code="401">未授权</response>
    /// <response code="404">流程不存在</response>
    /// <response code="400">流程状态不允许编辑</response>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateAntWorkflowDto dto)
    {
        try
        {
            var result = await _workflowService.UpdateAsync(dto);
            if (result == 0)
            {
                return Error<int>("流程不存在或状态不允许编辑", 404);
            }
            return Success(result, "更新流程成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新流程失败，ID：{Id}", dto.Id);
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 删除流程
    /// </summary>
    /// <param name="id">流程ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权</response>
    /// <response code="404">流程不存在</response>
    /// <response code="400">流程状态不允许删除</response>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] Guid id)
    {
        try
        {
            var result = await _workflowService.DeleteAsync(id);
            if (result == 0)
            {
                return Error<int>("流程不存在或状态不允许删除", 404);
            }
            return Success(result, "删除流程成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除流程失败，ID：{Id}", id);
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 发布流程
    /// </summary>
    /// <param name="id">流程ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">发布成功</response>
    /// <response code="401">未授权</response>
    /// <response code="404">流程不存在</response>
    /// <response code="400">流程状态不允许发布</response>
    [HttpPost("publish/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Publish(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userName = GetCurrentUserName();
            var result = await _workflowService.PublishAsync(id, userId, userName ?? "系统");
            if (result == 0)
            {
                return Error<int>("流程不存在或状态不允许发布", 404);
            }
            return Success(result, "发布流程成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发布流程失败，ID：{Id}", id);
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 停用流程
    /// </summary>
    /// <param name="id">流程ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">停用成功</response>
    /// <response code="401">未授权</response>
    /// <response code="404">流程不存在</response>
    /// <response code="400">流程状态不允许停用</response>
    [HttpPut("disable/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Disable(Guid id)
    {
        try
        {
            var result = await _workflowService.DisableAsync(id);
            if (result == 0)
            {
                return Error<int>("流程不存在或状态不允许停用", 404);
            }
            return Success(result, "停用流程成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "停用流程失败，ID：{Id}", id);
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 启用流程
    /// </summary>
    /// <param name="id">流程ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">启用成功</response>
    /// <response code="401">未授权</response>
    /// <response code="404">流程不存在</response>
    /// <response code="400">流程状态不允许启用</response>
    [HttpPut("enable/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Enable(Guid id)
    {
        try
        {
            var result = await _workflowService.EnableAsync(id);
            if (result == 0)
            {
                return Error<int>("流程不存在或状态不允许启用", 404);
            }
            return Success(result, "启用流程成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启用流程失败，ID：{Id}", id);
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 复制流程
    /// </summary>
    /// <param name="dto">复制参数</param>
    /// <returns>新流程ID</returns>
    /// <response code="200">复制成功，返回新流程ID</response>
    /// <response code="401">未授权</response>
    /// <response code="404">源流程不存在</response>
    /// <response code="400">参数验证失败或编码重复</response>
    [HttpPost("copy")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Copy([FromBody] CopyAntWorkflowDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var id = await _workflowService.CopyAsync(dto, userId);
            return Success(id, "复制流程成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "复制流程失败，源ID：{SourceId}", dto.SourceId);
            return Error<Guid>(ex.Message);
        }
    }

    #endregion

    #region 版本管理

    /// <summary>
    /// 获取流程版本列表
    /// </summary>
    /// <param name="workflowId">流程ID</param>
    /// <returns>版本列表</returns>
    /// <response code="200">返回版本列表</response>
    /// <response code="401">未授权</response>
    [HttpPost("versions/{workflowId}")]
    [ProducesResponseType(typeof(ApiResponse<List<AntWorkflowVersionDto>>), 200)]
    public async Task<ApiResponse<List<AntWorkflowVersionDto>>> GetVersions(Guid workflowId)
    {
        try
        {
            var result = await _versionService.GetListByWorkflowIdAsync(workflowId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取流程版本列表失败，流程ID：{WorkflowId}", workflowId);
            return Error<List<AntWorkflowVersionDto>>("获取流程版本列表失败");
        }
    }

    /// <summary>
    /// 获取版本详情
    /// </summary>
    /// <param name="id">版本ID</param>
    /// <returns>版本详细信息</returns>
    /// <response code="200">返回版本详情</response>
    /// <response code="401">未授权</response>
    /// <response code="404">版本不存在</response>
    [HttpGet("version/{id}")]
    [ProducesResponseType(typeof(ApiResponse<AntWorkflowVersionDto>), 200)]
    public async Task<ApiResponse<AntWorkflowVersionDto>> GetVersionDetail(Guid id)
    {
        try
        {
            var result = await _versionService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<AntWorkflowVersionDto>("版本不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取版本详情失败，版本ID：{Id}", id);
            return Error<AntWorkflowVersionDto>("获取版本详情失败");
        }
    }

    #endregion
}