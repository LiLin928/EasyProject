namespace EasyWeChatModels.Dto;

/// <summary>
/// 添加分类参数
/// </summary>
public class AddCategoryDto
{
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
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }
}