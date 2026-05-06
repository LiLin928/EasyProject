namespace EasyWeChatModels.Dto;

/// <summary>
/// 换货物流详情
/// </summary>
public class ExchangeShipDetailDto
{
    /// <summary>
    /// 售后ID
    /// </summary>
    public Guid RefundId { get; set; }

    /// <summary>
    /// 售后编号
    /// </summary>
    public string RefundNo { get; set; } = string.Empty;

    /// <summary>
    /// 快递公司
    /// </summary>
    public string ShipCompany { get; set; } = string.Empty;

    /// <summary>
    /// 快递单号
    /// </summary>
    public string ShipNo { get; set; } = string.Empty;

    /// <summary>
    /// 发货时间
    /// </summary>
    public string? ShipTime { get; set; }

    /// <summary>
    /// 换货商品列表
    /// </summary>
    public List<ExchangeItemDto>? Items { get; set; }

    /// <summary>
    /// 物流轨迹
    /// </summary>
    public List<ShipTrackItemDto>? Tracks { get; set; }
}