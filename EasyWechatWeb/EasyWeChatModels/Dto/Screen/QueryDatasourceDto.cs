namespace EasyWeChatModels.Dto;

/// <summary>
/// 查询数据源请求 DTO
/// </summary>
/// <remarks>
/// 用于分页查询数据源列表，支持按名称和类型筛选
/// </remarks>
public class QueryDatasourceDto
{
    /// <summary>
    /// 页码
    /// </summary>
    /// <example>1</example>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    /// <example>10</example>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 数据源名称（模糊查询）
    /// </summary>
    /// <example>MySQL</example>
    public string? Name { get; set; }

    /// <summary>
    /// 数据源类型（别名，用于前端兼容）
    /// </summary>
    /// <example>mysql</example>
    public string? DbType { get; set; }

    /// <summary>
    /// 数据源类型（原始字段）
    /// </summary>
    /// <example>mysql</example>
    public string? Type { get; set; }

    /// <summary>
    /// 状态筛选
    /// </summary>
    /// <example>active</example>
    public string? Status { get; set; }
}