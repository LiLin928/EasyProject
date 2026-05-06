namespace EasyWeChatModels.Dto;

/// <summary>
/// 聊天消息 DTO
/// </summary>
public class ChatMessageDto
{
    /// <summary>
    /// 消息ID
    /// </summary>
    public Guid MessageId { get; set; }

    /// <summary>
    /// 发送者类型：0-客户，1-客服
    /// </summary>
    public int SenderType { get; set; }

    /// <summary>
    /// 发送者名称
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// 消息类型：0-文字，1-图片，2-语音
    /// </summary>
    public int MessageType { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 语音时长（秒）
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// 发送时间（Unix 毫秒时间戳）
    /// </summary>
    public long CreateTime { get; set; }

    /// <summary>
    /// 是否已读
    /// </summary>
    public bool IsRead { get; set; }
}