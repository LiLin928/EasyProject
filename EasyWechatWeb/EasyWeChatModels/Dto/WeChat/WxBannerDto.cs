namespace EasyWeChatModels.Dto;

/// <summary>
/// 微信轮播图 DTO
/// </summary>
public class WxBannerDto
{
    /// <summary>
    /// 轮播图ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 图片URL
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// 跳转类型：none/product/category/page
    /// </summary>
    public string LinkType { get; set; } = "none";

    /// <summary>
    /// 跳转目标值
    /// </summary>
    public string? LinkValue { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }
}