namespace EasyWeChatModels.Dto;

/// <summary>
/// 消息信息 DTO
/// </summary>
/// <remarks>
/// 用于返回消息的基本信息，包含消息内容和用户的阅读状态
/// </remarks>
public class MessageDto
{
    /// <summary>
    /// 消息ID
    /// </summary>
    /// <example>1</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 消息标题
    /// </summary>
    /// <example>系统升级通知</example>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    /// <example>系统将于今晚22:00进行升级维护，届时系统将暂停服务...</example>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 消息类型
    /// </summary>
    /// <remarks>
    /// 消息类型：1-系统消息 2-通知 3-提醒
    /// </remarks>
    /// <example>1</example>
    public int Type { get; set; }

    /// <summary>
    /// 消息类型名称
    /// </summary>
    /// <remarks>
    /// 消息类型的中文描述，便于前端展示
    /// </remarks>
    /// <example>系统消息</example>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// 消息级别
    /// </summary>
    /// <remarks>
    /// 消息级别：1-普通 2-重要 3-紧急
    /// </remarks>
    /// <example>2</example>
    public int Level { get; set; }

    /// <summary>
    /// 消息级别名称
    /// </summary>
    /// <remarks>
    /// 消息级别的中文描述，便于前端展示
    /// </remarks>
    /// <example>重要</example>
    public string LevelName { get; set; } = string.Empty;

    /// <summary>
    /// 发送者ID
    /// </summary>
    /// <remarks>
    /// 发送消息的用户ID，系统消息为null
    /// </remarks>
    /// <example>1</example>
    public Guid? SenderId { get; set; }

    /// <summary>
    /// 发送者名称
    /// </summary>
    /// <example>系统</example>
    public string? SenderName { get; set; }

    /// <summary>
    /// 是否已读
    /// </summary>
    /// <remarks>
    /// 当前用户是否已阅读此消息
    /// </remarks>
    /// <example>false</example>
    public bool IsRead { get; set; }

    /// <summary>
    /// 阅读时间
    /// </summary>
    /// <remarks>
    /// 用户阅读消息的时间，未读时为null
    /// </remarks>
    /// <example>2024-01-01 10:00:00</example>
    public DateTime? ReadTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 消息发送时间
    /// </remarks>
    /// <example>2024-01-01 08:00:00</example>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 消息用户关联ID
    /// </summary>
    /// <remarks>
    /// MessageUser表的ID，用于标记已读、删除等操作
    /// </remarks>
    /// <example>1</example>
    public Guid MessageUserId { get; set; }
}