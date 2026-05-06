namespace EasyWeChatModels.Dto;

/// <summary>
/// 售后商品DTO
/// </summary>
public class RefundItemDto
{
    /// <summary>
    /// 明细ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 售后记录ID
    /// </summary>
    public Guid RefundId { get; set; }

    /// <summary>
    /// 订单商品ID
    /// </summary>
    public Guid OrderItemId { get; set; }

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

    /// <summary>
    /// 金额
    /// </summary>
    public decimal Amount { get; set; }
}