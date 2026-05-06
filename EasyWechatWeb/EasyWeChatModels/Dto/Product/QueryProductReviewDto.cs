namespace EasyWeChatModels.Dto;

/// <summary>
/// 评价查询参数
/// </summary>
public class QueryProductReviewDto
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
    /// 商品名称
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 评分
    /// </summary>
    public int? Rating { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 开始日期
    /// </summary>
    public string? StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public string? EndDate { get; set; }
}