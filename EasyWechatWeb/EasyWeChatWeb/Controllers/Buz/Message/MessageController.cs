using BusinessManager.Buz.IService;
using BusinessManager.Buz.Service;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 消息控制器
/// </summary>
/// <remarks>
/// 提供消息的发送、接收、阅读状态管理等API接口。
/// 支持系统消息、通知、提醒等多种消息类型。
/// 支持单发和群发消息，支持消息的阅读状态跟踪和删除管理。
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessageController : BaseController
{
    /// <summary>
    /// 消息服务接口
    /// </summary>
    public IMessageService _messageService { get; set; } = null!;
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<MessageController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取消息列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页后的消息列表</returns>
    /// <response code="200">成功获取消息列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取当前用户的消息列表，支持按类型和阅读状态筛选。
    /// 返回结果按创建时间倒序排列，最新的消息排在最前面。
    /// 只返回未被用户删除的消息。
    /// </remarks>
    /// <example>
    /// POST /api/message/list
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "pageIndex": 1,
    ///     "pageSize": 10,
    ///     "type": 1,
    ///     "isRead": 0
    /// }
    /// </example>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<MessageDto>>), 200)]
    public async Task<ApiResponse<PageResponse<MessageDto>>> GetList([FromBody] QueryMessageDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _messageService.GetPageListAsync(userId, query.PageIndex, query.PageSize, query.Type, query.IsRead);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取消息列表失败");
            return Error<PageResponse<MessageDto>>("获取消息列表失败");
        }
    }

    /// <summary>
    /// 获取未读消息列表
    /// </summary>
    /// <returns>未读消息列表，最多100条</returns>
    /// <response code="200">成功获取未读消息列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取当前用户的所有未读消息。
    /// 按消息级别和创建时间排序，紧急消息优先展示。
    /// 适用于消息通知栏、未读消息弹窗等场景。
    /// </remarks>
    /// <example>
    /// GET /api/message/unread
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("unread")]
    [ProducesResponseType(typeof(ApiResponse<List<MessageDto>>), 200)]
    public async Task<ApiResponse<List<MessageDto>>> GetUnreadList()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _messageService.GetUnreadListAsync(userId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取未读消息列表失败");
            return Error<List<MessageDto>>("获取未读消息列表失败");
        }
    }

    /// <summary>
    /// 获取未读消息数量
    /// </summary>
    /// <returns>未读消息数量</returns>
    /// <response code="200">成功获取未读消息数量</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取当前用户的未读消息总数。
    /// 用于消息图标上的未读数量徽标显示。
    /// </remarks>
    /// <example>
    /// GET /api/message/unread-count
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> GetUnreadCount()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _messageService.GetUnreadCountAsync(userId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取未读消息数量失败");
            return Error<int>("获取未读消息数量失败");
        }
    }

    /// <summary>
    /// 获取消息详情
    /// </summary>
    /// <param name="id">消息ID</param>
    /// <returns>消息详细信息</returns>
    /// <response code="200">成功获取消息详情</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">消息不存在或用户无权限查看</response>
    /// <remarks>
    /// 根据消息ID获取消息的完整内容。
    /// 获取详情时会自动将消息标记为已读。
    /// 只能查看属于自己的消息。
    /// </remarks>
    /// <example>
    /// GET /api/message/detail/1
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<MessageDto>), 200)]
    public async Task<ApiResponse<MessageDto>> GetDetail(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _messageService.GetByIdAsync(id, userId);
            if (result == null)
            {
                return Error<MessageDto>("消息不存在或您无权限查看", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取消息详情失败: {Id}", id);
            return Error<MessageDto>("获取消息详情失败");
        }
    }

    /// <summary>
    /// 发送消息给指定用户
    /// </summary>
    /// <param name="dto">发送消息请求参数，包含消息内容和接收者用户ID列表</param>
    /// <returns>新创建的消息ID</returns>
    /// <response code="200">发送成功，返回消息ID</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">参数验证失败</response>
    /// <remarks>
    /// 发送消息给指定的用户列表。
    /// 支持设置消息类型（系统消息、通知、提醒）和级别（普通、重要、紧急）。
    /// UserIds为空时，消息只保存在数据库中，不发送给任何用户。
    /// </remarks>
    /// <example>
    /// POST /api/message/send
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "title": "审批通过通知",
    ///     "content": "您的请假申请已通过审批",
    ///     "type": 2,
    ///     "level": 1,
    ///     "userIds": [1, 2, 3]
    /// }
    /// </example>
    [HttpPost("send")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Send([FromBody] SendMessageDto dto)
    {
        try
        {
            // 设置发送者为当前用户（如果未指定）
            if (dto.SenderId == null)
            {
                dto.SenderId = GetCurrentUserId();
                dto.SenderName = GetCurrentRealName();
            }

            var messageId = await _messageService.SendAsync(dto);
            return Success(messageId, "消息发送成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送消息失败");
            return Error<Guid>("发送消息失败");
        }
    }

    /// <summary>
    /// 发送消息给所有用户（系统广播）
    /// </summary>
    /// <param name="dto">发送消息请求参数，包含消息内容。UserIds参数将被忽略</param>
    /// <returns>新创建的消息ID</returns>
    /// <response code="200">发送成功，返回消息ID</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">参数验证失败</response>
    /// <remarks>
    /// 发送系统广播消息给所有活跃用户。
    /// 通常用于系统公告、重要通知等场景。
    /// 发送者默认为"系统"，可自定义。
    /// </remarks>
    /// <example>
    /// POST /api/message/send-all
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "title": "系统升级公告",
    ///     "content": "系统将于今晚22:00进行升级维护",
    ///     "type": 1,
    ///     "level": 3
    /// }
    /// </example>
    [HttpPost("send-all")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> SendToAll([FromBody] SendMessageDto dto)
    {
        try
        {
            // 设置发送者信息
            dto.SenderId = GetCurrentUserId();
            dto.SenderName = dto.SenderName ?? GetCurrentRealName() ?? "系统";

            var messageId = await _messageService.SendToAllAsync(dto);
            return Success(messageId, "系统广播消息发送成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送系统广播消息失败");
            return Error<Guid>("发送系统广播消息失败");
        }
    }

    /// <summary>
    /// 标记消息为已读
    /// </summary>
    /// <param name="id">消息ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">标记成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 将指定消息标记为已读状态。
    /// 只能标记属于自己的消息。
    /// 如果消息已经已读，不影响状态，返回成功。
    /// </remarks>
    /// <example>
    /// PUT /api/message/read/1
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPut("read/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> MarkRead(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var affected = await _messageService.MarkReadAsync(id, userId);
            return Success(affected, "消息已标记为已读");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "标记消息已读失败: {Id}", id);
            return Error<int>("标记消息已读失败");
        }
    }

    /// <summary>
    /// 标记所有消息为已读
    /// </summary>
    /// <returns>标记为已读的消息数量</returns>
    /// <response code="200">标记成功，返回已读消息数量</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 一键将当前用户的所有未读消息标记为已读。
    /// 用于批量处理未读消息的场景。
    /// </remarks>
    /// <example>
    /// PUT /api/message/read-all
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPut("read-all")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> MarkAllRead()
    {
        try
        {
            var userId = GetCurrentUserId();
            var affected = await _messageService.MarkAllReadAsync(userId);
            return Success(affected, $"已将 {affected} 条消息标记为已读");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "标记所有消息已读失败");
            return Error<int>("标记所有消息已读失败");
        }
    }

    /// <summary>
    /// 删除消息（软删除）
    /// </summary>
    /// <param name="id">消息ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 将消息从当前用户的消息列表中移除（软删除）。
    /// 消息不会从数据库中物理删除，其他用户仍可查看。
    /// 只能删除属于自己的消息。
    /// </remarks>
    /// <example>
    /// POST /api/message/delete
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    /// </example>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var affected = await _messageService.DeleteAsync(id, userId);
            return Success(affected, "消息已删除");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除消息失败: {Id}", id);
            return Error<int>("删除消息失败");
        }
    }
}