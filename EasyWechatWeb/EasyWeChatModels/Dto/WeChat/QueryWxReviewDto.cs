namespace EasyWeChatModels.Dto;

/// <summary>
/// 微信商品评价查询 DTO
/// </summary>
public class QueryWxReviewDto
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
    /// 评分筛选（1-5）
    /// </summary>
    public int? Rating { get; set; }

    /// <summary>
    /// 是否有图
    /// </summary>
    public bool? HasImage { get; set; }
}