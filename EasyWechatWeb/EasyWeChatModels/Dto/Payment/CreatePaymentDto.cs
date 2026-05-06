namespace EasyWeChatModels.Dto;

/// <summary>
/// 创建支付 DTO
/// </summary>
public class CreatePaymentDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// 支付方式：1-微信，2-支付宝，3-余额
    /// </summary>
    public int PaymentMethod { get; set; } = 1;
}