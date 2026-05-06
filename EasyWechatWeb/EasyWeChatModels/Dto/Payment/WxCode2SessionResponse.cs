using System.Text.Json.Serialization;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 微信 code2Session 接口响应
/// </summary>
/// <remarks>
/// 调用微信登录 API (jscode2session) 返回的数据结构。
/// 用于获取用户的 OpenId 和 SessionKey。
///
/// API 地址：https://api.weixin.qq.com/sns/jscode2session
/// 参数：appid, secret, js_code, grant_type=authorization_code
/// </remarks>
public class WxCode2SessionResponse
{
    /// <summary>
    /// 用户唯一标识（OpenId）
    /// </summary>
    /// <remarks>
    /// 用户在当前小程序下的唯一标识。
    /// 不同小程序下同一用户的 OpenId 不同。
    /// </remarks>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>
    /// 会话密钥（SessionKey）
    /// </summary>
    /// <remarks>
    /// 用于解密微信小程序 getUserInfo 返回的加密数据。
    /// 有效期较短，不建议持久化存储。
    /// </remarks>
    [JsonPropertyName("session_key")]
    public string? SessionKey { get; set; }

    /// <summary>
    /// 用户在开放平台的唯一标识（UnionId）
    /// </summary>
    /// <remarks>
    /// 需要小程序绑定到微信开放平台后才会返回。
    /// 同一用户在不同小程序下 UnionId 相同。
    /// 用于多小程序用户统一识别。
    /// </remarks>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }

    /// <summary>
    /// 错误码
    /// </summary>
    /// <remarks>
    /// 成功时为 0，失败时为具体错误码。
    /// 常见错误码：
    /// - 40029: code 无效
    /// - 45011: 频率限制
    /// - -1: 系统繁忙
    /// </remarks>
    [JsonPropertyName("errcode")]
    public int ErrCode { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string? ErrMsg { get; set; }
}