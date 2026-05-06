namespace EasyWeChatModels.Dto;

/// <summary>
/// 订单项 DTO
/// </summary>
public class OrderItemDto
{
    /// <summary>
    /// 订单项ID
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
    /// 单价
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 小计金额
    /// </summary>
    public decimal Amount { get; set; }
}