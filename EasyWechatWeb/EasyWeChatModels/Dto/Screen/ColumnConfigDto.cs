namespace EasyWeChatModels.Dto;

/// <summary>
/// 列配置项 DTO
/// </summary>
public class ColumnConfigDto
{
    public string Field { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int Width { get; set; } = 100;
    public string Align { get; set; } = "left";
    public string? Format { get; set; }
    public DrilldownConfigDto? Drilldown { get; set; }
}