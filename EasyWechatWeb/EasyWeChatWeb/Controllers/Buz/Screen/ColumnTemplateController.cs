using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 列配置模板控制器
/// </summary>
[ApiController]
[Route("api/column-template")]
[Authorize]
public class ColumnTemplateController : BaseController
{
    public IColumnTemplateService _columnTemplateService { get; set; } = null!;
    public ILogger<ColumnTemplateController> _logger { get; set; } = null!;

    [HttpPost("list")]
    public async Task<ApiResponse<PageResponse<ColumnTemplateDto>>> GetList([FromBody] QueryColumnTemplateDto query)
    {
        try
        {
            var result = await _columnTemplateService.GetPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取列模板列表失败");
            return Error<PageResponse<ColumnTemplateDto>>("获取列模板列表失败");
        }
    }

    [HttpGet("detail/{id}")]
    public async Task<ApiResponse<ColumnTemplateDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _columnTemplateService.GetByIdAsync(id);
            if (result == null) return Error<ColumnTemplateDto>("模板不存在", 404);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取列模板详情失败");
            return Error<ColumnTemplateDto>("获取列模板详情失败");
        }
    }

    [HttpPost("single")]
    public async Task<ApiResponse<List<ColumnTemplateDto>>> GetSingleTemplates()
    {
        try
        {
            var result = await _columnTemplateService.GetSingleTemplatesAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取单列模板失败");
            return Error<List<ColumnTemplateDto>>("获取单列模板失败");
        }
    }

    [HttpPost("add")]
    public async Task<ApiResponse<Guid>> Add([FromBody] CreateColumnTemplateDto dto)
    {
        try
        {
            var creatorId = GetCurrentUserId();
            var id = await _columnTemplateService.AddAsync(dto, creatorId);
            return Success(id, "添加成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加列模板失败");
            return Error<Guid>("添加列模板失败");
        }
    }

    [HttpPut("update")]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateColumnTemplateDto dto)
    {
        try
        {
            var result = await _columnTemplateService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新列模板失败");
            return Error<int>("更新列模板失败");
        }
    }

    [HttpPost("delete")]
    public async Task<ApiResponse<int>> Delete([FromBody] Guid id)
    {
        try
        {
            var result = await _columnTemplateService.DeleteAsync(id);
            return Success(result, "删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除列模板失败");
            return Error<int>("删除列模板失败");
        }
    }

    [HttpPost("delete-batch")]
    public async Task<ApiResponse<int>> DeleteBatch([FromBody] List<Guid> ids)
    {
        try
        {
            var count = 0;
            foreach (var id in ids)
            {
                count += await _columnTemplateService.DeleteAsync(id);
            }
            return Success(count, "批量删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量删除列模板失败");
            return Error<int>("批量删除列模板失败");
        }
    }

    [HttpPost("fetch-columns")]
    public async Task<ApiResponse<List<DetectedColumnDto>>> FetchColumns([FromBody] FetchColumnsDto dto)
    {
        try
        {
            var result = await _columnTemplateService.FetchColumnsAsync(dto);
            return Success(result);
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<List<DetectedColumnDto>>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取列信息失败");
            return Error<List<DetectedColumnDto>>("获取列信息失败");
        }
    }
}