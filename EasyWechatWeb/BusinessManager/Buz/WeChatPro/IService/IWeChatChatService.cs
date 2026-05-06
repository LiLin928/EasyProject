using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 微信聊天服务接口
/// </summary>
public interface IWeChatChatService
{
    /// <summary>
    /// 创建或获取会话
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>会话信息</returns>
    /// <remarks>
    /// 每个用户只有一个会话，如果已存在则返回现有会话
    /// </remarks>
    Task<ChatSessionDto> GetOrCreateSessionAsync(Guid userId);

    /// <summary>
    /// 获取历史消息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="query">查询参数</param>
    /// <returns>消息分页列表</returns>
    Task<PageResponse<ChatMessageDto>> GetMessagesAsync(Guid userId, QueryChatMessageDto query);

    /// <summary>
    /// 保存消息
    /// </summary>
    /// <param name="sessionId">会话ID</param>
    /// <param name="senderType">发送者类型</param>
    /// <param name="senderId">发送者ID</param>
    /// <param name="messageType">消息类型</param>
    /// <param name="content">消息内容</param>
    /// <param name="duration">语音时长</param>
    /// <returns>消息 DTO</returns>
    Task<ChatMessageDto> SaveMessageAsync(Guid sessionId, int senderType, Guid senderId, int messageType, string content, int duration = 0);

    /// <summary>
    /// 标记消息已读
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="sessionId">会话ID</param>
    /// <returns>是否成功</returns>
    Task<bool> MarkAsReadAsync(Guid userId, Guid sessionId);

    /// <summary>
    /// 获取用户会话（用于验证 WebSocket 连接）
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>会话实体</returns>
    Task<Guid> GetSessionIdByUserIdAsync(Guid userId);
}