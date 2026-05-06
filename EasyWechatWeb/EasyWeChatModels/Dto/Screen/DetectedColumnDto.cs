namespace EasyWeChatModels.Dto;

/// <summary>
/// 检测到的列信息 DTO
/// </summary>
public class DetectedColumnDto
{
    public string Field { get; set; } = string.Empty;
    public string Type { get; set; } = "string";
}