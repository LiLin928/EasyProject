using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 验证SQL请求 DTO
/// </summary>
/// <remarks>
/// 用于验证SQL语法是否正确
/// </remarks>
public class ValidateSqlDto
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
    /// <example>SELECT * FROM users WHERE status = 1</example>
    [Required(ErrorMessage = "SQL语句不能为空")]
    public string Sql { get; set; } = string.Empty;
}