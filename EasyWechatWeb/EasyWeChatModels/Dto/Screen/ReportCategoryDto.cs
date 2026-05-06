namespace EasyWeChatModels.Dto;

/// <summary>
/// 报表分类 DTO
/// </summary>
public class ReportCategoryDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
}