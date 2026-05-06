namespace EasyWeChatModels.Dto;

/// <summary>
/// 会话信息 DTO
/// </summary>
public class ChatSessionDto
{
    /// <summary>
    /// 会话ID
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// 会话状态：0-进行中，1-已结束
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 客户未读消息数
    /// </summary>
    public int UnreadCount { get; set; }

    /// <summary>
    /// 最后消息摘要
    /// </summary>
    public string? LastMessage { get; set; }

    /// <summary>
    /// 最后消息时间（Unix 毫秒时间戳）
    /// </summary>
    public long LastMessageTime { get; set; }

    /// <summary>
    /// 会话创建时间（Unix 毫秒时间戳）
    /// </summary>
    public long CreateTime { get; set; }
}