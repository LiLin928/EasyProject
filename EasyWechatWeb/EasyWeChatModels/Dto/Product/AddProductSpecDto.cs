namespace EasyWeChatModels.Dto;

/// <summary>
/// 创建规格参数
/// </summary>
public class AddProductSpecDto
{
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
    public bool Required { get; set; } = true;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 选项列表
    /// </summary>
    public List<AddSpecOptionDto>? Options { get; set; }
}