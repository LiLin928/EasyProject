namespace EasyWeChatModels.Dto;

/// <summary>
/// 库存查询参数
/// </summary>
public class QueryStockDto
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
    /// 分类ID
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// 是否低库存
    /// </summary>
    public bool? LowStock { get; set; }
}