namespace EasyWeChatModels.Dto;

/// <summary>
/// 订单发货参数
/// </summary>
public class ShipDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// 物流公司
    /// </summary>
    public string LogisticsCompany { get; set; } = string.Empty;

    /// <summary>
    /// 物流单号
    /// </summary>
    public string LogisticsNumber { get; set; } = string.Empty;
}