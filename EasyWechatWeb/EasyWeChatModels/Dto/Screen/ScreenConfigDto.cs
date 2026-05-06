using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 大屏详情响应 DTO
/// </summary>
/// <remarks>
/// 包含大屏基本信息及其所有组件
/// </remarks>
public class ScreenConfigDto
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
    /// 大屏样式配置（JSON格式）
    /// </summary>
    /// <example>{"backgroundColor":"#1a1a2e","gridSize":10}</example>
    public string Style { get; set; } = "{}";

    /// <summary>
    /// 权限配置（JSON格式）
    /// </summary>
    /// <example>{"view":["user1","user2"],"edit":["user1"]}</example>
    public string? Permissions { get; set; }

    /// <summary>
    /// 是否公开
    /// </summary>
    /// <remarks>
    /// 0-私有，1-公开
    /// </remarks>
    /// <example>0</example>
    public int IsPublic { get; set; }

    /// <summary>
    /// 创建者ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    public Guid CreatedBy { get; set; }

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

    /// <summary>
    /// 组件列表
    /// </summary>
    /// <remarks>
    /// 大屏包含的所有组件
    /// </remarks>
    public List<ScreenComponentDto> Components { get; set; } = new();
}