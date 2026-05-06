using Minio;
using Minio.DataModel.Args;

namespace CommonManager.Helper;

/// <summary>
/// MinIO 文件存储帮助类，提供对象存储的常用操作方法
/// </summary>
/// <remarks>
/// 实现 IFileStorageHelper 接口，支持与其他存储方式切换。
/// MinIO 是一个高性能的分布式对象存储服务，兼容 Amazon S3 API。
/// </remarks>
public class MinioHelper : IFileStorageHelper
{
    /// <summary>
    /// MinIO 客户端实例
    /// </summary>
    private readonly IMinioClient _minioClient;

    /// <summary>
    /// 默认桶名称
    /// </summary>
    private readonly string _defaultBucketName;

    /// <summary>
    /// 构造函数，初始化 MinIO 客户端
    /// </summary>
    /// <param name="endpoint">MinIO 服务地址，如 "localhost:9000" 或 "minio.example.com"</param>
    /// <param name="accessKey">访问密钥（Access Key）</param>
    /// <param name="secretKey">安全密钥（Secret Key）</param>
    /// <param name="bucketName">默认桶名称，用于不指定桶名时的操作</param>
    /// <param name="useSSL">是否使用 SSL，默认为 false。生产环境建议启用</param>
    /// <remarks>
    /// MinIO 客户端创建后可重复使用，建议通过依赖注入管理生命周期。
    /// 桶名称需符合 S3 规范：仅小写字母、数字、点和短横线，3-63 字符。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 本地开发环境
    /// var minio = new MinioHelper("localhost:9000", "minioadmin", "minioadmin", "dev-bucket");
    ///
    /// // 生产环境（启用 SSL）
    /// var minioProd = new MinioHelper(
    ///     "minio.company.com",
    ///     "access-key-id",
    ///     "secret-access-key",
    ///     "prod-bucket",
    ///     useSSL: true
    /// );
    /// </code>
    /// </example>
    public MinioHelper(string endpoint, string accessKey, string secretKey, string bucketName, bool useSSL = false)
    {
        _defaultBucketName = bucketName;

        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(useSSL)
            .Build();
    }

    /// <summary>
    /// 设置存储桶为公开读取
    /// </summary>
    /// <param name="bucketName">桶名称</param>
    /// <returns>设置成功返回 true，失败返回 false</returns>
    /// <remarks>
    /// 公开策略允许任何人直接访问桶中的文件，无需签名。
    /// 适用于图片、静态资源等需要公开访问的文件。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 设置桶为公开读取
    /// await minio.SetBucketPublicAsync("public-images");
    /// // 之后可以直接通过 URL 访问文件
    /// // http://minio.example.com/public-images/avatar.jpg
    /// </code>
    /// </example>
    public async Task<bool> SetBucketPublicAsync(string bucketName)
    {
        try
        {
            // 公开读取策略 JSON（AWS S3 格式）
            var policyJson = $@" {{
                ""Version"": ""2012-10-17"",
                ""Statement"": [
                    {{
                        ""Effect"": ""Allow"",
                        ""Principal"": {{""AWS"": ""*""}},
                        ""Action"": [""s3:GetObject""],
                        ""Resource"": [""arn:aws:s3:::{bucketName}/*""]
                    }}
                ]
            }}";

            var args = new SetPolicyArgs()
                .WithBucket(bucketName)
                .WithPolicy(policyJson);

            await _minioClient.SetPolicyAsync(args);
            LogHelper.Info($"存储桶 {bucketName} 已设置为公开读取");
            return true;
        }
        catch (Exception ex)
        {
            LogHelper.Error($"设置存储桶公开策略失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 检查桶是否存在
    /// </summary>
    /// <param name="bucketName">桶名称</param>
    /// <returns>存在返回 true，不存在返回 false</returns>
    /// <remarks>
    /// 在上传文件前建议先检查桶是否存在，不存在则创建。
    /// </remarks>
    /// <example>
    /// <code>
    /// if (!await minio.BucketExistsAsync("my-bucket"))
    /// {
    ///     await minio.MakeBucketAsync("my-bucket");
    /// }
    /// </code>
    /// </example>
    public async Task<bool> BucketExistsAsync(string bucketName)
    {
        var args = new BucketExistsArgs()
            .WithBucket(bucketName);
        return await _minioClient.BucketExistsAsync(args);
    }

    /// <summary>
    /// 创建新桶
    /// </summary>
    /// <param name="bucketName">要创建的桶名称</param>
    /// <returns>创建成功返回 true，失败返回 false</returns>
    /// <remarks>
    /// 桶创建后会记录日志。桶名称需符合 S3 规范。
    /// 创建失败时会通过 LogHelper 记录错误日志。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 创建用户图片桶
    /// await minio.MakeBucketAsync("user-images");
    ///
    /// // 创建公共资源桶
    /// await minio.MakeBucketAsync("public-resources");
    /// </code>
    /// </example>
    public async Task<bool> MakeBucketAsync(string bucketName)
    {
        try
        {
            var args = new MakeBucketArgs()
                .WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(args);
            return true;
        }
        catch (Exception ex)
        {
            LogHelper.Error($"创建桶失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 上传文件到默认桶（使用流数据）
    /// </summary>
    /// <param name="objectName">对象名称（文件路径），如 "images/avatar.jpg"</param>
    /// <param name="data">文件数据流</param>
    /// <param name="contentType">内容类型（MIME），默认为 "application/octet-stream"</param>
    /// <returns>上传成功返回 true，失败返回 false</returns>
    /// <remarks>
    /// 使用默认桶名称上传文件。如果桶不存在会自动创建。
    /// 内容类型影响浏览器如何处理文件，建议正确设置。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 上传图片
    /// using var imageStream = File.OpenRead("avatar.jpg");
    /// await minio.UploadAsync("images/avatar.jpg", imageStream, "image/jpeg");
    ///
    /// // 上传 PDF
    /// using var pdfStream = File.OpenRead("document.pdf");
    /// await minio.UploadAsync("docs/report.pdf", pdfStream, "application/pdf");
    /// </code>
    /// </example>
    public async Task<bool> UploadAsync(string objectName, Stream data, string contentType = "application/octet-stream")
    {
        return await UploadAsync(_defaultBucketName, objectName, data, contentType);
    }

    /// <summary>
    /// 上传文件到指定桶（使用流数据）
    /// </summary>
    /// <param name="bucketName">目标桶名称</param>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <param name="data">文件数据流</param>
    /// <param name="contentType">内容类型（MIME）</param>
    /// <returns>上传成功返回 true，失败返回 false</returns>
    /// <remarks>
    /// 如果桶不存在会自动创建。失败时记录错误日志。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 上传到指定桶
    /// await minio.UploadAsync("user-images", "user123/avatar.jpg", imageStream, "image/jpeg");
    /// </code>
    /// </example>
    public async Task<bool> UploadAsync(string bucketName, string objectName, Stream data, string contentType = "application/octet-stream")
    {
        try
        {
            // 确保桶存在
            if (!await BucketExistsAsync(bucketName))
            {
                await MakeBucketAsync(bucketName);
            }

            var args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(data)
                .WithObjectSize(data.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(args);
            return true;
        }
        catch (Exception ex)
        {
            LogHelper.Error($"上传文件失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 上传本地文件到默认桶
    /// </summary>
    /// <param name="filePath">本地文件完整路径</param>
    /// <param name="objectName">对象名称（存储路径）</param>
    /// <returns>上传成功返回 true，失败返回 false</returns>
    /// <remarks>
    /// 自动根据文件扩展名识别内容类型。
    /// 适合批量上传本地文件。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 上传本地图片
    /// await minio.UploadFileAsync("C:\\temp\\photo.jpg", "photos/2024/photo.jpg");
    ///
    /// // 上传本地文档
    /// await minio.UploadFileAsync("C:\\docs\\report.pdf", "reports/2024-q1.pdf");
    /// </code>
    /// </example>
    public async Task<bool> UploadFileAsync(string filePath, string objectName)
    {
        try
        {
            using var fileStream = File.OpenRead(filePath);
            var contentType = GetContentType(Path.GetExtension(filePath));
            return await UploadAsync(objectName, fileStream, contentType);
        }
        catch (Exception ex)
        {
            LogHelper.Error($"上传本地文件失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 从默认桶下载文件到流
    /// </summary>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <returns>文件数据流，失败返回 null</returns>
    /// <remarks>
    /// 返回的 MemoryStream 需调用方手动释放。
    /// </remarks>
    /// <example>
    /// <code>
    /// var stream = await minio.DownloadAsync("images/avatar.jpg");
    /// if (stream != null)
    /// {
    ///     using var imageStream = stream;
    ///     var image = Image.FromStream(imageStream);
    /// }
    /// </code>
    /// </example>
    public async Task<Stream?> DownloadAsync(string objectName)
    {
        return await DownloadAsync(_defaultBucketName, objectName);
    }

    /// <summary>
    /// 从指定桶下载文件到流
    /// </summary>
    /// <param name="bucketName">源桶名称</param>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <returns>文件数据流，失败返回 null</returns>
    /// <remarks>
    /// 返回 MemoryStream，调用方需手动释放。
    /// 失败时记录错误日志。
    /// </remarks>
    /// <example>
    /// <code>
    /// var stream = await minio.DownloadAsync("user-images", "user123/avatar.jpg");
    /// </code>
    /// </example>
    public async Task<Stream?> DownloadAsync(string bucketName, string objectName)
    {
        try
        {
            var memoryStream = new MemoryStream();
            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream => stream.CopyTo(memoryStream));

            await _minioClient.GetObjectAsync(args);
            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (Exception ex)
        {
            LogHelper.Error($"下载文件失败: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 从默认桶下载文件到本地路径
    /// </summary>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <param name="savePath">本地保存路径</param>
    /// <returns>下载成功返回 true，失败返回 false</returns>
    /// <remarks>
    /// 会自动创建目标目录（如果不存在）。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 下载文件到本地
    /// await minio.DownloadFileAsync("reports/2024-q1.pdf", "C:\\downloads\\report.pdf");
    /// </code>
    /// </example>
    public async Task<bool> DownloadFileAsync(string objectName, string savePath)
    {
        return await DownloadFileAsync(_defaultBucketName, objectName, savePath);
    }

    /// <summary>
    /// 从指定桶下载文件到本地路径
    /// </summary>
    /// <param name="bucketName">源桶名称</param>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <param name="savePath">本地保存路径</param>
    /// <returns>下载成功返回 true，失败返回 false</returns>
    /// <remarks>
    /// 自动创建目标目录，失败时记录错误日志。
    /// </remarks>
    /// <example>
    /// <code>
    /// await minio.DownloadFileAsync("user-images", "user123/avatar.jpg", "C:\\temp\\avatar.jpg");
    /// </code>
    /// </example>
    public async Task<bool> DownloadFileAsync(string bucketName, string objectName, string savePath)
    {
        try
        {
            // 确保目录存在
            var directory = Path.GetDirectoryName(savePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var fileStream = File.Create(savePath);
            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream => stream.CopyTo(fileStream));

            await _minioClient.GetObjectAsync(args);
            return true;
        }
        catch (Exception ex)
        {
            LogHelper.Error($"下载文件到本地失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 从默认桶删除文件
    /// </summary>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <returns>删除成功返回 true，失败返回 false</returns>
    /// <remarks>
    /// 文件不存在时操作也会成功。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 删除单个文件
    /// await minio.RemoveAsync("images/avatar.jpg");
    /// </code>
    /// </example>
    public async Task<bool> RemoveAsync(string objectName)
    {
        return await RemoveAsync(_defaultBucketName, objectName);
    }

    /// <summary>
    /// 从指定桶删除文件
    /// </summary>
    /// <param name="bucketName">源桶名称</param>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <returns>删除成功返回 true，失败返回 false</returns>
    /// <remarks>
    /// 删除指定桶中的文件对象，失败时记录错误日志。
    /// </remarks>
    /// <example>
    /// <code>
    /// await minio.RemoveAsync("user-images", "user123/avatar.jpg");
    /// </code>
    /// </example>
    public async Task<bool> RemoveAsync(string bucketName, string objectName)
    {
        try
        {
            var args = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);

            await _minioClient.RemoveObjectAsync(args);
            return true;
        }
        catch (Exception ex)
        {
            LogHelper.Error($"删除文件失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 获取默认桶中文件的预签名 URL（用于临时访问）
    /// </summary>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <param name="expireSeconds">URL 有效时间（秒），默认为 3600（1小时）</param>
    /// <returns>预签名 URL，失败返回 null</returns>
    /// <remarks>
    /// 预签名 URL 允许无需认证即可在有效期内访问文件。
    /// 常用于分享文件链接、前端直接上传/下载等场景。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 获取 1 小时有效期的下载链接
    /// var url = await minio.GetPresignedUrlAsync("reports/report.pdf");
    /// Console.WriteLine($"下载链接: {url}");
    ///
    /// // 获取 5 分钟有效期的临时链接
    /// var tempUrl = await minio.GetPresignedUrlAsync("images/photo.jpg", 300);
    /// </code>
    /// </example>
    public async Task<string?> GetPresignedUrlAsync(string objectName, int expireSeconds = 3600)
    {
        return await GetPresignedUrlAsync(_defaultBucketName, objectName, expireSeconds);
    }

    /// <summary>
    /// 获取指定桶中文件的预签名 URL
    /// </summary>
    /// <param name="bucketName">桶名称</param>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <param name="expireSeconds">URL 有效时间（秒）</param>
    /// <returns>预签名 URL，失败返回 null</returns>
    /// <remarks>
    /// 预签名 URL 包含签名信息，过期后失效。
    /// 失败时记录错误日志。
    /// </remarks>
    /// <example>
    /// <code>
    /// var url = await minio.GetPresignedUrlAsync("user-images", "user123/avatar.jpg", 7200);
    /// </code>
    /// </example>
    public async Task<string?> GetPresignedUrlAsync(string bucketName, string objectName, int expireSeconds = 3600)
    {
        try
        {
            var args = new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithExpiry(expireSeconds);

            return await _minioClient.PresignedGetObjectAsync(args);
        }
        catch (Exception ex)
        {
            LogHelper.Error($"获取预签名URL失败: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 获取默认桶中文件的直接访问 URL
    /// </summary>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <returns>直接访问 URL（不含签名）</returns>
    /// <remarks>
    /// 直接访问 URL 不含签名信息，仅适用于公开桶。
    /// 对于私有桶，请使用 GetPresignedUrlAsync 方法。
    /// </remarks>
    /// <example>
    /// <code>
    /// var url = minio.GetFileUrl("public/logo.png");
    /// // 输出: http://localhost:9000/my-bucket/public/logo.png
    /// </code>
    /// </example>
    public string GetFileUrl(string objectName)
    {
        return GetFileUrl(_defaultBucketName, objectName);
    }

    /// <summary>
    /// 获取指定桶中文件的直接访问 URL
    /// </summary>
    /// <param name="bucketName">桶名称</param>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <returns>直接访问 URL</returns>
    /// <remarks>
    /// 返回标准的 MinIO/S3 URL 格式。
    /// </remarks>
    /// <example>
    /// <code>
    /// var url = minio.GetFileUrl("public-resources", "images/banner.jpg");
    /// // 输出: http://minio.example.com/public-resources/images/banner.jpg
    /// </code>
    /// </example>
    public string GetFileUrl(string bucketName, string objectName)
    {
        // 返回直接访问的URL格式
        // _minioClient.Config.Endpoint 可能已经包含协议前缀，需要正确处理
        var endpoint = _minioClient.Config.Endpoint;
        var protocol = _minioClient.Config.Secure ? "https" : "http";

        // 如果 endpoint 已经包含协议前缀，直接使用；否则添加协议
        if (endpoint.StartsWith("http://") || endpoint.StartsWith("https://"))
        {
            return $"{endpoint}/{bucketName}/{objectName}";
        }
        else
        {
            return $"{protocol}://{endpoint}/{bucketName}/{objectName}";
        }
    }

    /// <summary>
    /// 列出默认桶中的所有文件
    /// </summary>
    /// <param name="prefix">前缀过滤，如 "images/" 仅列出 images 目录下的文件</param>
    /// <returns>文件对象名称列表</returns>
    /// <remarks>
    /// 使用前缀可以限制列出特定目录下的文件。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 列出所有文件
    /// var allFiles = await minio.ListObjectsAsync();
    ///
    /// // 列出 images 目录下的文件
    /// var imageFiles = await minio.ListObjectsAsync("images/");
    /// </code>
    /// </example>
    public async Task<List<string>> ListObjectsAsync(string? prefix = null)
    {
        return await ListObjectsAsync(_defaultBucketName, prefix);
    }

    /// <summary>
    /// 列出指定桶中的所有文件
    /// </summary>
    /// <param name="bucketName">桶名称</param>
    /// <param name="prefix">前缀过滤</param>
    /// <returns>文件对象名称列表</returns>
    /// <remarks>
    /// 递归列出所有匹配前缀的文件对象。
    /// 失败时返回空列表并记录错误日志。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 列出 user-images 桶中 user123 目录下的所有文件
    /// var files = await minio.ListObjectsAsync("user-images", "user123/");
    /// foreach (var file in files)
    /// {
    ///     Console.WriteLine(file);
    /// }
    /// </code>
    /// </example>
    public async Task<List<string>> ListObjectsAsync(string bucketName, string? prefix = null)
    {
        var result = new List<string>();
        try
        {
            var args = new ListObjectsArgs()
                .WithBucket(bucketName)
                .WithPrefix(prefix ?? "")
                .WithRecursive(true);

            await foreach (var item in _minioClient.ListObjectsEnumAsync(args))
            {
                result.Add(item.Key);
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error($"列出文件失败: {ex.Message}");
        }
        return result;
    }

    /// <summary>
    /// 检查文件是否存在
    /// </summary>
    /// <param name="objectName">对象名称（文件路径）</param>
    /// <returns>存在返回 true，不存在返回 false</returns>
    public async Task<bool> FileExistsAsync(string objectName)
    {
        try
        {
            var args = new StatObjectArgs()
                .WithBucket(_defaultBucketName)
                .WithObject(objectName);

            await _minioClient.StatObjectAsync(args);
            return true;
        }
        catch (Exception ex)
        {
            LogHelper.Error($"检查文件存在失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 根据文件扩展名获取对应的 MIME Content-Type
    /// </summary>
    /// <param name="extension">文件扩展名（含点号），如 ".jpg"、".pdf"</param>
    /// <returns>对应的 MIME Content-Type</returns>
    /// <remarks>
    /// 支持常见的图片、文档、音视频等文件类型。
    /// 未识别的扩展名返回 "application/octet-stream"（通用二进制流）。
    /// </remarks>
    /// <example>
    /// <code>
    /// var contentType = GetContentType(".jpg");  // 返回 "image/jpeg"
    /// var pdfType = GetContentType(".pdf");  // 返回 "application/pdf"
    /// var unknownType = GetContentType(".xyz");  // 返回 "application/octet-stream"
    /// </code>
    /// </example>
    private static string GetContentType(string extension)
    {
        return extension.ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".txt" => "text/plain",
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            ".7z" => "application/x-7z-compressed",
            ".mp3" => "audio/mpeg",
            ".mp4" => "video/mp4",
            ".avi" => "video/x-msvideo",
            ".mov" => "video/quicktime",
            _ => "application/octet-stream"
        };
    }
}