namespace EasyWeChatModels.Dto;

/// <summary>
/// 钻取配置 DTO
/// </summary>
public class DrilldownConfigDto
{
    public bool Enabled { get; set; }
    public Guid? TargetReportId { get; set; }
    public string Params { get; set; } = "[]";
}