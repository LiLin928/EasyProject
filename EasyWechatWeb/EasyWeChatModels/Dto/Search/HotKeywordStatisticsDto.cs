namespace EasyWeChatModels.Dto;

/// <summary>
/// 热门关键词统计DTO
/// </summary>
public class HotKeywordStatisticsDto
{
    /// <summary>
    /// 总关键词数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 启用数量
    /// </summary>
    public int ActiveCount { get; set; }

    /// <summary>
    /// 推荐数量
    /// </summary>
    public int RecommendCount { get; set; }

    /// <summary>
    /// 总搜索次数
    /// </summary>
    public int TotalSearchCount { get; set; }
}