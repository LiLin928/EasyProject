namespace EasyWeChatModels.Dto;

/// <summary>
/// 查询操作日志请求 DTO
/// </summary>
/// <remarks>
/// 用于查询操作日志列表时提交的筛选条件。
/// 支持按用户、模块、状态、时间范围等条件进行筛选。
/// </remarks>
public class QueryOperateLogDto
{
    /// <summary>
    /// 页码索引
    /// </summary>
    /// <remarks>
    /// 分页查询的页码，从1开始。
    /// </remarks>
    /// <example>1</example>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页显示数量
    /// </summary>
    /// <remarks>
    /// 分页查询每页显示的记录数，默认10条。
    /// </remarks>
    /// <example>10</example>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 操作用户ID
    /// </summary>
    /// <remarks>
    /// 按用户ID筛选，可选参数。不提供则查询所有用户。
    /// </remarks>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 操作用户名
    /// </summary>
    /// <remarks>
    /// 按用户名模糊匹配筛选，可选参数。
    /// </remarks>
    /// <example>admin</example>
    public string? UserName { get; set; }

    /// <summary>
    /// 操作模块
    /// </summary>
    /// <remarks>
    /// 按模块名称筛选，可选参数。精确匹配。
    /// </remarks>
    /// <example>用户管理</example>
    public string? Module { get; set; }

    /// <summary>
    /// 操作动作
    /// </summary>
    /// <remarks>
    /// 按操作动作模糊匹配筛选，可选参数。
    /// </remarks>
    /// <example>新增</example>
    public string? Action { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 按状态筛选，可选参数。1-成功，0-失败。不提供则查询所有状态。
    /// </remarks>
    /// <example>1</example>
    public int? Status { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    /// <remarks>
    /// 操作时间范围的开始时间，可选参数。
    /// </remarks>
    /// <example>2024-01-01 00:00:00</example>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    /// <remarks>
    /// 操作时间范围的结束时间，可选参数。
    /// </remarks>
    /// <example>2024-01-31 23:59:59</example>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    /// <remarks>
    /// 按IP地址筛选，可选参数。支持模糊匹配。
    /// </remarks>
    /// <example>192.168.1</example>
    public string? Ip { get; set; }
}