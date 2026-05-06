namespace EasyWeChatModels.Dto;

/// <summary>
/// 评价统计DTO
/// </summary>
public class ReviewStatisticsDto
{
    /// <summary>
    /// 总评价数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 平均评分
    /// </summary>
    public decimal AvgRating { get; set; }

    /// <summary>
    /// 5星数量
    /// </summary>
    public int FiveStarCount { get; set; }

    /// <summary>
    /// 4星数量
    /// </summary>
    public int FourStarCount { get; set; }

    /// <summary>
    /// 3星数量
    /// </summary>
    public int ThreeStarCount { get; set; }

    /// <summary>
    /// 2星数量
    /// </summary>
    public int TwoStarCount { get; set; }

    /// <summary>
    /// 1星数量
    /// </summary>
    public int OneStarCount { get; set; }

    /// <summary>
    /// 待审核数
    /// </summary>
    public int PendingCount { get; set; }

    /// <summary>
    /// 已回复数
    /// </summary>
    public int RepliedCount { get; set; }
}