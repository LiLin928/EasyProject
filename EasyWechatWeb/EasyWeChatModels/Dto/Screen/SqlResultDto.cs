namespace EasyWeChatModels.Dto;

/// <summary>
/// SQL执行结果 DTO
/// </summary>
/// <remarks>
/// 用于返回SQL查询的结果数据
/// </remarks>
public class SqlResultDto
{
    /// <summary>
    /// 查询结果数据
    /// </summary>
    /// <example>[{"name":"张三","age":25},{"name":"李四","age":30}]</example>
    public List<object> Data { get; set; } = new();

    /// <summary>
    /// 列信息
    /// </summary>
    public List<SqlColumnInfo> Columns { get; set; } = new();
}