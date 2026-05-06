namespace EasyWeChatModels.Dto;

/// <summary>
/// WebSocket 发送消息
/// </summary>
public class WsSendMessage
{
    /// <summary>
    /// 消息类型：message
    /// </summary>
    public string Type { get; set; } = "message";

    /// <summary>
    /// 会话ID
    /// </summary>
    public Guid SessionId { get; set; }

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
}