namespace EasyWeChatModels.Dto;

/// <summary>
/// 发布报表数据 DTO（公开访问）
/// </summary>
public class PublishReportDto
{
    /// <summary>
    /// 报表信息
    /// </summary>
    public ReportDto Report { get; set; } = new();

    /// <summary>
    /// 图表数据
    /// </summary>
    public List<ChartDataDto> ChartData { get; set; } = new();

    /// <summary>
    /// 表格数据
    /// </summary>
    public List<Dictionary<string, object>> TableData { get; set; } = new();

    /// <summary>
    /// 列配置
    /// </summary>
    public List<ColumnConfigDto>? ColumnConfigs { get; set; }

    /// <summary>
    /// 检测到的列
    /// </summary>
    public List<DetectedColumnDto> DetectedColumns { get; set; } = new();

    /// <summary>
    /// 数据摘要
    /// </summary>
    public ReportSummaryDto Summary { get; set; } = new();
}