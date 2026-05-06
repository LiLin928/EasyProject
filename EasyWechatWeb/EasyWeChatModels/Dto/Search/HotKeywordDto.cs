namespace EasyWeChatModels.Dto;

/// <summary>
/// 热门关键词DTO
/// </summary>
public class HotKeywordDto
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 关键词
    /// </summary>
    public string Keyword { get; set; } = string.Empty;

    /// <summary>
    /// 搜索次数
    /// </summary>
    public int SearchCount { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态：1-启用，0-禁用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态文本
    /// </summary>
    public string StatusText => Status == 1 ? "启用" : "禁用";

    /// <summary>
    /// 是否推荐
    /// </summary>
    public bool IsRecommend { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; } = string.Empty;
}