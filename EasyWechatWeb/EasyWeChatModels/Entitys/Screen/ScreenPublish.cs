using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 大屏发布记录实体类
/// </summary>
/// <remarks>
/// 用于存储大屏的发布历史记录，包括发布版本、访问统计等
/// </remarks>
[SugarTable("ScreenPublish", "大屏发布记录表")]
public class ScreenPublish
{
    /// <summary>
    /// 发布记录ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 大屏ID
    /// </summary>
    /// <remarks>
    /// 关联的大屏配置ID
    /// </remarks>
    [SugarColumn(ColumnDescription = "大屏ID")]
    public Guid ScreenId { get; set; }

    /// <summary>
    /// 大屏名称
    /// </summary>
    /// <remarks>
    /// 发布时的大屏名称快照，长度限制100字符
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "大屏名称")]
    public string ScreenName { get; set; } = string.Empty;

    /// <summary>
    /// 大屏数据快照
    /// </summary>
    /// <remarks>
    /// 发布时的大屏完整配置数据快照，JSON格式存储
    /// </remarks>
    [SugarColumn(ColumnDataType = "longtext", ColumnDescription = "大屏数据快照")]
    public string ScreenData { get; set; } = string.Empty;

    /// <summary>
    /// 发布URL
    /// </summary>
    /// <remarks>
    /// 发布后的大屏访问URL地址，长度限制100字符
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "发布URL")]
    public string PublishUrl { get; set; } = string.Empty;

    /// <summary>
    /// 发布人ID
    /// </summary>
    /// <remarks>
    /// 执行发布操作的用户ID
    /// </remarks>
    [SugarColumn(ColumnDescription = "发布人ID")]
    public Guid PublishedBy { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    /// <remarks>
    /// 大屏发布的日期时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "发布时间")]
    public DateTime PublishTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 浏览次数
    /// </summary>
    /// <remarks>
    /// 该发布版本被访问的次数统计，默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "浏览次数")]
    public int ViewCount { get; set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 发布状态：1-已发布，0-已下线。默认值为1
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态：1-已发布，0-已下线")]
    public int Status { get; set; } = 1;
}