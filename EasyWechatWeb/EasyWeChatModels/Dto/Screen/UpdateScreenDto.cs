using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新大屏请求 DTO
/// </summary>
/// <remarks>
/// 用于更新大屏基本信息，所有字段均为可选
/// </remarks>
public class UpdateScreenDto
{
    /// <summary>
    /// 大屏ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    [Required(ErrorMessage = "大屏ID不能为空")]
    public Guid Id { get; set; }

    /// <summary>
    /// 大屏名称
    /// </summary>
    /// <example>销售数据大屏</example>
    public string? Name { get; set; }

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
    public string? Style { get; set; }

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
    public int? IsPublic { get; set; }

    /// <summary>
    /// 组件列表（JSON格式）
    /// </summary>
    /// <remarks>
    /// 包含大屏所有组件的配置信息
    /// </remarks>
    public List<ScreenComponentDto>? Components { get; set; }
}