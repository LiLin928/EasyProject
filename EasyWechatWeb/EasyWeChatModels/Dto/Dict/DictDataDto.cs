namespace EasyWeChatModels.Dto;

/// <summary>
/// 字典数据信息 DTO
/// </summary>
public class DictDataDto
{
    /// <summary>
    /// 字典数据ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 字典类型编码
    /// </summary>
    public string TypeCode { get; set; } = string.Empty;

    /// <summary>
    /// 标签
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// 中文标签
    /// </summary>
    public string LabelZh { get; set; } = string.Empty;

    /// <summary>
    /// 英文标签
    /// </summary>
    public string? LabelEn { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态：1-正常，0-禁用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; } = string.Empty;

    /// <summary>
    /// 更新时间
    /// </summary>
    public string? UpdateTime { get; set; }
}