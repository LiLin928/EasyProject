namespace EasyWeChatModels.Dto;

/// <summary>
/// 大屏发布记录响应 DTO
/// </summary>
/// <remarks>
/// 用于返回发布详情信息
/// </remarks>
public class ScreenPublishDto
{
    /// <summary>
    /// 发布ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    public Guid PublishId { get; set; }

    /// <summary>
    /// 大屏ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    public Guid ScreenId { get; set; }

    /// <summary>
    /// 大屏名称
    /// </summary>
    /// <example>销售数据大屏</example>
    public string ScreenName { get; set; } = string.Empty;

    /// <summary>
    /// 发布时的大屏数据快照（JSON格式）
    /// </summary>
    /// <example>{"screen":{},"components":[]}</example>
    public string ScreenData { get; set; } = "{}";

    /// <summary>
    /// 发布访问URL
    /// </summary>
    /// <example>https://example.com/screen/abc123</example>
    public string PublishUrl { get; set; } = string.Empty;

    /// <summary>
    /// 发布者ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    public Guid PublishedBy { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    /// <example>2024-01-01 00:00:00</example>
    public DateTime? PublishTime { get; set; }

    /// <summary>
    /// 浏览次数
    /// </summary>
    /// <example>100</example>
    public int ViewCount { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 0-已下架，1-已发布
    /// </remarks>
    /// <example>1</example>
    public int Status { get; set; }
}