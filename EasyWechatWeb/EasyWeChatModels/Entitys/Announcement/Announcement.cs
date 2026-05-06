using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 系统公告实体类
/// </summary>
[SugarTable("Announcement", "系统公告表")]
public class Announcement
{
    /// <summary>
    /// 公告ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 标题
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "标题")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 内容（富文本）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", ColumnDescription = "内容")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 类型：1全员公告 2定向公告
    /// </summary>
    [SugarColumn(ColumnDescription = "类型：1全员公告 2定向公告")]
    public int Type { get; set; } = 1;

    /// <summary>
    /// 级别：1普通 2重要 3紧急
    /// </summary>
    [SugarColumn(ColumnDescription = "级别：1普通 2重要 3紧急")]
    public int Level { get; set; } = 1;

    /// <summary>
    /// 目标角色ID列表（JSON数组）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "目标角色ID列表")]
    public string? TargetRoleIds { get; set; }

    /// <summary>
    /// 是否置顶：0否 1是
    /// </summary>
    [SugarColumn(ColumnDescription = "是否置顶：0否 1是")]
    public int IsTop { get; set; } = 0;

    /// <summary>
    /// 置顶时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "置顶时间")]
    public DateTime? TopTime { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "发布时间")]
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 撤回时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "撤回时间")]
    public DateTime? RecallTime { get; set; }

    /// <summary>
    /// 状态：0草稿 1已发布 2已撤回
    /// </summary>
    [SugarColumn(ColumnDescription = "状态：0草稿 1已发布 2已撤回")]
    public int Status { get; set; } = 0;

    /// <summary>
    /// 创建人ID
    /// </summary>
    [SugarColumn(ColumnDescription = "创建人ID")]
    public Guid CreatorId { get; set; }

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