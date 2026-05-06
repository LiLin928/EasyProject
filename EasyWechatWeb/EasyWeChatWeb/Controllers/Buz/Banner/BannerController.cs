using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 轮播图控制器
/// </summary>
/// <remarks>
/// 提供轮播图管理、排序管理等接口
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BannerController : BaseController
{
    /// <summary>
    /// 轮播图服务接口
    /// </summary>
    public IBannerService _bannerService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<BannerController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取轮播图列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页轮播图列表</returns>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<BannerDto>>), 200)]
    public async Task<ApiResponse<PageResponse<BannerDto>>> GetList([FromBody] QueryBannerDto query)
    {
        try
        {
            var result = await _bannerService.GetPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取轮播图列表失败");
            return Error<PageResponse<BannerDto>>("获取轮播图列表失败");
        }
    }

    /// <summary>
    /// 获取启用的轮播图列表（用于前端展示）
    /// </summary>
    /// <returns>轮播图列表</returns>
    [HttpPost("active")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<BannerDto>>), 200)]
    public async Task<ApiResponse<List<BannerDto>>> GetActiveList()
    {
        try
        {
            var result = await _bannerService.GetActiveListAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取启用轮播图列表失败");
            return Error<List<BannerDto>>("获取启用轮播图列表失败");
        }
    }

    /// <summary>
    /// 获取轮播图详情
    /// </summary>
    /// <param name="id">轮播图ID</param>
    /// <returns>轮播图详情</returns>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<BannerDto>), 200)]
    public async Task<ApiResponse<BannerDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _bannerService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<BannerDto>("轮播图不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取轮播图详情失败: {Id}", id);
            return Error<BannerDto>("获取轮播图详情失败");
        }
    }

    /// <summary>
    /// 添加轮播图
    /// </summary>
    /// <param name="dto">添加参数</param>
    /// <returns>新创建的轮播图ID</returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Add([FromBody] AddBannerDto dto)
    {
        try
        {
            var id = await _bannerService.AddAsync(dto);
            return Success(id, "添加成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加轮播图失败");
            return Error<Guid>("添加轮播图失败");
        }
    }

    /// <summary>
    /// 更新轮播图
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateBannerDto dto)
    {
        try
        {
            var result = await _bannerService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新轮播图失败: {Id}", dto.Id);
            return Error<int>("更新轮播图失败");
        }
    }

    /// <summary>
    /// 删除轮播图
    /// </summary>
    /// <param name="id">轮播图ID</param>
    /// <returns>影响的行数</returns>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] Guid id)
    {
        try
        {
            var result = await _bannerService.DeleteAsync(id);
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除轮播图失败: {Id}", id);
            return Error<int>("删除轮播图失败");
        }
    }

    /// <summary>
    /// 批量删除轮播图
    /// </summary>
    /// <param name="ids">轮播图ID列表</param>
    /// <returns>影响的行数</returns>
    [HttpPost("delete-batch")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> DeleteBatch([FromBody] List<Guid> ids)
    {
        try
        {
            var count = 0;
            foreach (var id in ids)
            {
                count += await _bannerService.DeleteAsync(id);
            }
            return Success(count, "批量删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量删除轮播图失败");
            return Error<int>("批量删除轮播图失败");
        }
    }

    /// <summary>
    /// 更新轮播图状态
    /// </summary>
    /// <param name="id">轮播图ID</param>
    /// <param name="status">状态值</param>
    /// <returns>影响的行数</returns>
    [HttpPut("status/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> UpdateStatus(Guid id, [FromQuery] int status)
    {
        try
        {
            var result = await _bannerService.UpdateStatusAsync(id, status);
            return Success(result, "状态更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新轮播图状态失败: {Id}", id);
            return Error<int>("更新轮播图状态失败");
        }
    }

    /// <summary>
    /// 批量更新排序
    /// </summary>
    /// <param name="dto">排序参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("sort")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> BatchSort([FromBody] SortBannerDto dto)
    {
        try
        {
            var result = await _bannerService.BatchSortAsync(dto);
            return Success(result, "排序更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量更新排序失败");
            return Error<int>("批量更新排序失败");
        }
    }
}