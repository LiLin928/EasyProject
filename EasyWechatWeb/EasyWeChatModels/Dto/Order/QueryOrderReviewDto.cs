namespace EasyWeChatModels.Dto;

/// <summary>
/// 订单评价查询参数
/// </summary>
public class QueryOrderReviewDto
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
    /// 订单编号
    /// </summary>
    public string? OrderNo { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid? ProductId { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string? Status { get; set; }
}