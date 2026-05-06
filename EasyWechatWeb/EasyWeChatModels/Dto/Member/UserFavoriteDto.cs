namespace EasyWeChatModels.Dto;

/// <summary>
/// 用户收藏DTO
/// </summary>
public class UserFavoriteDto
{
    /// <summary>
    /// 收藏ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 商品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 商品图片
    /// </summary>
    public string? ProductImage { get; set; }

    /// <summary>
    /// 商品价格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 分组ID
    /// </summary>
    public Guid? GroupId { get; set; }

    /// <summary>
    /// 分组名称
    /// </summary>
    public string? GroupName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; } = string.Empty;
}