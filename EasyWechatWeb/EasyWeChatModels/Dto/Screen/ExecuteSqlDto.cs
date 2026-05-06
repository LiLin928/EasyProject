using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 执行SQL请求 DTO
/// </summary>
/// <remarks>
/// 用于执行数据源的SQL查询
/// </remarks>
public class ExecuteSqlDto
{
    /// <summary>
    /// 数据源ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    [Required(ErrorMessage = "数据源ID不能为空")]
    public Guid DatasourceId { get; set; }

    /// <summary>
    /// SQL语句
    /// </summary>
    /// <example>SELECT * FROM users WHERE status = @status</example>
    [Required(ErrorMessage = "SQL语句不能为空")]
    public string Sql { get; set; } = string.Empty;

    /// <summary>
    /// 参数（JSON格式）
    /// </summary>
    /// <example>{"status":1}</example>
    public string? Params { get; set; }
}