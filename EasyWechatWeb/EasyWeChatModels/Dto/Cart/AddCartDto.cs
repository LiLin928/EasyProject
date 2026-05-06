namespace EasyWeChatModels.Dto;

/// <summary>
/// 添加购物车 DTO
/// </summary>
public class AddCartDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Count { get; set; } = 1;
}