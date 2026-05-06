namespace EasyWeChatModels.Dto;

/// <summary>
/// 购物车项 DTO
/// </summary>
public class CartDto
{
    /// <summary>
    /// 购物车项ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 商品信息
    /// </summary>
    public ProductDto Product { get; set; } = new();

    /// <summary>
    /// 数量
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 是否选中
    /// </summary>
    public bool Selected { get; set; }

    /// <summary>
    /// 小计金额
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}