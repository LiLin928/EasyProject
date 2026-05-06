namespace EasyWeChatModels.Dto;

/// <summary>
/// 评价统计 DTO
/// </summary>
public class WxReviewSummaryDto
{
    /// <summary>
    /// 总评价数
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 平均评分
    /// </summary>
    public decimal AvgRating { get; set; }

    /// <summary>
    /// 好评率（4-5星占比）
    /// </summary>
    public decimal GoodRate { get; set; }

    /// <summary>
    /// 各评分数量分布
    /// </summary>
    public Dictionary<int, int> RatingDistribution { get; set; } = new();
}