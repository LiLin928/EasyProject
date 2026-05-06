using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 添加字典数据参数
/// </summary>
public class AddDictDataDto
{
    /// <summary>
    /// 字典类型编码
    /// </summary>
    [Required(ErrorMessage = "字典类型编码不能为空")]
    public string TypeCode { get; set; } = string.Empty;

    /// <summary>
    /// 标签
    /// </summary>
    [Required(ErrorMessage = "标签不能为空")]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// 值
    /// </summary>
    [Required(ErrorMessage = "值不能为空")]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态：1-正常，0-禁用
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 中文标签
    /// </summary>
    public string? LabelZh { get; set; }

    /// <summary>
    /// 英文标签
    /// </summary>
    public string? LabelEn { get; set; }
}