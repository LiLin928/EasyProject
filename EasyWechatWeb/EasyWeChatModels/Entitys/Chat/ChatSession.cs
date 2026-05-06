using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 聊天会话实体类
/// </summary>
/// <remarks>
/// 用于存储客户与客服的聊天会话信息，每个客户只有一个会话
/// </remarks>
[SugarTable("ChatSession", "聊天会话表")]
public class ChatSession
{
    /// <summary>
    /// 会话ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 客户用户ID（外键关联 WeChatUser）
    /// </summary>
    /// <remarks>
    /// 每个客户只能有一个会话
    /// </remarks>
    [SugarColumn(ColumnDescription = "客户用户ID")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 会话状态
    /// </summary>
    /// <remarks>
    /// 会话状态：0-进行中，1-已结束
    /// </remarks>
    [SugarColumn(ColumnDescription = "会话状态：0-进行中，1-已结束")]
    public int Status { get; set; } = 0;

    /// <summary>
    /// 最后消息时间
    /// </summary>
    /// <remarks>
    /// 用于排序会话列表，显示最新活跃的会话
    /// </remarks>
    [SugarColumn(ColumnDescription = "最后消息时间")]
    public DateTime LastMessageTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 最后消息摘要
    /// </summary>
    /// <remarks>
    /// 最后一条消息的内容摘要，长度限制500字符
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "最后消息摘要")]
    public string? LastMessageContent { get; set; }

    /// <summary>
    /// 客户未读消息数
    /// </summary>
    /// <remarks>
    /// 客户端显示的未读消息数量，客服发送消息后增加
    /// </remarks>
    [SugarColumn(ColumnDescription = "客户未读消息数")]
    public int UnreadCount { get; set; } = 0;

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}