namespace EasyWeChatModels.Dto;

/// <summary>
/// 热门关键词查询参数
/// </summary>
public class QueryHotKeywordDto
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
    /// 关键词
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 状态：1-启用，0-禁用
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 是否推荐
    /// </summary>
    public bool? IsRecommend { get; set; }
}