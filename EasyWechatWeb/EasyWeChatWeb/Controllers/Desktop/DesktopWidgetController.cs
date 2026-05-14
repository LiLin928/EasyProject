using BusinessManager.Desktop.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EasyWeChatWeb.Controllers.Desktop;

[ApiController]
[Route("api/desktop/widget")]
[Authorize]
public class DesktopWidgetController : BaseController
{
    public IDesktopWidgetService _widgetService { get; set; } = null!;
    public ILogger<DesktopWidgetController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取组件列表(分页)
    /// </summary>
    [HttpGet("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<DesktopWidgetDto>>), 200)]
    public async Task<ApiResponse<PageResponse<DesktopWidgetDto>>> GetList([FromQuery] QueryDesktopWidgetDto query)
    {
        try
        {
            var (list, total) = await _widgetService.GetListAsync(query);
            var result = PageResponse<DesktopWidgetDto>.Create(list, total, query.PageIndex, query.PageSize);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取组件列表失败");
            return Error<PageResponse<DesktopWidgetDto>>("获取组件列表失败");
        }
    }

    /// <summary>
    /// 获取组件详情
    /// </summary>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<DesktopWidgetDto>), 200)]
    public async Task<ApiResponse<DesktopWidgetDto>> GetById(string id)
    {
        try
        {
            var widgetId = Guid.Parse(id);
            var dto = await _widgetService.GetByIdAsync(widgetId);
            if (dto == null)
                return Error<DesktopWidgetDto>("组件不存在");
            return Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取组件详情失败");
            return Error<DesktopWidgetDto>("获取组件详情失败");
        }
    }

    /// <summary>
    /// 创建组件
    /// </summary>
    [HttpPost("add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Add([FromBody] AddDesktopWidgetDto dto)
    {
        try
        {
            var id = await _widgetService.AddAsync(dto);
            return Success(id, "创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建组件失败");
            return Error<Guid>("创建组件失败");
        }
    }

    /// <summary>
    /// 更新组件
    /// </summary>
    [HttpPost("update")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> Update([FromBody] UpdateDesktopWidgetDto dto)
    {
        try
        {
            var result = await _widgetService.UpdateAsync(dto);
            if (!result)
                return Error<bool>("组件不存在");
            return Success(result, "更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新组件失败");
            return Error<bool>("更新组件失败");
        }
    }

    /// <summary>
    /// 删除组件
    /// </summary>
    [HttpDelete("delete/{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> Delete(string id)
    {
        try
        {
            var widgetId = Guid.Parse(id);
            var result = await _widgetService.DeleteAsync(widgetId);
            if (!result)
                return Error<bool>("组件不存在");
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除组件失败");
            return Error<bool>("删除组件失败");
        }
    }

    /// <summary>
    /// 获取所有启用的组件列表(用于分配)
    /// </summary>
    [HttpGet("enabled-list")]
    [ProducesResponseType(typeof(ApiResponse<List<DesktopWidgetDto>>), 200)]
    public async Task<ApiResponse<List<DesktopWidgetDto>>> GetEnabledList()
    {
        try
        {
            var list = await _widgetService.GetEnabledListAsync();
            return Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取启用组件列表失败");
            return Error<List<DesktopWidgetDto>>("获取启用组件列表失败");
        }
    }
}