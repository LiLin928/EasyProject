using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新报表 DTO
/// </summary>
public class UpdateReportDto
{
    [Required(ErrorMessage = "报表ID不能为空")]
    public Guid Id { get; set; }

    public string? Name { get; set; }
    public string? Category { get; set; }
    public Guid? DatasourceId { get; set; }
    public string? SqlQuery { get; set; }
    public bool? ShowChart { get; set; }
    public bool? ShowTable { get; set; }
    public string? ChartType { get; set; }
    public string? XAxisField { get; set; }
    public string? YAxisField { get; set; }
    public string? Aggregation { get; set; }
    public bool? AutoColumns { get; set; }
    public Guid? ColumnTemplateId { get; set; }
    public List<ColumnConfigDto>? ColumnConfigs { get; set; }
}