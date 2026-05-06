namespace EasyWeChatModels.Dto;

/// <summary>
/// SQL验证结果 DTO
/// </summary>
/// <remarks>
/// 用于返回SQL语法验证的结果
/// </remarks>
public class SqlValidateDto
{
    /// <summary>
    /// 是否有效
    /// </summary>
    /// <example>true</example>
    public bool Valid { get; set; }

    /// <summary>
    /// 验证消息（验证失败时返回错误信息）
    /// </summary>
    /// <example>SQL语法错误：缺少FROM子句</example>
    public string? Message { get; set; }

    /// <summary>
    /// 列信息（验证成功时返回）
    /// </summary>
    public List<SqlColumnInfo>? Columns { get; set; }
}