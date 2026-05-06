namespace EasyWeChatModels.Dto;

/// <summary>
/// 报表信息 DTO
/// </summary>
public class ReportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Guid DatasourceId { get; set; }
    public string? DatasourceName { get; set; }
    public string SqlQuery { get; set; } = string.Empty;
    public bool ShowChart { get; set; } = true;
    public bool ShowTable { get; set; } = true;
    public string ChartType { get; set; } = "bar";
    public string? XAxisField { get; set; }
    public string? YAxisField { get; set; }
    public string Aggregation { get; set; } = "sum";
    public bool AutoColumns { get; set; } = true;
    public Guid? ColumnTemplateId { get; set; }
    public List<ColumnConfigDto>? ColumnConfigs { get; set; }
    public string? Creator { get; set; }
    public DateTime? CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }
}