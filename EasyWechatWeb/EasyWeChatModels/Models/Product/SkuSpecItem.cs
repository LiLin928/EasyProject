namespace EasyWeChatModels.Models.Product;

/// <summary>
/// SKU规格项
/// </summary>
/// <remarks>
/// 用于 ProductSku 的 SpecCombination JSON 字段序列化/反序列化
/// </remarks>
public class SkuSpecItem
{
    /// <summary>规格ID</summary>
    public Guid SpecId { get; set; }

    /// <summary>规格选项ID</summary>
    public Guid OptionId { get; set; }
}