namespace EasyWeChatModels.Dto;

/// <summary>
/// 字典数据项
/// </summary>
public class DictDataItemDto
{
    /// <summary>
    /// 值
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 中文标签
    /// </summary>
    public string LabelZh { get; set; } = string.Empty;

    /// <summary>
    /// 英文标签
    /// </summary>
    public string? LabelEn { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }
}