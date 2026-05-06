namespace EasyWeChatModels.Dto;

/// <summary>
/// 商品查询 DTO
/// </summary>
public class QueryProductDto
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 商品名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// SKU码
    /// </summary>
    public string? SkuCode { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// 关键字（模糊搜索）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 是否热销
    /// </summary>
    public bool? IsHot { get; set; }

    /// <summary>
    /// 是否新品
    /// </summary>
    public bool? IsNew { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 最低价格
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// 最高价格
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// 排序字段
    /// </summary>
    public string? SortField { get; set; }

    /// <summary>
    /// 排序方式：asc/desc
    /// </summary>
    public string? SortOrder { get; set; }
}