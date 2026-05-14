using BusinessManager.Desktop.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EasyWeChatWeb.Controllers.Desktop;

[ApiController]
[Route("api/desktop/role-config")]
[Authorize]
public class RoleWidgetConfigController : BaseController
{
    public IRoleWidgetConfigService _roleConfigService { get; set; } = null!;
    public ILogger<RoleWidgetConfigController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取角色的组件配置列表
    /// </summary>
    [HttpGet("list/{roleId}")]
    [ProducesResponseType(typeof(ApiResponse<List<RoleWidgetConfigDto>>), 200)]
    public async Task<ApiResponse<List<RoleWidgetConfigDto>>> GetByRoleId(string roleId)
    {
        try
        {
            var id = Guid.Parse(roleId);
            var list = await _roleConfigService.GetByRoleIdAsync(id);
            return Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取角色组件配置失败");
            return Error<List<RoleWidgetConfigDto>>("获取角色组件配置失败");
        }
    }

    /// <summary>
    /// 保存角色组件配置
    /// </summary>
    [HttpPost("save")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> Save([FromBody] AddRoleWidgetConfigDto dto)
    {
        try
        {
            var result = await _roleConfigService.SaveAsync(dto);
            return Success(result, "保存成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存角色组件配置失败");
            return Error<bool>("保存角色组件配置失败");
        }
    }

    /// <summary>
    /// 获取角色可用的组件列表(用于分配选择)
    /// </summary>
    [HttpGet("available-widgets/{roleId}")]
    [ProducesResponseType(typeof(ApiResponse<List<AvailableWidgetDto>>), 200)]
    public async Task<ApiResponse<List<AvailableWidgetDto>>> GetAvailableWidgets(string roleId)
    {
        try
        {
            var id = Guid.Parse(roleId);
            var list = await _roleConfigService.GetAvailableWidgetsAsync(id);
            return Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取可用组件列表失败");
            return Error<List<AvailableWidgetDto>>("获取可用组件列表失败");
        }
    }
}