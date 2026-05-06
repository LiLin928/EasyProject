namespace EasyWeChatModels.Dto;

/// <summary>
/// 换货商品DTO
/// </summary>
public class ExchangeItemDto
{
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
    /// 商品单价
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Quantity { get; set; }
}