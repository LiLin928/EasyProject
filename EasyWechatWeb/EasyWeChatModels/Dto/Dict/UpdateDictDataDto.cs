using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新字典数据参数
/// </summary>
public class UpdateDictDataDto
{
    /// <summary>
    /// 字典数据ID
    /// </summary>
    [Required(ErrorMessage = "字典数据ID不能为空")]
    public Guid Id { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// 中文标签
    /// </summary>
    public string? LabelZh { get; set; }

    /// <summary>
    /// 英文标签
    /// </summary>
    public string? LabelEn { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; set; }

    /// <summary>
    /// 状态：1-正常，0-禁用
    /// </summary>
    public int? Status { get; set; }
}