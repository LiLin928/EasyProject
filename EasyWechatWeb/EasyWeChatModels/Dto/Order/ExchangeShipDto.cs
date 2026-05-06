namespace EasyWeChatModels.Dto;

/// <summary>
/// 换货发货参数
/// </summary>
public class ExchangeShipDto
{
    /// <summary>
    /// 售后ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 快递公司
    /// </summary>
    public string ShipCompany { get; set; } = string.Empty;

    /// <summary>
    /// 快递单号
    /// </summary>
    public string ShipNo { get; set; } = string.Empty;
}