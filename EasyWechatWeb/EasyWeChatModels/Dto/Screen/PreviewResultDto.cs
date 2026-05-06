namespace EasyWeChatModels.Dto;

/// <summary>
/// 预览结果 DTO
/// </summary>
public class PreviewResultDto
{
    public List<ChartDataDto> ChartData { get; set; } = new();
    public List<Dictionary<string, object>> TableData { get; set; } = new();
    public List<DetectedColumnDto> DetectedColumns { get; set; } = new();
    public ReportSummaryDto Summary { get; set; } = new();
}