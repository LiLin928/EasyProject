namespace CommonManager.Helper;

/// <summary>
/// 文件存储帮助类接口
/// </summary>
/// <remarks>
/// 定义文件存储的通用操作接口，支持 MinIO 对象存储和本地文件存储两种实现。
/// 通过依赖注入和配置切换，业务代码无需关心具体存储实现。
/// </remarks>
public interface IFileStorageHelper
{
    /// <summary>
    /// 上传文件到存储
    /// </summary>
    /// <param name="objectName">对象名称（文件路径），如 "file/2024/01/15/uuid.pdf"</param>
    /// <param name="data">文件数据流</param>
    /// <param name="contentType">内容类型（MIME）</param>
    /// <returns>上传成功返回 true，失败返回 false</returns>
    Task<bool> UploadAsync(string objectName, Stream data, string contentType);

    /// <summary>
    /// 从存储下载文件
    /// </summary>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <returns>文件数据流，失败返回 null</returns>
    Task<Stream?> DownloadAsync(string objectName);

    /// <summary>
    /// 从存储删除文件
    /// </summary>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <returns>删除成功返回 true，失败返回 false</returns>
    Task<bool> RemoveAsync(string objectName);

    /// <summary>
    /// 获取文件访问URL
    /// </summary>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <returns>文件访问URL</returns>
    string GetFileUrl(string objectName);

    /// <summary>
    /// 获取预签名URL（临时访问链接）
    /// </summary>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <param name="expireSeconds">URL有效时间（秒），本地存储忽略此参数</param>
    /// <returns>预签名URL，失败返回 null</returns>
    /// <remarks>
    /// MinIO 实现：返回带签名的临时访问URL
    /// 本地存储实现：直接返回公开访问URL（无过期限制）
    /// </remarks>
    Task<string?> GetPresignedUrlAsync(string objectName, int expireSeconds = 3600);

    /// <summary>
    /// 检查文件是否存在
    /// </summary>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <returns>存在返回 true，不存在返回 false</returns>
    Task<bool> FileExistsAsync(string objectName);
}