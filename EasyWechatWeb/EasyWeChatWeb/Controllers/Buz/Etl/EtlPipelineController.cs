using BusinessManager.Etl.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Etl;

/// <summary>
/// ETL任务流控制器
/// </summary>
[ApiController]
[Route("api/etl/pipeline")]
[Authorize]
public class EtlPipelineController : BaseController
{
    public IEtlPipelineService _pipelineService { get; set; } = null!;
    public ILogger<EtlPipelineController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取任务流分页列表
    /// </summary>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<EtlPipelineDto>>), 200)]
    public async Task<ApiResponse<PageResponse<EtlPipelineDto>>> GetList([FromBody] QueryEtlPipelineDto query)
    {
        try
        {
            var data = await _pipelineService.GetPageListAsync(query);
            return Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取任务流列表失败");
            return Error<PageResponse<EtlPipelineDto>>("获取任务流列表失败");
        }
    }

    /// <summary>
    /// 获取任务流详情
    /// </summary>
    [HttpGet("detail")]
    [ProducesResponseType(typeof(ApiResponse<EtlPipelineDto>), 200)]
    public async Task<ApiResponse<EtlPipelineDto>> GetDetail([FromQuery] Guid id)
    {
        try
        {
            var data = await _pipelineService.GetByIdAsync(id);
            if (data == null)
            {
                return Error<EtlPipelineDto>("任务流不存在");
            }
            return Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取任务流详情失败");
            return Error<EtlPipelineDto>("获取任务流详情失败");
        }
    }

    /// <summary>
    /// 创建任务流
    /// </summary>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Create([FromBody] CreateEtlPipelineDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userName = GetCurrentUserName();
            var id = await _pipelineService.AddAsync(dto, userId, userName);
            return Success(id, "创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建任务流失败");
            return Error<Guid>(ex.Message);
        }
    }

    /// <summary>
    /// 更新任务流
    /// </summary>
    [HttpPost("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateEtlPipelineDto dto)
    {
        try
        {
            var result = await _pipelineService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新任务流失败");
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 删除任务流
    /// </summary>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] DeleteRequestDto dto)
    {
        try
        {
            var result = await _pipelineService.DeleteAsync(dto.Ids);
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除任务流失败");
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 发布任务流
    /// </summary>
    [HttpPost("publish")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Publish([FromBody] IdRequestDto dto)
    {
        try
        {
            var result = await _pipelineService.PublishAsync(dto.Id);
            return Success(result, "发布成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发布任务流失败");
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 取消发布任务流
    /// </summary>
    [HttpPost("unpublish")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Unpublish([FromBody] IdRequestDto dto)
    {
        try
        {
            var result = await _pipelineService.UnpublishAsync(dto.Id);
            return Success(result, "取消发布成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取消发布任务流失败");
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 复制任务流
    /// </summary>
    [HttpPost("copy")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Copy([FromBody] CopyEtlPipelineDto dto)
    {
        try
        {
            var newId = await _pipelineService.CopyAsync(dto.Id, dto.Name);
            return Success(newId, "复制成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "复制任务流失败");
            return Error<Guid>(ex.Message);
        }
    }

    /// <summary>
    /// 执行任务流
    /// </summary>
    [HttpPost("execute")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Execute([FromBody] ExecuteEtlPipelineDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userName = GetCurrentUserName();
            var paramsJson = dto.Params != null ? System.Text.Json.JsonSerializer.Serialize(dto.Params) : null;
            var executionId = await _pipelineService.ExecuteAsync(dto.Id, paramsJson, userId, userName);
            return Success(executionId, "执行任务已创建");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行任务流失败");
            return Error<Guid>(ex.Message);
        }
    }

    /// <summary>
    /// 获取DAG配置
    /// </summary>
    [HttpGet("dag")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<ApiResponse<string>> GetDag([FromQuery] Guid id)
    {
        try
        {
            var dagConfig = await _pipelineService.GetDagConfigAsync(id);
            if (dagConfig == null)
            {
                return Error<string>("任务流不存在");
            }
            return Success<string>(dagConfig!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取DAG配置失败");
            return Error<string>("获取DAG配置失败");
        }
    }

    /// <summary>
    /// 保存DAG配置
    /// </summary>
    [HttpPost("dag/save")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> SaveDag([FromBody] SaveDagConfigDto dto)
    {
        try
        {
            var dagJson = dto.Dag != null ? System.Text.Json.JsonSerializer.Serialize(dto.Dag) : "";
            var result = await _pipelineService.SaveDagConfigAsync(dto.Id, dagJson);
            return Success(result, "保存成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存DAG配置失败");
            return Error<int>(ex.Message);
        }
    }
}

/// <summary>
/// ID请求DTO
/// </summary>
public class IdRequestDto
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }
}

/// <summary>
/// 删除请求DTO
/// </summary>
public class DeleteRequestDto
{
    /// <summary>
    /// ID列表
    /// </summary>
    public List<Guid> Ids { get; set; } = new();
}

/// <summary>
/// 复制任务流DTO
/// </summary>
public class CopyEtlPipelineDto
{
    /// <summary>
    /// 源任务流ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 新任务流名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// 执行任务流DTO
/// </summary>
public class ExecuteEtlPipelineDto
{
    /// <summary>
    /// 任务流ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 执行参数
    /// </summary>
    public object? Params { get; set; }
}

/// <summary>
/// 保存DAG配置DTO
/// </summary>
public class SaveDagConfigDto
{
    /// <summary>
    /// 任务流ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// DAG配置
    /// </summary>
    public object? Dag { get; set; }
}