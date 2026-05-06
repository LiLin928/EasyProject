namespace EasyWeChatModels.Dto;

/// <summary>
/// 物流轨迹DTO
/// </summary>
public class ShipTrackDto
{
    /// <summary>
    /// 物流公司
    /// </summary>
    public string Company { get; set; } = string.Empty;

    /// <summary>
    /// 快递单号
    /// </summary>
    public string ShipNo { get; set; } = string.Empty;

    /// <summary>
    /// 物流轨迹列表
    /// </summary>
    public List<ShipTrackItemDto>? Tracks { get; set; }

    /// <summary>
    /// 是否已签收
    /// </summary>
    public bool IsSigned { get; set; }
}