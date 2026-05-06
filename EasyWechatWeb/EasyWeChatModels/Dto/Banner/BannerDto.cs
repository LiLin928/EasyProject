namespace EasyWeChatModels.Dto;

/// <summary>
/// 轮播图 DTO
/// </summary>
public class BannerDto
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
    public int Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; } = string.Empty;

    /// <summary>
    /// 更新时间
    /// </summary>
    public string? UpdateTime { get; set; }
}