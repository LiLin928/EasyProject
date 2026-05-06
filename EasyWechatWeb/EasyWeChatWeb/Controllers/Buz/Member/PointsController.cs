using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 积分控制器
/// </summary>
/// <remarks>
/// 提供积分记录查询功能接口
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PointsController : BaseController
{
    /// <summary>
    /// 积分服务接口
    /// </summary>
    public IPointsService _pointsService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<PointsController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取积分记录列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页积分记录列表</returns>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<PointsRecordDto>>), 200)]
    public async Task<ApiResponse<PageResponse<PointsRecordDto>>> GetList([FromBody] QueryPointsRecordDto query)
    {
        try
        {
            var result = await _pointsService.GetPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取积分记录列表失败");
            return Error<PageResponse<PointsRecordDto>>("获取积分记录列表失败");
        }
    }
}