using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 预览报表 DTO
/// </summary>
public class PreviewReportDto
{
    [Required(ErrorMessage = "数据源不能为空")]
    public Guid DatasourceId { get; set; }

    [Required(ErrorMessage = "SQL查询不能为空")]
    public string SqlQuery { get; set; } = string.Empty;

    public string ChartType { get; set; } = "bar";
    public string? XAxisField { get; set; }
    public string? YAxisField { get; set; }
    public string Aggregation { get; set; } = "sum";
}