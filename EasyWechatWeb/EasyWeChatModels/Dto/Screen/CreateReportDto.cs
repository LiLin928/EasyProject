using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 创建报表 DTO
/// </summary>
public class CreateReportDto
{
    [Required(ErrorMessage = "报表名称不能为空")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "分类不能为空")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "数据源不能为空")]
    public Guid DatasourceId { get; set; }

    [Required(ErrorMessage = "SQL查询不能为空")]
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
}