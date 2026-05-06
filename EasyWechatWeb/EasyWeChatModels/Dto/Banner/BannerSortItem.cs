namespace EasyWeChatModels.Dto;

/// <summary>
/// 轮播图排序项
/// </summary>
public class BannerSortItem
{
    /// <summary>
    /// 轮播图ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 排序值
    /// </summary>
    public int Sort { get; set; }
}