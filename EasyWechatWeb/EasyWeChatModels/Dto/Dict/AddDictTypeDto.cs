using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 添加字典类型参数
/// </summary>
public class AddDictTypeDto
{
    /// <summary>
    /// 字典编码
    /// </summary>
    [Required(ErrorMessage = "字典编码不能为空")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 字典名称
    /// </summary>
    [Required(ErrorMessage = "字典名称不能为空")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 状态：1-正常，0-禁用
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 中文名称
    /// </summary>
    public string? LabelZh { get; set; }

    /// <summary>
    /// 英文名称
    /// </summary>
    public string? LabelEn { get; set; }
}