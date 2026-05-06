namespace EasyWeChatModels.Dto;

/// <summary>
/// 新增热门关键词参数
/// </summary>
public class AddHotKeywordDto
{
    /// <summary>
    /// 关键词
    /// </summary>
    public string Keyword { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态：1-启用，0-禁用
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 是否推荐
    /// </summary>
    public bool IsRecommend { get; set; } = false;
}