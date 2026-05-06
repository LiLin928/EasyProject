namespace EasyWeChatModels.Dto;

/// <summary>
/// SQL列信息 DTO
/// </summary>
/// <remarks>
/// 用于描述结果集的列结构
/// </remarks>
public class SqlColumnInfo
{
    /// <summary>
    /// 列名
    /// </summary>
    /// <example>user_name</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 列数据类型
    /// </summary>
    /// <example>VARCHAR</example>
    public string Type { get; set; } = string.Empty;
}