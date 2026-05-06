using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 导入结果 DTO
/// </summary>
/// <remarks>
/// 用于记录数据导入操作的结果信息。
/// 包含成功数量、失败数量、错误详情等。
/// 用于反馈导入操作的执行情况。
/// </remarks>
public class ImportResultDto
{
    /// <summary>
    /// 导入成功的记录数量
    /// </summary>
    /// <remarks>
    /// 成功导入到数据库的记录数量。
    /// 数据验证通过且插入成功。
    /// </remarks>
    /// <example>95</example>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 导入失败的记录数量
    /// </summary>
    /// <remarks>
    /// 导入失败的记录数量，包括验证失败和插入失败。
    /// 失败原因会记录在 Errors 列表中。
    /// </remarks>
    /// <example>5</example>
    public int FailCount { get; set; }

    /// <summary>
    /// 总记录数量
    /// </summary>
    /// <remarks>
    /// Excel 文件中读取的总记录数量。
    /// 等于 SuccessCount + FailCount。
    /// </remarks>
    /// <example>100</example>
    public int TotalCount { get; set; }

    /// <summary>
    /// 是否存在错误
    /// </summary>
    /// <remarks>
    /// 当 FailCount > 0 时为 true。
    /// 用于快速判断导入是否完全成功。
    /// </remarks>
    /// <example>false</example>
    public bool HasErrors => FailCount > 0;

    /// <summary>
    /// 导入错误详情列表
    /// </summary>
    /// <remarks>
    /// 包含每条失败记录的详细错误信息。
    /// 包括行号、字段名、错误消息等。
    /// 用于向用户反馈具体的导入问题。
    /// </remarks>
    public List<ImportErrorDto> Errors { get; set; } = new();

    /// <summary>
    /// 导入时间
    /// </summary>
    /// <remarks>
    /// 导入操作完成的时间。
    /// 用于日志记录和追踪。
    /// </remarks>
    /// <example>2024-01-01 12:00:00</example>
    public DateTime ImportTime { get; set; }

    /// <summary>
    /// 导入数据类型
    /// </summary>
    /// <remarks>
    /// 导入的数据类型标识，如 "User"、"Role" 等。
    /// 用于区分不同类型的导入操作。
    /// </remarks>
    /// <example>User</example>
    public string ImportType { get; set; } = string.Empty;

    /// <summary>
    /// 操作用户ID
    /// </summary>
    /// <remarks>
    /// 执行导入操作的用户ID。
    /// 用于审计和追踪操作来源。
    /// </remarks>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid OperatorId { get; set; }

    /// <summary>
    /// 操作用户名
    /// </summary>
    /// <remarks>
    /// 执行导入操作的用户名。
    /// 用于日志记录和显示。
    /// </remarks>
    /// <example>admin</example>
    public string? OperatorName { get; set; }

    /// <summary>
    /// 导入消息
    /// </summary>
    /// <remarks>
    /// 导入操作的总体消息，包含成功/失败统计。
    /// 用于向用户显示的友好消息。
    /// </remarks>
    /// <example>导入完成，成功95条，失败5条</example>
    public string Message => $"导入完成，成功{SuccessCount}条，失败{FailCount}条";
}