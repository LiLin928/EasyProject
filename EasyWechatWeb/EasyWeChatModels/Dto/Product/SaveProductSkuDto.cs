namespace EasyWeChatModels.Dto;

/// <summary>
/// 创建/更新SKU参数
/// </summary>
public class SaveProductSkuDto
{
    /// <summary>
    /// SKU ID（更新时必填）
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// SKU编码
    /// </summary>
    public string? SkuCode { get; set; }

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
    /// 规格组合（选项ID列表）
    /// </summary>
    public List<Guid> OptionIds { get; set; } = new();
}