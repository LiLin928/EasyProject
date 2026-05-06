using System.Text.Json.Serialization;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 微信手机号解密结果 DTO
/// </summary>
public class WxPhoneInfoDto
{
    /// <summary>
    /// 完整手机号（带国家码，如 +8613800138000）
    /// </summary>
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// 纯数字手机号（如 13800138000）
    /// </summary>
    [JsonPropertyName("purePhoneNumber")]
    public string PurePhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// 国家码（如 86）
    /// </summary>
    [JsonPropertyName("countryCode")]
    public string CountryCode { get; set; } = "86";

    /// <summary>
    /// 水印信息（微信返回）
    /// </summary>
    [JsonPropertyName("watermark")]
    public WxWatermark? Watermark { get; set; }
}

/// <summary>
/// 微信数据水印
/// </summary>
public class WxWatermark
{
    /// <summary>
    /// 小程序 AppId
    /// </summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;

    /// <summary>
    /// 时间戳
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }
}