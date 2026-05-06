namespace EasyWeChatModels.Dto;

/// <summary>
/// WebSocket 接收消息
/// </summary>
public class WsReceiveMessage
{
    /// <summary>
    /// 消息类型：message / system
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 消息ID
    /// </summary>
    public Guid? MessageId { get; set; }

    /// <summary>
    /// 发送者类型：0-客户，1-客服
    /// </summary>
    public int? SenderType { get; set; }

    /// <summary>
    /// 发送者名称
    /// </summary>
    public string? SenderName { get; set; }

    /// <summary>
    /// 消息类型：0-文字，1-图片，2-语音
    /// </summary>
    public int? MessageType { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 发送时间（Unix 毫秒时间戳）
    /// </summary>
    public long? CreateTime { get; set; }
}