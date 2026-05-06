using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 文件记录实体类
/// </summary>
/// <remarks>
/// 用于存储上传文件的元数据信息，包括文件名、存储路径、大小、类型等。
/// 支持MinIO对象存储，可关联业务类型和业务ID。
/// </remarks>
[SugarTable("FileRecord", "文件记录表")]
public class FileRecord
{
    /// <summary>
    /// 文件记录ID（主键，GUID）
    /// </summary>
    /// <remarks>
    /// 文件记录的唯一标识符，系统自动生成GUID。
    /// </remarks>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 原始文件名
    /// </summary>
    /// <remarks>
    /// 用户上传时的原始文件名，包含扩展名。
    /// 长度限制255字符。
    /// </remarks>
    /// <example>report.pdf</example>
    [SugarColumn(Length = 255, ColumnDescription = "原始文件名")]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 文件存储路径
    /// </summary>
    /// <remarks>
    /// 文件在MinIO对象存储中的完整路径，包含目录结构。
    /// 格式通常为：业务类型/日期/UUID.扩展名
    /// </remarks>
    /// <example>document/2024/01/uuid.pdf</example>
    [SugarColumn(Length = 500, ColumnDescription = "文件存储路径")]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    /// <remarks>
    /// 文件的大小，以字节为单位。
    /// 用于统计存储空间和限制上传大小。
    /// </remarks>
    /// <example>1024000</example>
    [SugarColumn(ColumnDescription = "文件大小（字节）")]
    public long FileSize { get; set; }

    /// <summary>
    /// 文件扩展名
    /// </summary>
    /// <remarks>
    /// 文件的扩展名，不含点号。
    /// 用于识别文件类型和设置Content-Type。
    /// </remarks>
    /// <example>pdf</example>
    [SugarColumn(Length = 50, ColumnDescription = "文件扩展名")]
    public string FileExt { get; set; } = string.Empty;

    /// <summary>
    /// 内容类型（MIME类型）
    /// </summary>
    /// <remarks>
    /// 文件的MIME类型，用于浏览器正确处理文件。
    /// 如：image/jpeg、application/pdf等。
    /// </remarks>
    /// <example>application/pdf</example>
    [SugarColumn(Length = 100, ColumnDescription = "内容类型")]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// MinIO桶名称
    /// </summary>
    /// <remarks>
    /// 文件存储的MinIO桶名称。
    /// 不同业务类型可使用不同的桶。
    /// </remarks>
    /// <example>easyproject</example>
    [SugarColumn(Length = 100, ColumnDescription = "MinIO桶名称")]
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// MinIO对象名称
    /// </summary>
    /// <remarks>
    /// 文件在MinIO中的对象名称，与FilePath相同。
    /// 用于MinIO API操作。
    /// </remarks>
    /// <example>document/2024/01/uuid.pdf</example>
    [SugarColumn(Length = 500, ColumnDescription = "MinIO对象名称")]
    public string ObjectName { get; set; } = string.Empty;

    /// <summary>
    /// 文件访问URL
    /// </summary>
    /// <remarks>
    /// 文件的直接访问URL，不含签名信息。
    /// 仅适用于公开桶，私有桶需使用预签名URL。
    /// </remarks>
    /// <example>http://localhost:9000/easyproject/document/2024/01/uuid.pdf</example>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "文件访问URL")]
    public string? Url { get; set; }

    /// <summary>
    /// 上传用户ID
    /// </summary>
    /// <remarks>
    /// 上传该文件的用户ID，关联User表。
    /// 用于追踪文件来源和权限控制。
    /// </remarks>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [SugarColumn(ColumnDescription = "上传用户ID")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 业务ID
    /// </summary>
    /// <remarks>
    /// 文件关联的业务记录ID，如订单ID、产品ID等。
    /// 用于业务系统查询相关文件。
    /// </remarks>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [SugarColumn(IsNullable = true, ColumnDescription = "业务ID")]
    public Guid? BusinessId { get; set; }

    /// <summary>
    /// 文件状态
    /// </summary>
    /// <remarks>
    /// 文件状态：1-正常，0-已删除。
    /// 删除操作为逻辑删除，实际文件可能仍保留。
    /// </remarks>
    /// <example>1</example>
    [SugarColumn(ColumnDescription = "状态：1-正常，0-已删除")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 文件上传时间，记录文件的创建时间。
    /// </remarks>
    /// <example>2024-01-01 00:00:00</example>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}