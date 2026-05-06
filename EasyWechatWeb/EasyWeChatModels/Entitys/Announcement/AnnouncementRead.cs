using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 公告阅读记录实体类
/// </summary>
[SugarTable("AnnouncementRead", "公告阅读记录表")]
public class AnnouncementRead
{
    /// <summary>
    /// 记录ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 公告ID
    /// </summary>
    [SugarColumn(ColumnDescription = "公告ID")]
    public Guid AnnouncementId { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户ID")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 是否已读：0未读 1已读
    /// </summary>
    [SugarColumn(ColumnDescription = "是否已读：0未读 1已读")]
    public int IsRead { get; set; } = 0;

    /// <summary>
    /// 阅读时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "阅读时间")]
    public DateTime? ReadTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}