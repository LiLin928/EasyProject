using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.WeChatPro;

/// <summary>
/// 微信轮播图控制器
/// </summary>
[ApiController]
[Route("api/wechat/banner")]
public class WeChatBannerController : BaseController
{
    /// <summary>
    /// 轮播图服务
    /// </summary>
    public IBannerService _bannerService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatBannerController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取启用的轮播图列表
    /// </summary>
    /// <returns>轮播图列表</returns>
    [HttpGet("active")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<WxBannerDto>>), 200)]
    public async Task<ApiResponse<List<WxBannerDto>>> GetActiveList()
    {
        try
        {
            var list = await _bannerService.GetActiveListAsync();
            var result = list.Select(b => new WxBannerDto
            {
                Id = b.Id,
                Image = b.Image,
                LinkType = b.LinkType ?? "none",
                LinkValue = b.LinkValue,
                Sort = b.Sort
            }).OrderBy(b => b.Sort).ToList();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取轮播图列表失败");
            return Error<List<WxBannerDto>>("获取轮播图列表失败");
        }
    }
}