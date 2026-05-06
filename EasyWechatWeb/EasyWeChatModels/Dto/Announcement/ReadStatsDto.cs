namespace EasyWeChatModels.Dto;

/// <summary>
/// 公告阅读统计 DTO
/// </summary>
/// <remarks>
/// 用于返回公告的阅读统计数据
/// </remarks>
public class ReadStatsDto
{
    /// <summary>
    /// 公告ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid AnnouncementId { get; set; }

    /// <summary>
    /// 公告标题
    /// </summary>
    /// <example>系统升级通知</example>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 已阅读人数
    /// </summary>
    /// <remarks>
    /// 已经阅读该公告的用户数量
    /// </remarks>
    /// <example>50</example>
    public int ReadCount { get; set; }

    /// <summary>
    /// 未阅读人数
    /// </summary>
    /// <remarks>
    /// 尚未阅读该公告的用户数量
    /// </remarks>
    /// <example>50</example>
    public int UnreadCount { get; set; }

    /// <summary>
    /// 总目标人数
    /// </summary>
    /// <remarks>
    /// 该公告应送达的用户总数
    /// </remarks>
    /// <example>100</example>
    public int TotalCount { get; set; }

    /// <summary>
    /// 阅读率
    /// </summary>
    /// <remarks>
    /// 阅读人数占总人数的百分比
    /// </remarks>
    /// <example>50.00</example>
    public decimal ReadRate { get; set; }
}