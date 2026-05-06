namespace EasyWeChatModels.Options;

/// <summary>
/// MinIO 配置选项
/// </summary>
/// <remarks>
/// 用于配置 MinIO 对象存储服务的连接参数
/// </remarks>
public class MinioOption
{
    /// <summary>
    /// 服务端点（不含协议前缀）
    /// </summary>
    /// <remarks>
    /// MinIO 服务器地址，不包含 http:// 或 https:// 前缀
    /// </remarks>
    /// <example>localhost:9000</example>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// 访问密钥
    /// </summary>
    /// <remarks>
    /// MinIO 的 Access Key，用于身份验证
    /// </remarks>
    /// <example>minioadmin</example>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>
    /// 秘密密钥
    /// </summary>
    /// <remarks>
    /// MinIO 的 Secret Key，用于身份验证
    /// </remarks>
    /// <example>minioadmin</example>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// 默认桶名
    /// </summary>
    /// <remarks>
    /// 默认的存储桶名称，用于文件上传
    /// </remarks>
    /// <example>easyproject</example>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// 是否使用 SSL
    /// </summary>
    /// <remarks>
    /// 是否使用 HTTPS 协议连接 MinIO 服务，默认为 false
    /// </remarks>
    /// <example>false</example>
    public bool UseSSL { get; set; } = false;
}