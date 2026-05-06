namespace EasyWeChatModels.Dto;

/// <summary>
/// 小程序调起支付参数
/// </summary>
/// <remarks>
/// 小程序前端调用 wx.requestPayment 时需要传入的参数。
/// 由后端调用微信支付预下单接口后生成。
/// </remarks>
public class WxPayParamsDto
{
    /// <summary>
    /// 时间戳（秒）
    /// </summary>
    public string TimeStamp { get; set; } = string.Empty;

    /// <summary>
    /// 随机字符串
    /// </summary>
    public string NonceStr { get; set; } = string.Empty;

    /// <summary>
    /// 订单详情扩展字符串，格式为 prepay_id=xxx
    /// </summary>
    public string Package { get; set; } = string.Empty;

    /// <summary>
    /// 签名类型，固定为 RSA
    /// </summary>
    public string SignType { get; set; } = "RSA";

    /// <summary>
    /// 签名
    /// </summary>
    public string PaySign { get; set; } = string.Empty;
}