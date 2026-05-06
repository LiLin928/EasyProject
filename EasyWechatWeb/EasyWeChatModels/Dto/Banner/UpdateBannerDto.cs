namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新轮播图参数
/// </summary>
public class UpdateBannerDto : AddBannerDto
{
    /// <summary>
    /// 轪播图ID
    /// </summary>
    public Guid Id { get; set; }
}