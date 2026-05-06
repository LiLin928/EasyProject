namespace EasyWeChatModels.Dto;

/// <summary>
/// 查询报表 DTO
/// </summary>
public class QueryReportDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Name { get; set; }
    public string? Category { get; set; }
}