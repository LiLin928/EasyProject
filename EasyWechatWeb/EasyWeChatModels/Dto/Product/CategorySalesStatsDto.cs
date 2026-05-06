namespace EasyWeChatModels.Dto;

/// <summary>
/// 分类销量统计DTO
/// </summary>
public class CategorySalesStatsDto
{
    /// <summary>
    /// 分类名称
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 销售数量
    /// </summary>
    public int SalesCount { get; set; }

    /// <summary>
    /// 销售金额
    /// </summary>
    public decimal SalesAmount { get; set; }
}