using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 导出结果 DTO
/// </summary>
/// <remarks>
/// 用于记录数据导出操作的结果信息。
/// 包含导出的文件名、记录数量和导出时间等。
/// 主要用于日志记录和操作反馈。
/// </remarks>
public class ExportResultDto
{
    /// <summary>
    /// 导出的文件名
    /// </summary>
    /// <remarks>
    /// 包含文件扩展名的完整文件名，如 "users_20240101.xlsx"。
    /// 文件名通常包含导出类型和时间戳，便于识别和管理。
    /// </remarks>
    /// <example>users_20240101_120000.xlsx</example>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 导出的记录总数
    /// </summary>
    /// <remarks>
    /// 实际导出的数据记录数量。
    /// 用于验证导出结果是否完整。
    /// </remarks>
    /// <example>100</example>
    public int TotalCount { get; set; }

    /// <summary>
    /// 导出时间
    /// </summary>
    /// <remarks>
    /// 导出操作完成的时间。
    /// 用于日志记录和追踪。
    /// </remarks>
    /// <example>2024-01-01 12:00:00</example>
    public DateTime ExportTime { get; set; }

    /// <summary>
    /// 导出数据类型
    /// </summary>
    /// <remarks>
    /// 导出的数据类型标识，如 "User"、"Role" 等。
    /// 用于区分不同类型的导出操作。
    /// </remarks>
    /// <example>User</example>
    public string ExportType { get; set; } = string.Empty;

    /// <summary>
    /// 操作用户ID
    /// </summary>
    /// <remarks>
    /// 执行导出操作的用户ID。
    /// 用于审计和追踪操作来源。
    /// </remarks>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid OperatorId { get; set; }

    /// <summary>
    /// 操作用户名
    /// </summary>
    /// <remarks>
    /// 执行导出操作的用户名。
    /// 用于日志记录和显示。
    /// </remarks>
    /// <example>admin</example>
    public string? OperatorName { get; set; }
}