using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 创建大屏请求 DTO
/// </summary>
/// <remarks>
/// 用于创建新的大屏
/// </remarks>
public class CreateScreenDto
{
    /// <summary>
    /// 大屏名称
    /// </summary>
    /// <example>销售数据大屏</example>
    [Required(ErrorMessage = "大屏名称不能为空")]
    [MaxLength(100, ErrorMessage = "大屏名称长度不能超过100个字符")]
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
    public int IsPublic { get; set; } = 0;
}