namespace EasyWeChatModels.Dto;

/// <summary>
/// 商品SKU DTO
/// </summary>
public class ProductSkuDto
{
    /// <summary>
    /// SKU ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// SKU编码
    /// </summary>
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// 规格组合文本（如：红色-XL）
    /// </summary>
    public string SpecText { get; set; } = string.Empty;

    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 库存
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// 图片
    /// </summary>
    public string? Image { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 规格组合详情
    /// </summary>
    public List<SkuSpecItemDto>? SpecItems { get; set; }
}