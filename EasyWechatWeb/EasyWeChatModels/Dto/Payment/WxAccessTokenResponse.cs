using System.Text.Json.Serialization;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 微信 AccessToken 响应
/// </summary>
/// <remarks>
/// 调用获取 AccessToken API 返回的数据结构。
/// AccessToken 用于调用大部分微信后台接口。
///
/// API 地址：https://api.weixin.qq.com/cgi-bin/token
/// 参数：grant_type=client_credential, appid, secret
///
/// 注意：
/// - AccessToken 有效期 2 小时
/// - 建议使用中控服务器统一管理，避免频繁刷新
/// - 每日刷新次数有限制（约 200 次）
/// </remarks>
public class WxAccessTokenResponse
{
    /// <summary>
    /// 获取到的凭证
    /// </summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    /// <summary>
    /// 凭证有效时间（秒）
    /// </summary>
    /// <remarks>
    /// 通常为 7200 秒（2小时）。
    /// </remarks>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// 错误码
    /// </summary>
    [JsonPropertyName("errcode")]
    public int ErrCode { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string? ErrMsg { get; set; }
}