namespace EasyWeChatModels.Dto;

/// <summary>
/// 角色查询参数 DTO
/// </summary>
public class QueryRoleDto
{
    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    /// <example>1</example>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    /// <example>10</example>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 角色名称（模糊匹配）
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// 角色编码（模糊匹配）
    /// </summary>
    public string? RoleCode { get; set; }

    /// <summary>
    /// 状态（1-启用，0-禁用）
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 排序字段
    /// </summary>
    public string? SortField { get; set; }

    /// <summary>
    /// 排序方向（ascending/descending）
    /// </summary>
    public string? SortOrder { get; set; }
}