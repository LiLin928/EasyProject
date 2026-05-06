namespace EasyWeChatModels.Dto;

/// <summary>
/// 创建售后申请参数（后台代客申请）
/// </summary>
public class CreateRefundDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// 售后类型（refund退款/exchange换货）
    /// </summary>
    public string Type { get; set; } = "refund";

    /// <summary>
    /// 售后商品列表
    /// </summary>
    public List<CreateRefundItemDto> Items { get; set; } = new();

    /// <summary>
    /// 退款金额（退款时必填）
    /// </summary>
    public decimal? RefundAmount { get; set; }

    /// <summary>
    /// 退款原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// 问题描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 换货商品列表（换货时必填）
    /// </summary>
    public List<ExchangeItemDto>? ExchangeItems { get; set; }
}