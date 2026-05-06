namespace EasyWeChatModels.Dto;

/// <summary>
/// 查询列模板 DTO
/// </summary>
public class QueryColumnTemplateDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Name { get; set; }
    public string? Type { get; set; }
}