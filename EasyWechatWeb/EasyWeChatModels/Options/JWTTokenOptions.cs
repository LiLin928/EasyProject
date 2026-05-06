namespace EasyWeChatModels.Options;

/// <summary>
/// JWT Token 配置选项，用于配置 JWT Token 的生成和验证
/// </summary>
public class JWTTokenOptions
{
    /// <summary>
    /// 受众（Audience），Token 的目标接收方
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// 发行者（Issuer），Token 的签发方
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// 安全密钥（Security Key），用于 Token 签名和验证（至少 32 个字符）
    /// </summary>
    public string SecurityKey { get; set; } = string.Empty;

    /// <summary>
    /// Access Token 过期时间（分钟）
    /// </summary>
    public int AccessTokenExpiration { get; set; } = 60;

    /// <summary>
    /// Refresh Token 过期时间（分钟）
    /// </summary>
    public int RefreshTokenExpiration { get; set; } = 10080; // 7天
}