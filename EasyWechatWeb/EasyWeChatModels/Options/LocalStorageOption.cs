namespace EasyWeChatModels.Options;

/// <summary>
/// 本地文件存储配置选项
/// </summary>
/// <remarks>
/// 用于配置本地文件存储的相关参数，支持与 MinIO 存储切换。
/// 当 StorageType 设置为 "local" 时，系统使用本地文件存储。
/// </remarks>
public class LocalStorageOption
{
    /// <summary>
    /// 存储类型
    /// </summary>
    /// <remarks>
    /// 可选值：local（本地存储）、minio（MinIO对象存储）
    /// 默认值为 local，适合开发环境轻量运行。
    /// </remarks>
    /// <example>local</example>
    public string StorageType { get; set; } = "local";

    /// <summary>
    /// 本地存储根目录
    /// </summary>
    /// <remarks>
    /// 文件存储的根目录路径，可以是相对路径或绝对路径。
    /// 相对路径相对于应用程序根目录。
    /// 默认值为 "uploads"。
    /// </remarks>
    /// <example>uploads</example>
    public string RootPath { get; set; } = "uploads";

    /// <summary>
    /// 公开访问URL前缀
    /// </summary>
    /// <remarks>
    /// 文件公开访问的URL路径前缀，用于静态文件服务配置。
    /// 默认值为 "/uploads"，对应 RootPath 目录。
    /// </remarks>
    /// <example>/uploads</example>
    public string PublicUrlPrefix { get; set; } = "/uploads";

    /// <summary>
    /// 最大文件大小（字节）
    /// </summary>
    /// <remarks>
    /// 单个文件上传的最大大小限制，默认为 50MB。
    /// 超过此大小的文件将被拒绝上传。
    /// </remarks>
    /// <example>52428800</example>
    public long MaxFileSize { get; set; } = 50 * 1024 * 1024;

    /// <summary>
    /// 后端服务基础URL
    /// </summary>
    /// <remarks>
    /// 用于生成完整的文件访问URL，前端需要通过此URL访问静态文件。
    /// 例如：http://localhost:7600 或 https://api.yourdomain.com
    /// </remarks>
    /// <example>http://localhost:7600</example>
    public string BaseUrl { get; set; } = "http://localhost:7600";
}