namespace EasyWeChatModels.Dto;

/// <summary>
/// 文件上传结果 DTO
/// </summary>
/// <remarks>
/// 用于返回文件上传操作的结果信息。
/// 包含上传成功后的文件ID、访问URL等。
/// </remarks>
public class FileUploadResultDto
{
    /// <summary>
    /// 文件记录ID
    /// </summary>
    /// <remarks>
    /// 上传成功后生成的文件记录ID，用于后续操作。
    /// </remarks>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 原始文件名
    /// </summary>
    /// <remarks>
    /// 用户上传时的原始文件名。
    /// </remarks>
    /// <example>report.pdf</example>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    /// <example>1024000</example>
    public long FileSize { get; set; }

    /// <summary>
    /// 文件扩展名
    /// </summary>
    /// <example>pdf</example>
    public string FileExt { get; set; } = string.Empty;

    /// <summary>
    /// 内容类型（MIME类型）
    /// </summary>
    /// <example>application/pdf</example>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// 文件访问URL
    /// </summary>
    /// <remarks>
    /// 文件的访问URL，可用于下载或预览。
    /// </remarks>
    /// <example>http://localhost:9000/easyproject/document/2024/01/uuid.pdf</example>
    public string? Url { get; set; }

    /// <summary>
    /// 预签名URL
    /// </summary>
    /// <remarks>
    /// 文件的预签名访问URL，有过期时间限制。
    /// 适用于私有桶的临时访问。
    /// </remarks>
    /// <example>http://localhost:9000/easyproject/document/2024/01/uuid.pdf?X-Amz-...</example>
    public string? PresignedUrl { get; set; }

    /// <summary>
    /// 上传是否成功
    /// </summary>
    /// <remarks>
    /// 标识文件上传操作是否成功完成。
    /// </remarks>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    /// <remarks>
    /// 上传失败时的错误消息，成功时为空。
    /// </remarks>
    /// <example>文件大小超出限制</example>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 文件大小格式化字符串
    /// </summary>
    /// <example>1.5 MB</example>
    public string FileSizeFormat
    {
        get
        {
            if (FileSize < 1024)
                return $"{FileSize} B";
            else if (FileSize < 1024 * 1024)
                return $"{FileSize / 1024:F2} KB";
            else if (FileSize < 1024 * 1024 * 1024)
                return $"{FileSize / (1024 * 1024):F2} MB";
            else
                return $"{FileSize / (1024 * 1024 * 1024):F2} GB";
        }
    }
}