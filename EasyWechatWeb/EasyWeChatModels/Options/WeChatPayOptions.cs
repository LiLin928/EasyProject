namespace EasyWeChatModels.Options;

/// <summary>
/// 微信支付配置选项
/// </summary>
public class WeChatPayOptions
{
    /// <summary>
    /// 小程序 AppID
    /// </summary>
    public string AppId { get; set; } = string.Empty;

    /// <summary>
    /// 商户号
    /// </summary>
    public string MchId { get; set; } = string.Empty;

    /// <summary>
    /// 商户证书序列号
    /// </summary>
    public string CertSerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// 商户私钥
    /// </summary>
    public string PrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// APIv3 密钥（32 位字符串）
    /// </summary>
    public string ApiV3Key { get; set; } = string.Empty;

    /// <summary>
    /// 支付回调地址（必须 HTTPS）
    /// </summary>
    public string NotifyUrl { get; set; } = string.Empty;

    /// <summary>
    /// 是否使用 Mock 模式
    /// </summary>
    public bool UseMock { get; set; } = true;
}