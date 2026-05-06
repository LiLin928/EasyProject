namespace EasyWeChatModels.Dto;

/// <summary>
/// 字典类型信息 DTO
/// </summary>
public class DictTypeDto
{
    /// <summary>
    /// 字典类型ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 字典编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 字典名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

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

    /// <summary>
    /// 版本号
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// 中文名称
    /// </summary>
    public string LabelZh { get; set; } = string.Empty;

    /// <summary>
    /// 英文名称
    /// </summary>
    public string? LabelEn { get; set; }
}