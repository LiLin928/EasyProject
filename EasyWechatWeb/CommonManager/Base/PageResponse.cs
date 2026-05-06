namespace CommonManager.Base;

/// <summary>
/// 分页响应格式，用于封装分页查询的结果
/// </summary>
/// <typeparam name="T">数据项类型</typeparam>
/// <remarks>
/// 该类提供标准的分页响应格式，包含数据列表、分页信息和导航标识。
/// 所有分页查询接口应使用此类封装返回结果。
/// </remarks>
/// <example>
/// <code>
/// // 创建分页响应
/// var users = new List&lt;User&gt; { ... };
/// var response = PageResponse&lt;User&gt;.Create(users, 100, 1, 10);
///
/// // 检查是否有下一页
/// if (response.HasNextPage)
/// {
///     // 加载下一页
/// }
/// </code>
/// </example>
public class PageResponse<T>
{
    /// <summary>
    /// 数据列表，包含当前页的所有数据项
    /// </summary>
    /// <remarks>
    /// 列表大小通常等于 PageSize，但最后一页可能小于 PageSize。
    /// 字段名为 List，JSON 序列化后为 list，与前端保持一致。
    /// </remarks>
    public List<T> List { get; set; } = new();

    /// <summary>
    /// 总记录数，表示查询结果的总数量
    /// </summary>
    /// <remarks>
    /// 用于计算总页数和判断是否有更多数据。
    /// </remarks>
    public int Total { get; set; }

    /// <summary>
    /// 当前页码，从 1 开始计数
    /// </summary>
    /// <remarks>
    /// 页码从 1 开始，有效范围为 1 到 TotalPages。
    /// </remarks>
    public int PageIndex { get; set; }

    /// <summary>
    /// 每页数量，表示每页显示的记录数
    /// </summary>
    /// <remarks>
    /// 常用值为 10、20、50、100。
    /// </remarks>
    public int PageSize { get; set; }

    /// <summary>
    /// 总页数，根据 Total 和 PageSize 计算得出
    /// </summary>
    /// <remarks>
    /// 计算公式：TotalPages = Math.Ceiling(Total / PageSize)
    /// </remarks>
    public int TotalPages { get; set; }

    /// <summary>
    /// 是否有下一页，用于判断是否可以加载更多数据
    /// </summary>
    /// <remarks>
    /// 当 PageIndex 小于 TotalPages 时为 true。
    /// </remarks>
    /// <example>
    /// <code>
    /// if (response.HasNextPage)
    /// {
    ///     var nextPage = await GetPageListAsync(pageIndex + 1, pageSize);
    /// }
    /// </code>
    /// </example>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// 是否有上一页，用于判断是否可以返回前一页
    /// </summary>
    /// <remarks>
    /// 当 PageIndex 大于 1 时为 true。
    /// </remarks>
    /// <example>
    /// <code>
    /// if (response.HasPrevPage)
    /// {
    ///     var prevPage = await GetPageListAsync(pageIndex - 1, pageSize);
    /// }
    /// </code>
    /// </example>
    public bool HasPrevPage { get; set; }

    /// <summary>
    /// 创建分页响应对象
    /// </summary>
    /// <typeparam name="T">数据项类型</typeparam>
    /// <param name="items">当前页的数据列表</param>
    /// <param name="total">总记录数</param>
    /// <param name="pageIndex">当前页码（从 1 开始）</param>
    /// <param name="pageSize">每页数量</param>
    /// <returns>包含完整分页信息的 PageResponse 对象</returns>
    /// <remarks>
    /// 该方法会自动计算 TotalPages、HasNextPage 和 HasPrevPage。
    /// </remarks>
    /// <example>
    /// <code>
    /// var users = await _db.Queryable&lt;User&gt;().ToPageListAsync(1, 10, total);
    /// var response = PageResponse&lt;User&gt;.Create(users, total.Value, 1, 10);
    /// return ApiResponse&lt;PageResponse&lt;User&gt;&gt;.Success(response);
    /// </code>
    /// </example>
    public static PageResponse<T> Create(List<T> items, int total, int pageIndex, int pageSize)
    {
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return new PageResponse<T>
        {
            List = items,
            Total = total,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalPages = totalPages,
            HasNextPage = pageIndex < totalPages,
            HasPrevPage = pageIndex > 1
        };
    }

    /// <summary>
    /// 创建空的分页响应对象
    /// </summary>
    /// <typeparam name="T">数据项类型</typeparam>
    /// <param name="pageIndex">当前页码，默认为 1</param>
    /// <param name="pageSize">每页数量，默认为 10</param>
    /// <returns>空的 PageResponse 对象，Total 为 0，List 为空列表</returns>
    /// <remarks>
    /// 用于在没有数据时返回统一的空响应格式。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 查询无结果时返回空分页
    /// if (list.Count == 0)
    /// {
    ///     return ApiResponse&lt;PageResponse&lt;User&gt;&gt;.Success(PageResponse&lt;User&gt;.Empty());
    /// }
    /// </code>
    /// </example>
    public static PageResponse<T> Empty(int pageIndex = 1, int pageSize = 10)
    {
        return Create(new List<T>(), 0, pageIndex, pageSize);
    }
}