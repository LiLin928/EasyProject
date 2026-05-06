namespace EasyWeChatModels.Dto;

/// <summary>
/// 商品规格DTO
/// </summary>
public class ProductSpecDto
{
    /// <summary>
    /// 规格ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 规格名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 是否必选
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 规格选项列表
    /// </summary>
    public List<ProductSpecOptionDto>? Options { get; set; }
}