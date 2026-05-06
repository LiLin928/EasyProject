namespace EasyWeChatModels.Dto;

/// <summary>
/// 库存记录查询参数
/// </summary>
public class QueryStockRecordDto
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
    /// 商品ID
    /// </summary>
    public Guid? ProductId { get; set; }

    /// <summary>
    /// 商品名称（模糊查询）
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// 变动类型
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 开始日期
    /// </summary>
    public string? StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public string? EndDate { get; set; }
}