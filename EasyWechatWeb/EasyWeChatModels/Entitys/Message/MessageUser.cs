using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 消息用户关联实体类
/// </summary>
/// <remarks>
/// 用于存储消息与用户的关联关系，包括阅读状态、删除状态等。
/// 支持群发消息：一条消息可以关联多个用户，每个用户有独立的阅读和删除状态。
/// </remarks>
[SugarTable("MessageUser", "消息用户关联表")]
public class MessageUser
{
    /// <summary>
    /// 关联ID（主键）
    /// </summary>
    /// <remarks>
    /// 消息用户关联记录的唯一标识
    /// </remarks>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 消息ID
    /// </summary>
    /// <remarks>
    /// 关联的消息ID，对应Message表的Id字段
    /// </remarks>
    [SugarColumn(ColumnDescription = "消息ID")]
    public Guid MessageId { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    /// <remarks>
    /// 接收消息的用户ID，对应User表的Id字段
    /// </remarks>
    [SugarColumn(ColumnDescription = "用户ID")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 是否已读
    /// </summary>
    /// <remarks>
    /// 阅读状态标识：
    /// - false: 未读 - 用户尚未查看此消息
    /// - true: 已读 - 用户已查看此消息
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否已读")]
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// 阅读时间
    /// </summary>
    /// <remarks>
    /// 用户阅读消息的时间，未读时为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "阅读时间")]
    public DateTime? ReadTime { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    /// <remarks>
    /// 删除状态标识（软删除）：
    /// - false: 未删除 - 消息在用户的消息列表中可见
    /// - true: 已删除 - 用户已将消息从列表中移除
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否已删除")]
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 删除时间
    /// </summary>
    /// <remarks>
    /// 用户删除消息的时间，未删除时为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "删除时间")]
    public DateTime? DeleteTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 关联记录创建时间，即消息发送给用户的时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}