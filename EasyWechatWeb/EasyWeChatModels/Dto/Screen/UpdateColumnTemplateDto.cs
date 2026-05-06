using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新列模板 DTO
/// </summary>
public class UpdateColumnTemplateDto
{
    [Required(ErrorMessage = "模板ID不能为空")]
    public Guid Id { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<ColumnConfigDto>? Columns { get; set; }
}