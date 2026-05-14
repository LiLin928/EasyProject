using BusinessManager.Desktop.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EasyWeChatWeb.Controllers.Desktop;

[ApiController]
[Route("api/desktop/user-config")]
[Authorize]
public class UserWidgetConfigController : BaseController
{
    public IUserWidgetConfigService _userConfigService { get; set; } = null!;
    public ILogger<UserWidgetConfigController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取当前用户的桌面数据
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<UserDesktopDto>), 200)]
    public async Task<ApiResponse<UserDesktopDto>> GetMyDesktop()
    {
        try
        {
            var userId = GetCurrentUserId();
            var dto = await _userConfigService.GetUserDesktopAsync(userId);
            return Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户桌面数据失败");
            return Error<UserDesktopDto>("获取用户桌面数据失败");
        }
    }

    /// <summary>
    /// 保存用户桌面配置
    /// </summary>
    [HttpPost("save")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> Save([FromBody] SaveUserWidgetConfigDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _userConfigService.SaveAsync(userId, dto);
            return Success(result, "保存成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存用户桌面配置失败");
            return Error<bool>("保存用户桌面配置失败");
        }
    }

    /// <summary>
    /// 重置用户桌面为角色默认布局
    /// </summary>
    [HttpPost("reset")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> Reset()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _userConfigService.ResetAsync(userId);
            return Success(result, "重置成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重置用户桌面失败");
            return Error<bool>("重置用户桌面失败");
        }
    }
}