using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.WeChatPro;

/// <summary>
/// 微信聊天控制器
/// 提供聊天会话管理、历史消息查询等功能
/// </summary>
[ApiController]
[Route("api/wechat/chat")]
[Authorize]
public class WeChatChatController : BaseController
{
    /// <summary>
    /// 聊天服务接口
    /// </summary>
    public IWeChatChatService _chatService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatChatController> _logger { get; set; } = null!;

    /// <summary>
    /// 创建或获取会话
    /// </summary>
    /// <returns>会话信息</returns>
    [HttpPost("session")]
    [ProducesResponseType(typeof(ApiResponse<ChatSessionDto>), 200)]
    public async Task<ApiResponse<ChatSessionDto>> GetOrCreateSession()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<ChatSessionDto>("请先登录", 401);
            }

            var result = await _chatService.GetOrCreateSessionAsync(userId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取会话失败");
            return Error<ChatSessionDto>("获取会话失败");
        }
    }

    /// <summary>
    /// 获取历史消息
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>消息分页列表</returns>
    [HttpGet("messages")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<ChatMessageDto>>), 200)]
    public async Task<ApiResponse<PageResponse<ChatMessageDto>>> GetMessages([FromQuery] QueryChatMessageDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<PageResponse<ChatMessageDto>>("请先登录", 401);
            }

            var result = await _chatService.GetMessagesAsync(userId, query);
            return Success(result);
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            _logger.LogWarning(ex, "获取消息失败: {Message}", ex.Message);
            return Error<PageResponse<ChatMessageDto>>(ex.Message, ex.ErrorCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取消息失败");
            return Error<PageResponse<ChatMessageDto>>("获取消息失败");
        }
    }

    /// <summary>
    /// 标记消息已读
    /// </summary>
    /// <param name="dto">标记已读请求</param>
    /// <returns>操作结果</returns>
    [HttpPost("read")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> MarkAsRead([FromBody] MarkReadDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<bool>("请先登录", 401);
            }

            var result = await _chatService.MarkAsReadAsync(userId, dto.SessionId);
            if (!result)
            {
                return Error<bool>("会话不存在", 404);
            }
            return Success(result, "标记成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "标记已读失败");
            return Error<bool>("标记已读失败");
        }
    }
}