namespace EasyWeChatModels.Dto;

/// <summary>
/// 公告信息 DTO
/// </summary>
/// <remarks>
/// 用于返回公告的基本信息，包含公告内容和发布状态
/// </remarks>
public class AnnouncementDto
{
    /// <summary>
    /// 公告ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    /// <example>系统升级通知</example>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 内容（富文本）
    /// </summary>
    /// <example>系统将于今晚22:00进行升级维护...</example>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 类型：1全员公告 2定向公告
    /// </summary>
    /// <remarks>
    /// 全员公告发送给所有用户，定向公告发送给指定角色用户
    /// </remarks>
    /// <example>1</example>
    public int Type { get; set; }

    /// <summary>
    /// 类型名称
    /// </summary>
    /// <example>全员公告</example>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// 级别：1普通 2重要 3紧急
    /// </summary>
    /// <remarks>
    /// 公告级别，用于区分重要程度，影响前端展示样式
    /// </remarks>
    /// <example>2</example>
    public int Level { get; set; }

    /// <summary>
    /// 级别名称
    /// </summary>
    /// <example>重要</example>
    public string LevelName { get; set; } = string.Empty;

    /// <summary>
    /// 目标角色ID列表
    /// </summary>
    /// <remarks>
    /// 定向公告时指定接收的角色ID，全员公告时为空
    /// </remarks>
    /// <example>["3fa85f64-5717-4562-b3fc-2c963f66afa6"]</example>
    public List<Guid>? TargetRoleIds { get; set; }

    /// <summary>
    /// 目标角色名称列表
    /// </summary>
    /// <example>["管理员", "运营人员"]</example>
    public List<string>? TargetRoleNames { get; set; }

    /// <summary>
    /// 是否置顶：0否 1是
    /// </summary>
    /// <example>1</example>
    public int IsTop { get; set; }

    /// <summary>
    /// 置顶时间
    /// </summary>
    /// <example>2024-01-01T10:00:00</example>
    public DateTime? TopTime { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    /// <example>2024-01-01T08:00:00</example>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 撤回时间
    /// </summary>
    /// <example>null</example>
    public DateTime? RecallTime { get; set; }

    /// <summary>
    /// 状态：0草稿 1已发布 2已撤回
    /// </summary>
    /// <example>1</example>
    public int Status { get; set; }

    /// <summary>
    /// 状态名称
    /// </summary>
    /// <example>已发布</example>
    public string StatusName { get; set; } = string.Empty;

    /// <summary>
    /// 创建人ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid CreatorId { get; set; }

    /// <summary>
    /// 创建人名称
    /// </summary>
    /// <example>管理员</example>
    public string? CreatorName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <example>2024-01-01T08:00:00</example>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <example>2024-01-02T10:00:00</example>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 阅读人数
    /// </summary>
    /// <remarks>
    /// 已阅读该公告的用户数量
    /// </remarks>
    /// <example>50</example>
    public int ReadCount { get; set; }

    /// <summary>
    /// 总目标人数
    /// </summary>
    /// <remarks>
    /// 该公告应送达的用户总数
    /// </remarks>
    /// <example>100</example>
    public int TotalCount { get; set; }

    /// <summary>
    /// 当前用户是否已读（仅详情查询时返回）
    /// </summary>
    /// <remarks>
    /// 标识当前用户对该公告的阅读状态：0未读 1已读
    /// 仅在查询详情时提供此字段，列表查询不返回
    /// </remarks>
    /// <example>1</example>
    public int? IsRead { get; set; }

    /// <summary>
    /// 附件列表
    /// </summary>
    /// <remarks>
    /// 公告关联的附件文件列表，包含文件名、大小、类型等信息
    /// </remarks>
    /// <example>[{"id":"xxx","fileName":"report.pdf","fileSize":1024,"fileExt":"pdf"}]</example>
    public List<FileRecordDto>? Attachments { get; set; }
}