using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 报表控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportController : BaseController
{
    public IReportService _reportService { get; set; } = null!;
    public ILogger<ReportController> _logger { get; set; } = null!;

    [HttpPost("list")]
    public async Task<ApiResponse<PageResponse<ReportDto>>> GetList([FromBody] QueryReportDto query)
    {
        try
        {
            var result = await _reportService.GetPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取报表列表失败");
            return Error<PageResponse<ReportDto>>("获取报表列表失败");
        }
    }

    [HttpGet("detail/{id}")]
    public async Task<ApiResponse<ReportDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _reportService.GetByIdAsync(id);
            if (result == null) return Error<ReportDto>("报表不存在", 404);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取报表详情失败");
            return Error<ReportDto>("获取报表详情失败");
        }
    }

    [HttpPost("add")]
    public async Task<ApiResponse<Guid>> Add([FromBody] CreateReportDto dto)
    {
        try
        {
            var creatorId = GetCurrentUserId();
            var id = await _reportService.AddAsync(dto, creatorId);
            return Success(id, "添加成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加报表失败");
            return Error<Guid>("添加报表失败");
        }
    }

    [HttpPut("update")]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateReportDto dto)
    {
        try
        {
            var result = await _reportService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新报表失败");
            return Error<int>("更新报表失败");
        }
    }

    [HttpPost("delete")]
    public async Task<ApiResponse<int>> Delete([FromBody] Guid id)
    {
        try
        {
            var result = await _reportService.DeleteAsync(id);
            return Success(result, "删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除报表失败");
            return Error<int>("删除报表失败");
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
                count += await _reportService.DeleteAsync(id);
            }
            return Success(count, "批量删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量删除报表失败");
            return Error<int>("批量删除报表失败");
        }
    }

    [HttpPost("preview")]
    public async Task<ApiResponse<PreviewResultDto>> Preview([FromBody] PreviewReportDto dto)
    {
        try
        {
            var result = await _reportService.PreviewAsync(dto);
            return Success(result);
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<PreviewResultDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "预览报表失败");
            return Error<PreviewResultDto>("预览报表失败");
        }
    }

    [HttpGet("execute/{id}")]
    public async Task<ApiResponse<PreviewResultDto>> Execute(Guid id)
    {
        try
        {
            var result = await _reportService.ExecuteAsync(id);
            return Success(result);
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<PreviewResultDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行报表失败");
            return Error<PreviewResultDto>("执行报表失败");
        }
    }

    [HttpGet("categories")]
    public async Task<ApiResponse<List<ReportCategoryDto>>> GetCategories()
    {
        try
        {
            var result = await _reportService.GetCategoriesAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取报表分类失败");
            return Error<List<ReportCategoryDto>>("获取报表分类失败");
        }
    }

    /// <summary>
    /// 获取发布报表数据（需要登录）
    /// </summary>
    [HttpGet("publish/{id}")]
    public async Task<ApiResponse<PublishReportDto>> GetPublishData(Guid id)
    {
        try
        {
            var result = await _reportService.GetPublishDataAsync(id);
            if (result == null) return Error<PublishReportDto>("报表不存在", 404);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取发布报表数据失败");
            return Error<PublishReportDto>("获取发布报表数据失败");
        }
    }
}