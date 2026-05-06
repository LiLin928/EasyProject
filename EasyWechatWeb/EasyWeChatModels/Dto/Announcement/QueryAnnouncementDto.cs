namespace EasyWeChatModels.Dto;

/// <summary>
/// 查询公告请求 DTO
/// </summary>
/// <remarks>
/// 用于查询公告列表时的筛选条件
/// </remarks>
public class QueryAnnouncementDto
{
    /// <summary>
    /// 页码
    /// </summary>
    /// <remarks>
    /// 分页查询的页码，从1开始
    /// </remarks>
    /// <example>1</example>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    /// <remarks>
    /// 分页查询的每页数量，默认10条
    /// </remarks>
    /// <example>10</example>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 标题关键字
    /// </summary>
    /// <remarks>
    /// 用于模糊搜索公告标题
    /// </remarks>
    /// <example>升级</example>
    public string? Title { get; set; }

    /// <summary>
    /// 类型：1全员公告 2定向公告
    /// </summary>
    /// <remarks>
    /// 按公告类型筛选
    /// </remarks>
    /// <example>1</example>
    public int? Type { get; set; }

    /// <summary>
    /// 级别：1普通 2重要 3紧急
    /// </summary>
    /// <remarks>
    /// 按公告级别筛选
    /// </remarks>
    /// <example>2</example>
    public int? Level { get; set; }

    /// <summary>
    /// 状态：0草稿 1已发布 2已撤回
    /// </summary>
    /// <remarks>
    /// 按公告状态筛选
    /// </remarks>
    /// <example>1</example>
    public int? Status { get; set; }

    /// <summary>
    /// 是否置顶
    /// </summary>
    /// <remarks>
    /// 按是否置顶筛选
    /// </remarks>
    /// <example>1</example>
    public int? IsTop { get; set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    /// <remarks>
    /// 按创建人筛选
    /// </remarks>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid? CreatorId { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    /// <remarks>
    /// 按发布时间范围筛选，开始时间
    /// </remarks>
    /// <example>2024-01-01</example>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    /// <remarks>
    /// 按发布时间范围筛选，结束时间
    /// </remarks>
    /// <example>2024-12-31</example>
    public DateTime? EndTime { get; set; }
}