namespace EasyWeChatModels.Dto;

/// <summary>
/// 创建规格选项参数
/// </summary>
public class AddSpecOptionDto
{
    /// <summary>
    /// 选项名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 选项值
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// 价格调整
    /// </summary>
    public decimal? PriceAdjust { get; set; }

    /// <summary>
    /// 库存
    /// </summary>
    public int? Stock { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; } = 0;
}