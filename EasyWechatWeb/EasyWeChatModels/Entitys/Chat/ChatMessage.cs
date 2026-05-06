using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 聊天消息实体类
/// </summary>
/// <remarks>
/// 用于存储聊天会话中的所有消息记录
/// </remarks>
[SugarTable("ChatMessage", "聊天消息表")]
public class ChatMessage
{
    /// <summary>
    /// 消息ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 会话ID（外键关联 ChatSession）
    /// </summary>
    /// <remarks>
    /// 外键关联 ChatSession 表
    /// </remarks>
    [SugarColumn(ColumnDescription = "会话ID")]
    public Guid SessionId { get; set; }

    /// <summary>
    /// 发送者类型
    /// </summary>
    /// <remarks>
    /// 发送者类型：0-客户，1-客服
    /// </remarks>
    [SugarColumn(ColumnDescription = "发送者类型：0-客户，1-客服")]
    public int SenderType { get; set; }

    /// <summary>
    /// 发送者ID
    /// </summary>
    /// <remarks>
    /// 客户发送时为 UserId，客服发送时为客服用户ID
    /// </remarks>
    [SugarColumn(ColumnDescription = "发送者ID")]
    public Guid SenderId { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    /// <remarks>
    /// 消息类型：0-文字，1-图片，2-语音
    /// </remarks>
    [SugarColumn(ColumnDescription = "消息类型：0-文字，1-图片，2-语音")]
    public int MessageType { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    /// <remarks>
    /// 文字内容、图片URL或语音URL，长度限制2000字符
    /// </remarks>
    [SugarColumn(Length = 2000, ColumnDescription = "消息内容")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 语音时长（秒）
    /// </summary>
    /// <remarks>
    /// 仅语音消息有效，其他类型为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "语音时长（秒）")]
    public int? Duration { get; set; }

    /// <summary>
    /// 是否已读
    /// </summary>
    /// <remarks>
    /// 默认为 false，接收方查看后标记为 true
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否已读")]
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// 发送时间
    /// </summary>
    /// <remarks>
    /// 消息发送时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "发送时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}