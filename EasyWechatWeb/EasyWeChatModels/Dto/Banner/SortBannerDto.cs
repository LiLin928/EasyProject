namespace EasyWeChatModels.Dto;

/// <summary>
/// 批量排序参数
/// </summary>
public class SortBannerDto
{
    /// <summary>
    /// 轮播图ID和排序值列表
    /// </summary>
    public List<BannerSortItem> Items { get; set; } = new List<BannerSortItem>();
}