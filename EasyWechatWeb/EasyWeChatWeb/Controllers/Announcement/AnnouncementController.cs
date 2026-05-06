using BusinessManager.Announcement.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Announcement;

/// <summary>
/// 系统公告控制器
/// </summary>
/// <remarks>
/// 提供系统公告的管理功能，包括公告的增删改查、发布撤回、置顶管理等。
/// 支持全员公告和定向公告，提供公告阅读状态跟踪和统计功能。
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnnouncementController : BaseController
{
    /// <summary>
    /// 公告服务接口
    /// </summary>
    public IAnnouncementService _announcementService { get; set; } = null!;
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<AnnouncementController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取公告列表（分页）
    /// </summary>
    /// <param name="query">查询参数，包含分页、标题关键字、类型、级别、状态等筛选条件</param>
    /// <returns>分页后的公告列表</returns>
    /// <response code="200">成功获取公告列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 支持按标题关键字模糊搜索，按类型、级别、状态、是否置顶等条件筛选。
    /// 支持按发布时间范围筛选。
    /// 返回结果按置顶状态和创建时间排序，置顶公告优先显示。
    /// </remarks>
    /// <example>
    /// POST /api/announcement/list
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "pageIndex": 1,
    ///     "pageSize": 10,
    ///     "title": "升级",
    ///     "type": 1,
    ///     "level": 2,
    ///     "status": 1
    /// }
    /// </example>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<AnnouncementDto>>), 200)]
    public async Task<ApiResponse<PageResponse<AnnouncementDto>>> GetList([FromBody] QueryAnnouncementDto query)
    {
        try
        {
            var result = await _announcementService.GetPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取公告列表失败");
            return Error<PageResponse<AnnouncementDto>>("获取公告列表失败");
        }
    }

    /// <summary>
    /// 获取公告详情
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>公告详细信息</returns>
    /// <response code="200">成功获取公告详情</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">公告不存在</response>
    /// <remarks>
    /// 获取公告的完整信息，包括富文本内容和目标角色信息。
    /// 同时返回阅读人数和总目标人数等统计数据。
    /// 会返回当前用户的阅读状态和附件列表。
    /// </remarks>
    /// <example>
    /// GET /api/announcement/detail/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementDto>), 200)]
    public async Task<ApiResponse<AnnouncementDto>> GetDetail(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _announcementService.GetByIdAsync(id, userId);
            if (result == null)
            {
                return Error<AnnouncementDto>("公告不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取公告详情失败: {Id}", id);
            return Error<AnnouncementDto>("获取公告详情失败");
        }
    }

    /// <summary>
    /// 新增公告（草稿）
    /// </summary>
    /// <param name="dto">新增公告请求参数，包含标题、内容、类型、级别等信息</param>
    /// <returns>新创建的公告ID</returns>
    /// <response code="200">添加成功，返回新公告ID</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">参数验证失败</response>
    /// <remarks>
    /// 创建新的公告，默认状态为草稿。
    /// 定向公告需要指定目标角色ID列表。
    /// 可选择是否置顶，置顶公告会在列表中优先显示。
    /// </remarks>
    /// <example>
    /// POST /api/announcement/add
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "title": "系统升级通知",
    ///     "content": "系统将于今晚22:00进行升级维护...",
    ///     "type": 1,
    ///     "level": 2,
    ///     "isTop": 0
    /// }
    /// </example>
    [HttpPost("add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Add([FromBody] AddAnnouncementDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var id = await _announcementService.AddAsync(dto, userId);
            return Success(id, "公告创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增公告失败");
            return Error<Guid>("新增公告失败");
        }
    }

    /// <summary>
    /// 更新公告
    /// </summary>
    /// <param name="dto">更新公告请求参数，包含公告ID和需要更新的字段</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">更新成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">参数验证失败或公告状态不允许更新</response>
    /// <remarks>
    /// 更新公告的标题、内容、类型、级别等信息。
    /// 只能更新草稿状态的公告，已发布的公告不可更新。
    /// 更新时会记录更新时间。
    /// </remarks>
    /// <example>
    /// POST /api/announcement/update
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///     "title": "更新后的标题",
    ///     "content": "更新后的内容...",
    ///     "type": 1,
    ///     "level": 2
    /// }
    /// </example>
    [HttpPost("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateAnnouncementDto dto)
    {
        try
        {
            var result = await _announcementService.UpdateAsync(dto);
            return Success(result, "公告更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新公告失败: {Id}", dto.Id);
            return Error<int>("更新公告失败");
        }
    }

    /// <summary>
    /// 发布公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">发布成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">公告状态不允许发布</response>
    /// <remarks>
    /// 将草稿状态的公告发布，发布后用户可以查看。
    /// 发布时会记录发布时间，同时创建阅读记录。
    /// 全员公告为所有用户创建阅读记录，定向公告为指定角色用户创建阅读记录。
    /// 不能发布已撤回的公告，需要重新编辑后再发布。
    /// </remarks>
    /// <example>
    /// POST /api/announcement/publish/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("publish/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Publish(Guid id)
    {
        try
        {
            var result = await _announcementService.PublishAsync(id);
            return Success(result, "公告发布成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发布公告失败: {Id}", id);
            return Error<int>("发布公告失败");
        }
    }

    /// <summary>
    /// 撤回公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">撤回成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">公告状态不允许撤回</response>
    /// <remarks>
    /// 将已发布的公告撤回，撤回后用户无法查看。
    /// 撤回时会记录撤回时间，公告状态变为已撤回。
    /// 只能撤回已发布的公告，草稿状态的公告不需要撤回。
    /// </remarks>
    /// <example>
    /// POST /api/announcement/recall/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("recall/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Recall(Guid id)
    {
        try
        {
            var result = await _announcementService.RecallAsync(id);
            return Success(result, "公告撤回成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "撤回公告失败: {Id}", id);
            return Error<int>("撤回公告失败");
        }
    }

    /// <summary>
    /// 删除公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">公告状态不允许删除</response>
    /// <remarks>
    /// 删除公告及其阅读记录。
    /// 只能删除草稿或已撤回状态的公告，已发布的公告不可删除。
    /// 删除操作不可恢复，需谨慎使用。
    /// </remarks>
    /// <example>
    /// DELETE /api/announcement/delete/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpDelete("delete/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete(Guid id)
    {
        try
        {
            var result = await _announcementService.DeleteAsync(id);
            return Success(result, "公告删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除公告失败: {Id}", id);
            return Error<int>("删除公告失败");
        }
    }

    /// <summary>
    /// 置顶/取消置顶公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">操作成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">公告状态不允许置顶</response>
    /// <remarks>
    /// 切换公告的置顶状态。
    /// 置顶公告会在列表中优先显示，取消置顶后按正常排序显示。
    /// 置顶时会记录置顶时间，取消置顶时清除置顶时间。
    /// 只有已发布的公告可以置顶。
    /// </remarks>
    /// <example>
    /// POST /api/announcement/top/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("top/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> ToggleTop(Guid id)
    {
        try
        {
            // 先获取公告详情判断当前置顶状态
            var announcement = await _announcementService.GetByIdAsync(id);
            if (announcement == null)
            {
                return Error<int>("公告不存在", 404);
            }

            // 切换置顶状态：当前是置顶则取消，否则置顶
            var newIsTop = announcement.IsTop == 1 ? 0 : 1;
            var result = await _announcementService.ToggleTopAsync(id, newIsTop);
            return Success(result, newIsTop == 1 ? "公告置顶成功" : "公告取消置顶成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "切换公告置顶状态失败: {Id}", id);
            return Error<int>("切换置顶状态失败");
        }
    }

    /// <summary>
    /// 标记公告为已读
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">标记成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 标记当前用户对指定公告的阅读状态为已读。
    /// 标记时会记录阅读时间。
    /// 如果用户已经标记为已读，再次标记会更新阅读时间。
    /// </remarks>
    /// <example>
    /// POST /api/announcement/read/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("read/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> MarkRead(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _announcementService.MarkReadAsync(id, userId);
            return Success(result, "公告已标记为已读");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "标记公告已读失败: {Id}", id);
            return Error<int>("标记公告已读失败");
        }
    }

    /// <summary>
    /// 获取未读公告列表
    /// </summary>
    /// <returns>未读公告列表，最多返回100条</returns>
    /// <response code="200">成功获取未读公告列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取当前用户的所有未读公告。
    /// 只返回已发布且未撤回的公告。
    /// 置顶公告优先显示，然后按发布时间倒序排列。
    /// 适用于消息通知栏、未读公告弹窗等场景。
    /// </remarks>
    /// <example>
    /// GET /api/announcement/unread-list
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("unread-list")]
    [ProducesResponseType(typeof(ApiResponse<List<AnnouncementDto>>), 200)]
    public async Task<ApiResponse<List<AnnouncementDto>>> GetUnreadList()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _announcementService.GetUnreadListAsync(userId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取未读公告列表失败");
            return Error<List<AnnouncementDto>>("获取未读公告列表失败");
        }
    }

    /// <summary>
    /// 获取公告阅读统计
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>阅读统计数据，包含已读人数、未读人数、阅读率等</returns>
    /// <response code="200">成功获取阅读统计</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">公告不存在</response>
    /// <remarks>
    /// 统计指定公告的阅读情况。
    /// 返回已阅读人数、未阅读人数、总目标人数和阅读率。
    /// 阅读率 = 已阅读人数 / 总目标人数 * 100%。
    /// </remarks>
    /// <example>
    /// GET /api/announcement/read-stats/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("read-stats/{id}")]
    [ProducesResponseType(typeof(ApiResponse<ReadStatsDto>), 200)]
    public async Task<ApiResponse<ReadStatsDto>> GetReadStats(Guid id)
    {
        try
        {
            var result = await _announcementService.GetReadStatsAsync(id);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取公告阅读统计失败: {Id}", id);
            return Error<ReadStatsDto>("获取公告阅读统计失败");
        }
    }

    /// <summary>
    /// 获取公告阅读详情列表（分页）
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <param name="pageIndex">页码，从1开始</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="isRead">阅读状态筛选，可选。null-全部 0-未读 1-已读</param>
    /// <returns>分页阅读详情列表，包含用户信息和阅读状态</returns>
    /// <response code="200">成功获取阅读详情列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">公告不存在</response>
    /// <remarks>
    /// 获取指定公告的用户阅读详情列表。
    /// 包含用户基本信息（用户名、真实姓名、角色）和阅读状态（是否已读、阅读时间）。
    /// 支持按阅读状态筛选。
    /// 按阅读时间倒序排列，未读用户按创建时间排序。
    /// </remarks>
    /// <example>
    /// GET /api/announcement/read-detail/3fa85f64-5717-4562-b3fc-2c963f66afa6?pageIndex=1&amp;pageSize=10&amp;isRead=1
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("read-detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<ReadDetailDto>>), 200)]
    public async Task<ApiResponse<PageResponse<ReadDetailDto>>> GetReadDetail(
        Guid id,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? isRead = null)
    {
        try
        {
            var result = await _announcementService.GetReadDetailAsync(id, pageIndex, pageSize, isRead);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取公告阅读详情失败: {Id}", id);
            return Error<PageResponse<ReadDetailDto>>("获取公告阅读详情失败");
        }
    }
}