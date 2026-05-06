namespace EasyWeChatModels.Dto;

/// <summary>
/// 发布状态信息 DTO
/// </summary>
/// <remarks>
/// 用于返回大屏的发布状态信息
/// </remarks>
public class PublishInfoDto
{
    /// <summary>
    /// 是否已发布
    /// </summary>
    /// <example>true</example>
    public bool Published { get; set; }

    /// <summary>
    /// 发布ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    public Guid? PublishId { get; set; }

    /// <summary>
    /// 发布访问URL
    /// </summary>
    /// <example>https://example.com/screen/abc123</example>
    public string? PublishUrl { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    /// <example>2024-01-01 00:00:00</example>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// 浏览次数
    /// </summary>
    /// <example>100</example>
    public int? ViewCount { get; set; }
}