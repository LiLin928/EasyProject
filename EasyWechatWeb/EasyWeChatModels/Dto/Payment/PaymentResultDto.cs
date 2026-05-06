namespace EasyWeChatModels.Dto;

/// <summary>
/// 支付结果 DTO
/// </summary>
public class PaymentResultDto
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 订单ID
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// 支付ID
    /// </summary>
    public Guid? PaymentId { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 小程序调起支付的参数（微信支付时返回）
    /// </summary>
    /// <remarks>
    /// 当使用真实微信支付时，返回此参数供小程序前端调用 wx.requestPayment。
    /// Mock模式下此参数为空。
    /// </remarks>
    public WxPayParamsDto? WxPayParams { get; set; }
}