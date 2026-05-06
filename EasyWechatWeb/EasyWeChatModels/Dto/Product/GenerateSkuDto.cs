namespace EasyWeChatModels.Dto;

/// <summary>
/// 批量生成SKU参数
/// </summary>
public class GenerateSkuDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 基础价格
    /// </summary>
    public decimal BasePrice { get; set; }

    /// <summary>
    /// 默认库存
    /// </summary>
    public int DefaultStock { get; set; } = 100;
}