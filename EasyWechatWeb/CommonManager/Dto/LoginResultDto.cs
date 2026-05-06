namespace CommonManager.Dto;

/// <summary>
/// 登录结果 DTO，包含用户登录后的 Token 信息
/// </summary>
/// <remarks>
/// 该类定义了登录成功后返回给客户端的 Token 数据结构。
/// 采用双 Token 机制（Access Token + Refresh Token），提高安全性。
///
/// 返回数据说明：
/// - AccessToken：短期有效的访问令牌，用于日常接口认证
/// - RefreshToken：长期有效的刷新令牌，用于获取新的 Access Token
/// - ExpiresIn：Access Token 的有效期（秒），用于客户端计算过期时间
/// - TokenType：令牌类型，固定为 "Bearer"，用于 Authorization 请求头
///
/// 客户端使用方式：
/// - 将 AccessToken 存储在本地（如 localStorage、sessionStorage）
/// - 每次请求 API 时，在 Authorization 头携带 Token
/// - Access Token 过期前使用 Refresh Token 刷新
/// - Refresh Token 过期后需重新登录
/// </remarks>
public class LoginResultDto
{
    /// <summary>
    /// 访问令牌（Access Token），用于 API 接口认证
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 刷新令牌（Refresh Token），用于获取新的 Access Token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// 过期时间（秒），表示 Access Token 的有效持续时间
    /// </summary>
    public long ExpiresIn { get; set; }

    /// <summary>
    /// 令牌类型，固定为 "Bearer"
    /// </summary>
    public string TokenType { get; set; } = "Bearer";
}