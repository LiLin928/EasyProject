namespace EasyWeChatModels.Dto;

/// <summary>
/// 销量趋势DTO
/// </summary>
public class SalesTrendDto
{
    /// <summary>
    /// 日期
    /// </summary>
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// 销售数量
    /// </summary>
    public int SalesCount { get; set; }

    /// <summary>
    /// 销售金额
    /// </summary>
    public decimal SalesAmount { get; set; }

    /// <summary>
    /// 订单数量
    /// </summary>
    public int OrderCount { get; set; }
}