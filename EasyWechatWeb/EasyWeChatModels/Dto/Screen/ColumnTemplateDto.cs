namespace EasyWeChatModels.Dto;

/// <summary>
/// 列配置模板 DTO
/// </summary>
public class ColumnTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? DatasourceId { get; set; }
    public string? SqlQuery { get; set; }
    public List<ColumnConfigDto> Columns { get; set; } = new();
    public DateTime? CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
}