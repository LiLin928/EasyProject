using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 消息实体类
/// </summary>
/// <remarks>
/// 用于存储系统消息、通知、提醒等消息信息。
/// 消息可以通过MessageUser表与用户建立关联，实现消息的分发和阅读状态跟踪。
/// </remarks>
[SugarTable("Message", "消息表")]
public class Message
{
    /// <summary>
    /// 消息ID（主键）
    /// </summary>
    /// <remarks>
    /// 消息的唯一标识，使用Guid类型
    /// </remarks>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 消息标题
    /// </summary>
    /// <remarks>
    /// 消息的标题，用于消息列表展示，长度限制200字符
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "消息标题")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    /// <remarks>
    /// 消息的详细内容，支持富文本格式，使用Text类型存储大量内容
    /// </remarks>
    [SugarColumn(ColumnDataType = "text", ColumnDescription = "消息内容")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 消息类型
    /// </summary>
    /// <remarks>
    /// 消息类型定义：
    /// - 1: 系统消息 - 系统自动发送的消息，如系统升级通知
    /// - 2: 通知 - 业务相关的通知，如审批通过通知
    /// - 3: 提醒 - 用户相关的提醒，如任务到期提醒
    /// </remarks>
    [SugarColumn(ColumnDescription = "消息类型：1-系统消息 2-通知 3-提醒")]
    public int Type { get; set; } = 1;

    /// <summary>
    /// 消息级别
    /// </summary>
    /// <remarks>
    /// 消息级别定义：
    /// - 1: 普通 - 普通级别的消息，可稍后查看
    /// - 2: 重要 - 重要级别的消息，需要及时查看
    /// - 3: 紧急 - 紧急级别的消息，需要立即处理
    /// </remarks>
    [SugarColumn(ColumnDescription = "消息级别：1-普通 2-重要 3-紧急")]
    public int Level { get; set; } = 1;

    /// <summary>
    /// 发送者ID
    /// </summary>
    /// <remarks>
    /// 发送消息的用户ID，系统消息可为空表示系统发送
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "发送者ID")]
    public Guid? SenderId { get; set; }

    /// <summary>
    /// 发送者名称
    /// </summary>
    /// <remarks>
    /// 发送者的显示名称，用于消息展示，系统消息可设置为"系统"
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "发送者名称")]
    public string? SenderName { get; set; }

    /// <summary>
    /// 消息状态
    /// </summary>
    /// <remarks>
    /// 消息状态定义：
    /// - 1: 正常 - 消息正常，可以查看
    /// - 0: 禁用 - 消息被禁用，不再展示
    /// </remarks>
    [SugarColumn(ColumnDescription = "消息状态：1-正常 0-禁用")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 消息创建时间，即消息发送时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}