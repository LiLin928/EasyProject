using Microsoft.Extensions.Logging;

namespace CommonManager.Helper;

/// <summary>
/// 本地文件存储帮助类
/// </summary>
/// <remarks>
/// 实现本地文件存储，将文件保存到本地磁盘目录。
/// 适合开发环境轻量运行，无需依赖 MinIO 等外部服务。
/// 文件存储路径格式：{RootPath}/file/{year}/{month}/{day}/{uuid}.{ext}
/// </remarks>
public class LocalStorageHelper : IFileStorageHelper
{
    /// <summary>
    /// 存储根目录
    /// </summary>
    private readonly string _rootPath;

    /// <summary>
    /// 公开访问URL前缀
    /// </summary>
    private readonly string _publicUrlPrefix;

    /// <summary>
    /// 后端服务基础URL
    /// </summary>
    private readonly string _baseUrl;

    /// <summary>
    /// 日志记录器
    /// </summary>
    private readonly ILogger<LocalStorageHelper>? _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="rootPath">存储根目录</param>
    /// <param name="publicUrlPrefix">公开访问URL前缀</param>
    /// <param name="baseUrl">后端服务基础URL（如 http://localhost:7600）</param>
    /// <param name="logger">日志记录器（可选）</param>
    public LocalStorageHelper(string rootPath, string publicUrlPrefix, string baseUrl, ILogger<LocalStorageHelper>? logger = null)
    {
        _rootPath = rootPath;
        _publicUrlPrefix = publicUrlPrefix;
        _baseUrl = baseUrl?.TrimEnd('/') ?? "";
        _logger = logger;

        // 确保根目录存在
        EnsureDirectoryExists(_rootPath);
    }

    /// <summary>
    /// 上传文件到本地存储
    /// </summary>
    /// <param name="objectName">对象名称（相对路径），如 "file/2024/01/15/uuid.pdf"</param>
    /// <param name="data">文件数据流</param>
    /// <param name="contentType">内容类型（本地存储不使用，但保留参数兼容接口）</param>
    /// <returns>上传成功返回 true，失败返回 false</returns>
    public async Task<bool> UploadAsync(string objectName, Stream data, string contentType)
    {
        try
        {
            // 计算完整文件路径
            var fullPath = GetFullPath(objectName);

            // 确保目录存在
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory))
            {
                EnsureDirectoryExists(directory);
            }

            // 保存文件到本地
            using var fileStream = File.Create(fullPath);
            await data.CopyToAsync(fileStream);

            _logger?.LogInformation("文件上传成功，路径：{Path}", fullPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "文件上传失败，对象名：{ObjectName}", objectName);
            LogHelper.Error($"上传文件失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 从本地存储下载文件
    /// </summary>
    /// <param name="objectName">对象名称（相对路径）</param>
    /// <returns>文件数据流，文件不存在返回 null</returns>
    public async Task<Stream?> DownloadAsync(string objectName)
    {
        try
        {
            var fullPath = GetFullPath(objectName);

            if (!File.Exists(fullPath))
            {
                _logger?.LogWarning("文件不存在，路径：{Path}", fullPath);
                return null;
            }

            // 读取文件到内存流
            var memoryStream = new MemoryStream();
            using var fileStream = File.OpenRead(fullPath);
            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            _logger?.LogDebug("文件下载成功，路径：{Path}", fullPath);
            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "文件下载失败，对象名：{ObjectName}", objectName);
            LogHelper.Error($"下载文件失败: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 从本地存储删除文件
    /// </summary>
    /// <param name="objectName">对象名称（相对路径）</param>
    /// <returns>删除成功返回 true，失败返回 false</returns>
    public async Task<bool> RemoveAsync(string objectName)
    {
        try
        {
            var fullPath = GetFullPath(objectName);

            if (!File.Exists(fullPath))
            {
                _logger?.LogWarning("文件不存在，无法删除，路径：{Path}", fullPath);
                return true; // 文件不存在也视为删除成功
            }

            // 异步删除文件
            await Task.Run(() => File.Delete(fullPath));

            _logger?.LogInformation("文件删除成功，路径：{Path}", fullPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "文件删除失败，对象名：{ObjectName}", objectName);
            LogHelper.Error($"删除文件失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 获取文件访问URL
    /// </summary>
    /// <param name="objectName">对象名称（相对路径）</param>
    /// <returns>完整访问URL（包含后端服务地址）</returns>
    public string GetFileUrl(string objectName)
    {
        // 返回完整URL，前端通过后端静态文件服务访问
        return $"{_baseUrl}{_publicUrlPrefix}/{objectName}";
    }

    /// <summary>
    /// 获取预签名URL
    /// </summary>
    /// <param name="objectName">对象名称（相对路径）</param>
    /// <param name="expireSeconds">URL有效时间（本地存储忽略此参数）</param>
    /// <returns>公开访问URL（本地存储无签名，直接返回公开URL）</returns>
    public async Task<string?> GetPresignedUrlAsync(string objectName, int expireSeconds = 3600)
    {
        // 本地存储不支持预签名，直接返回公开访问URL
        // expireSeconds 参数被忽略
        await Task.CompletedTask;
        return GetFileUrl(objectName);
    }

    /// <summary>
    /// 检查文件是否存在
    /// </summary>
    /// <param name="objectName">对象名称（相对路径）</param>
    /// <returns>存在返回 true，不存在返回 false</returns>
    public async Task<bool> FileExistsAsync(string objectName)
    {
        var fullPath = GetFullPath(objectName);
        return await Task.Run(() => File.Exists(fullPath));
    }

    /// <summary>
    /// 获取文件完整路径
    /// </summary>
    /// <param name="objectName">对象名称（相对路径）</param>
    /// <returns>完整文件系统路径</returns>
    private string GetFullPath(string objectName)
    {
        // 移除前导斜杠，避免路径拼接问题
        var cleanObjectName = objectName.StartsWith("/")
            ? objectName.Substring(1)
            : objectName;

        return Path.Combine(_rootPath, cleanObjectName);
    }

    /// <summary>
    /// 确保目录存在
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    private void EnsureDirectoryExists(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            _logger?.LogDebug("创建目录：{Path}", directoryPath);
        }
    }

    /// <summary>
    /// 根据 MinIO 配置创建本地存储实例（兼容工厂模式）
    /// </summary>
    /// <param name="endpoint">端点（本地存储使用作为 BaseUrl）</param>
    /// <param name="accessKey">访问密钥（本地存储忽略）</param>
    /// <param name="secretKey">秘密密钥（本地存储忽略）</param>
    /// <param name="bucketName">桶名称（本地存储作为根目录子目录）</param>
    /// <param name="useSSL">是否使用SSL（本地存储用于构建 BaseUrl）</param>
    /// <returns>LocalStorageHelper 实例</returns>
    public static LocalStorageHelper CreateFromMinioConfig(
        string endpoint,
        string accessKey,
        string secretKey,
        string bucketName,
        bool useSSL = false)
    {
        // 将 bucketName 作为子目录
        var rootPath = Path.Combine("uploads", bucketName);
        var publicUrlPrefix = $"/uploads/{bucketName}";
        // 使用 MinIO endpoint 作为 BaseUrl（fallback 场景）
        var baseUrl = useSSL ? $"https://{endpoint}" : $"http://{endpoint}";
        return new LocalStorageHelper(rootPath, publicUrlPrefix, baseUrl);
    }
}