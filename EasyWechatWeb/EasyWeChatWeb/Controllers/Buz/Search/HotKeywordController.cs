using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 热门关键词控制器
/// </summary>
/// <remarks>
/// 提供热门关键词管理功能接口
/// </remarks>
[ApiController]
[Route("api/hotkeyword")]
[Authorize]
public class HotKeywordController : BaseController
{
    /// <summary>
    /// 热门关键词服务接口
    /// </summary>
    public IHotKeywordService _hotKeywordService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<HotKeywordController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页列表</returns>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<HotKeywordDto>>), 200)]
    public async Task<ApiResponse<PageResponse<HotKeywordDto>>> GetList([FromBody] QueryHotKeywordDto query)
    {
        try
        {
            var result = await _hotKeywordService.GetPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取热门关键词列表失败");
            return Error<PageResponse<HotKeywordDto>>("获取热门关键词列表失败");
        }
    }

    /// <summary>
    /// 获取详情
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>详情</returns>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<HotKeywordDto>), 200)]
    public async Task<ApiResponse<HotKeywordDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _hotKeywordService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<HotKeywordDto>("关键词不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取热门关键词详情失败: {Id}", id);
            return Error<HotKeywordDto>("获取热门关键词详情失败");
        }
    }

    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的ID</returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Create([FromBody] AddHotKeywordDto dto)
    {
        try
        {
            var result = await _hotKeywordService.CreateAsync(dto);
            return Success(result, "创建成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建热门关键词失败");
            return Error<Guid>("创建热门关键词失败");
        }
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateHotKeywordDto dto)
    {
        try
        {
            var result = await _hotKeywordService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新热门关键词失败: {Id}", dto.Id);
            return Error<int>("更新热门关键词失败");
        }
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>影响的行数</returns>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] Guid id)
    {
        try
        {
            var result = await _hotKeywordService.DeleteAsync(id);
            return Success(result, "删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除热门关键词失败: {Id}", id);
            return Error<int>("删除热门关键词失败");
        }
    }

    /// <summary>
    /// 更新状态
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="status">状态：1-启用，0-禁用</param>
    /// <returns>影响的行数</returns>
    [HttpPut("status/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> UpdateStatus(Guid id, [FromQuery] int status)
    {
        try
        {
            var result = await _hotKeywordService.UpdateStatusAsync(id, status);
            return Success(result, "状态更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新热门关键词状态失败: {Id}", id);
            return Error<int>("更新热门关键词状态失败");
        }
    }

    /// <summary>
    /// 获取推荐关键词列表
    /// </summary>
    /// <param name="limit">数量限制</param>
    /// <returns>关键词列表</returns>
    [HttpGet("recommend")]
    [ProducesResponseType(typeof(ApiResponse<List<HotKeywordDto>>), 200)]
    [AllowAnonymous]
    public async Task<ApiResponse<List<HotKeywordDto>>> GetRecommendList([FromQuery] int limit = 10)
    {
        try
        {
            var result = await _hotKeywordService.GetRecommendListAsync(limit);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取推荐关键词失败");
            return Error<List<HotKeywordDto>>("获取推荐关键词失败");
        }
    }

    /// <summary>
    /// 获取统计
    /// </summary>
    /// <returns>统计信息</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponse<HotKeywordStatisticsDto>), 200)]
    public async Task<ApiResponse<HotKeywordStatisticsDto>> GetStatistics()
    {
        try
        {
            var result = await _hotKeywordService.GetStatisticsAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取热门关键词统计失败");
            return Error<HotKeywordStatisticsDto>("获取热门关键词统计失败");
        }
    }
}