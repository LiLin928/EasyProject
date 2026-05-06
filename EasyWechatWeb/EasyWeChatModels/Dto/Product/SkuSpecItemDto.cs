namespace EasyWeChatModels.Dto;

/// <summary>
/// SKU规格项DTO
/// </summary>
public class SkuSpecItemDto
{
    /// <summary>
    /// 规格ID
    /// </summary>
    public Guid SpecId { get; set; }

    /// <summary>
    /// 规格名称
    /// </summary>
    public string SpecName { get; set; } = string.Empty;

    /// <summary>
    /// 选项ID
    /// </summary>
    public Guid OptionId { get; set; }

    /// <summary>
    /// 选项名称
    /// </summary>
    public string OptionName { get; set; } = string.Empty;
}