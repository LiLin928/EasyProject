namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新热门关键词参数
/// </summary>
public class UpdateHotKeywordDto
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 关键词
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 搜索次数
    /// </summary>
    public int? SearchCount { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 是否推荐
    /// </summary>
    public bool? IsRecommend { get; set; }
}