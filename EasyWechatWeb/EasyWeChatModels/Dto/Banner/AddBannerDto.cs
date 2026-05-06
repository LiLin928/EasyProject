namespace EasyWeChatModels.Dto;

/// <summary>
/// 添加轮播图参数
/// </summary>
public class AddBannerDto
{
    /// <summary>
    /// 图片URL
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// 跳转类型
    /// </summary>
    public string LinkType { get; set; } = "none";

    /// <summary>
    /// 跳转目标
    /// </summary>
    public string? LinkValue { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; } = 1;
}