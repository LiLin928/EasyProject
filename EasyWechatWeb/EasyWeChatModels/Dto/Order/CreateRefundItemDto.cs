namespace EasyWeChatModels.Dto;

/// <summary>
/// 创建售后商品参数
/// </summary>
public class CreateRefundItemDto
{
    /// <summary>
    /// 订单项ID
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
    /// 退货数量
    /// </summary>
    public int Quantity { get; set; }
}