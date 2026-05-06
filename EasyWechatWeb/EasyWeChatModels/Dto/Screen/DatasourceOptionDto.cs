namespace EasyWeChatModels.Dto;

/// <summary>
/// 数据源选项 DTO
/// </summary>
/// <remarks>
/// 用于下拉选择的数据源选项
/// </remarks>
public class DatasourceOptionDto
{
    /// <summary>
    /// 数据源ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 数据源名称
    /// </summary>
    /// <example>MySQL主库</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 数据源类型
    /// </summary>
    /// <example>MySQL</example>
    public string Type { get; set; } = string.Empty;
}