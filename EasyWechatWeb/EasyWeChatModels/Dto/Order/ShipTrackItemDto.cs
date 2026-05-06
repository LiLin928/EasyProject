namespace EasyWeChatModels.Dto;

/// <summary>
/// 物流轨迹项DTO
/// </summary>
public class ShipTrackItemDto
{
    /// <summary>
    /// 时间
    /// </summary>
    public string Time { get; set; } = string.Empty;

    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 地点
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
}