using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新字典类型参数
/// </summary>
public class UpdateDictTypeDto
{
    /// <summary>
    /// 字典类型ID
    /// </summary>
    [Required(ErrorMessage = "字典类型ID不能为空")]
    public Guid Id { get; set; }

    /// <summary>
    /// 字典名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 状态：1-正常，0-禁用
    /// </summary>
    public int? Status { get; set; }
}