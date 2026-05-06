namespace EasyWeChatModels.Dto;

/// <summary>
/// 消息用户关联 DTO
/// </summary>
/// <remarks>
/// 用于返回消息与用户的关联信息，主要用于内部查询
/// </remarks>
public class MessageUserDto
{
    /// <summary>
    /// 关联ID
    /// </summary>
    /// <example>1</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 消息ID
    /// </summary>
    /// <example>1</example>
    public Guid MessageId { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    /// <example>1</example>
    public Guid UserId { get; set; }

    /// <summary>
    /// 是否已读
    /// </summary>
    /// <example>false</example>
    public bool IsRead { get; set; }

    /// <summary>
    /// 阅读时间
    /// </summary>
    /// <example>2024-01-01 10:00:00</example>
    public DateTime? ReadTime { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    /// <example>false</example>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    /// <example>null</example>
    public DateTime? DeleteTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <example>2024-01-01 08:00:00</example>
    public DateTime CreateTime { get; set; }
}