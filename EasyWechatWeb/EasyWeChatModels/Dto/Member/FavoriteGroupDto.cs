namespace EasyWeChatModels.Dto;

/// <summary>
/// 收藏分组DTO
/// </summary>
public class FavoriteGroupDto
{
    /// <summary>
    /// 分组ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 分组名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 收藏数量
    /// </summary>
    public int FavoriteCount { get; set; }
}