namespace EasyWeChatModels.Dto;

/// <summary>
/// 文件记录 DTO
/// </summary>
/// <remarks>
/// 用于返回文件记录的基本信息，不包含敏感的存储路径信息。
/// 包含文件的元数据和访问URL。
/// </remarks>
public class FileRecordDto
{
    /// <summary>
    /// 文件记录ID
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 原始文件名
    /// </summary>
    /// <remarks>
    /// 用户上传时的原始文件名，包含扩展名。
    /// </remarks>
    /// <example>report.pdf</example>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    /// <remarks>
    /// 文件的大小，以字节为单位。
    /// </remarks>
    /// <example>1024000</example>
    public long FileSize { get; set; }

    /// <summary>
    /// 文件扩展名
    /// </summary>
    /// <remarks>
    /// 文件的扩展名，不含点号。
    /// </remarks>
    /// <example>pdf</example>
    public string FileExt { get; set; } = string.Empty;

    /// <summary>
    /// 内容类型（MIME类型）
    /// </summary>
    /// <remarks>
    /// 文件的MIME类型，用于浏览器正确处理文件。
    /// </remarks>
    /// <example>application/pdf</example>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// 文件访问URL
    /// </summary>
    /// <remarks>
    /// 文件的访问URL，可用于下载或预览。
    /// 私有桶的URL为预签名URL，有过期时间。
    /// </remarks>
    /// <example>http://localhost:9000/easyproject/document/2024/01/uuid.pdf</example>
    public string? Url { get; set; }

    /// <summary>
    /// 上传用户ID
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid UserId { get; set; }

    /// <summary>
    /// 业务ID
    /// </summary>
    /// <remarks>
    /// 文件关联的业务记录ID。
    /// </remarks>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid? BusinessId { get; set; }

    /// <summary>
    /// 文件状态
    /// </summary>
    /// <remarks>
    /// 文件状态：1-正常，0-已删除。
    /// </remarks>
    /// <example>1</example>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <example>2024-01-01 00:00:00</example>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 文件大小格式化字符串
    /// </summary>
    /// <remarks>
    /// 将文件大小转换为易读的格式，如"1.5 MB"、"500 KB"。
    /// </remarks>
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