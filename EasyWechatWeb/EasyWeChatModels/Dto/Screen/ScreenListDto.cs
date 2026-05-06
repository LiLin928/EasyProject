namespace EasyWeChatModels.Dto;

/// <summary>
/// 大屏列表项 DTO
/// </summary>
/// <remarks>
/// 用于列表展示的大屏简要信息
/// </remarks>
public class ScreenListDto
{
    /// <summary>
    /// 大屏ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 大屏名称
    /// </summary>
    /// <example>销售数据大屏</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 大屏描述
    /// </summary>
    /// <example>展示销售数据的可视化大屏</example>
    public string? Description { get; set; }

    /// <summary>
    /// 缩略图URL
    /// </summary>
    /// <example>https://example.com/thumbnail.png</example>
    public string? Thumbnail { get; set; }

    /// <summary>
    /// 是否公开
    /// </summary>
    /// <remarks>
    /// 0-私有，1-公开
    /// </remarks>
    /// <example>0</example>
    public int IsPublic { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 0-草稿，1-已发布，2-已下架
    /// </remarks>
    /// <example>0</example>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <example>2024-01-01 00:00:00</example>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <example>2024-01-02 00:00:00</example>
    public DateTime? UpdateTime { get; set; }
}