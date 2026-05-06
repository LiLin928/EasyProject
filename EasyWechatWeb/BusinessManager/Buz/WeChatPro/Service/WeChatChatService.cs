using BusinessManager.Buz.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 微信聊天服务实现类
/// </summary>
/// <remarks>
/// 实现聊天相关的业务逻辑，包括会话管理、消息存储和查询等功能。
/// 继承自<see cref="BaseService{ChatSession}"/>，使用SqlSugar进行数据库操作。
/// </remarks>
public class WeChatChatService : BaseService<ChatSession>, IWeChatChatService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatChatService> _logger { get; set; } = null!;

    /// <summary>
    /// 创建或获取会话
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>会话信息</returns>
    /// <remarks>
    /// 每个用户只有一个会话，如果已存在则返回现有会话
    /// </remarks>
    public async Task<ChatSessionDto> GetOrCreateSessionAsync(Guid userId)
    {
        var session = await GetFirstAsync(s => s.UserId == userId && s.Status == SessionStatus.Active);

        if (session == null)
        {
            session = new ChatSession
            {
                UserId = userId,
                Status = SessionStatus.Active,
                LastMessageTime = DateTime.Now,
                UnreadCount = 0,
                CreateTime = DateTime.Now
            };

            await InsertAsync(session);
            _logger.LogInformation("用户 {UserId} 创建新会话 {SessionId}", userId, session.Id);
        }

        return new ChatSessionDto
        {
            SessionId = session.Id,
            Status = session.Status,
            UnreadCount = session.UnreadCount,
            LastMessage = session.LastMessageContent,
            LastMessageTime = new DateTimeOffset(session.LastMessageTime).ToUnixTimeMilliseconds(),
            CreateTime = new DateTimeOffset(session.CreateTime).ToUnixTimeMilliseconds()
        };
    }

    /// <summary>
    /// 获取历史消息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="query">查询参数，包含分页和会话ID</param>
    /// <returns>消息分页列表</returns>
    /// <exception cref="BusinessException">会话不存在时抛出异常</exception>
    /// <remarks>
    /// 查询指定会话的历史消息，按发送时间倒序排列。
    /// 包含发送者信息（客户显示昵称/用户名，客服显示"客服"）。
    /// </remarks>
    public async Task<PageResponse<ChatMessageDto>> GetMessagesAsync(Guid userId, QueryChatMessageDto query)
    {
        var session = await GetFirstAsync(s => s.Id == query.SessionId && s.UserId == userId);
        if (session == null)
        {
            throw BusinessException.NotFound("会话不存在");
        }

        var total = new RefAsync<int>();
        var messages = await _db.Queryable<ChatMessage>()
            .Where(m => m.SessionId == query.SessionId)
            .OrderByDescending(m => m.CreateTime)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        var senderIds = messages.Select(m => m.SenderId).Distinct().ToList();
        // 只查询需要的字段，避免实体与表结构不匹配的问题
        var users = await _db.Queryable<WeChatUser>()
            .Where(u => senderIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Nickname, u.UserName })
            .ToListAsync();

        var userDict = users.ToDictionary(u => u.Id);

        var messageDtos = messages.Select(m =>
        {
            var senderName = "客服";
            if (m.SenderType == SenderType.Customer && userDict.TryGetValue(m.SenderId, out var user))
            {
                senderName = user.Nickname ?? user.UserName;
            }

            return new ChatMessageDto
            {
                MessageId = m.Id,
                SenderType = m.SenderType,
                SenderName = senderName,
                MessageType = m.MessageType,
                Content = m.Content,
                Duration = m.Duration ?? 0,
                CreateTime = new DateTimeOffset(m.CreateTime).ToUnixTimeMilliseconds(),
                IsRead = m.IsRead
            };
        }).ToList();

        return PageResponse<ChatMessageDto>.Create(messageDtos, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 保存消息
    /// </summary>
    /// <param name="sessionId">会话ID</param>
    /// <param name="senderType">发送者类型：0-客户，1-客服</param>
    /// <param name="senderId">发送者ID</param>
    /// <param name="messageType">消息类型：0-文字，1-图片，2-语音</param>
    /// <param name="content">消息内容</param>
    /// <param name="duration">语音时长（秒），仅语音消息有效</param>
    /// <returns>消息 DTO</returns>
    /// <exception cref="BusinessException">会话不存在时抛出异常</exception>
    /// <remarks>
    /// 保存消息到数据库，同时更新会话的最后消息时间和摘要。
    /// 客服发送的消息会增加客户的未读计数。
    /// </remarks>
    public async Task<ChatMessageDto> SaveMessageAsync(Guid sessionId, int senderType, Guid senderId, int messageType, string content, int duration = 0)
    {
        var session = await GetByIdAsync(sessionId);
        if (session == null)
        {
            throw BusinessException.NotFound("会话不存在");
        }

        var summary = content.Length > 50 ? content.Substring(0, 50) + "..." : content;

        var message = new ChatMessage
        {
            SessionId = sessionId,
            SenderType = senderType,
            SenderId = senderId,
            MessageType = messageType,
            Content = content,
            Duration = duration,
            IsRead = senderType == SenderType.Customer,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(message).ExecuteCommandAsync();

        session.LastMessageTime = DateTime.Now;
        session.LastMessageContent = summary;
        session.UpdateTime = DateTime.Now;

        if (senderType == SenderType.Staff)
        {
            session.UnreadCount++;
        }

        await UpdateAsync(session);

        var senderName = "客服";
        if (senderType == SenderType.Customer)
        {
            // 只查询需要的字段，避免实体与表结构不匹配的问题
            var user = await _db.Queryable<WeChatUser>()
                .Where(u => u.Id == senderId)
                .Select(u => new { u.Nickname, u.UserName })
                .FirstAsync();
            if (user != null)
            {
                senderName = user.Nickname ?? user.UserName;
            }
        }

        _logger.LogInformation("会话 {SessionId} 收到新消息，发送者：{SenderType}", sessionId, senderType);

        return new ChatMessageDto
        {
            MessageId = message.Id,
            SenderType = message.SenderType,
            SenderName = senderName,
            MessageType = message.MessageType,
            Content = message.Content,
            Duration = message.Duration ?? 0,
            CreateTime = new DateTimeOffset(message.CreateTime).ToUnixTimeMilliseconds(),
            IsRead = message.IsRead
        };
    }

    /// <summary>
    /// 标记消息已读
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="sessionId">会话ID</param>
    /// <returns>是否成功；会话不存在返回 false</returns>
    /// <remarks>
    /// 将会话中所有客服发送的未读消息标记为已读，并清零未读计数。
    /// </remarks>
    public async Task<bool> MarkAsReadAsync(Guid userId, Guid sessionId)
    {
        var session = await GetFirstAsync(s => s.Id == sessionId && s.UserId == userId);
        if (session == null)
        {
            return false;
        }

        await _db.Updateable<ChatMessage>()
            .Where(m => m.SessionId == sessionId && m.SenderType == SenderType.Staff && !m.IsRead)
            .SetColumns(m => m.IsRead == true)
            .ExecuteCommandAsync();

        session.UnreadCount = 0;
        session.UpdateTime = DateTime.Now;
        await UpdateAsync(session);

        _logger.LogInformation("用户 {UserId} 标记会话 {SessionId} 已读", userId, sessionId);

        return true;
    }

    /// <summary>
    /// 获取用户会话ID
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>会话ID；不存在返回 Guid.Empty</returns>
    /// <remarks>
    /// 用于验证 WebSocket 连接，快速获取用户的活跃会话ID。
    /// </remarks>
    public async Task<Guid> GetSessionIdByUserIdAsync(Guid userId)
    {
        var session = await GetFirstAsync(s => s.UserId == userId && s.Status == SessionStatus.Active);
        return session?.Id ?? Guid.Empty;
    }
}