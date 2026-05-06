namespace EasyWeChatModels.Dto;

/// <summary>
/// 分类 DTO
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// 分类ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 父分类ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; } = string.Empty;

    /// <summary>
    /// 子分类列表
    /// </summary>
    public List<CategoryDto>? Children { get; set; }
}