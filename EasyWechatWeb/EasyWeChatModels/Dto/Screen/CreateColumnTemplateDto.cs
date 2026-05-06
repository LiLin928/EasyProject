using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 创建列模板 DTO
/// </summary>
public class CreateColumnTemplateDto
{
    [Required(ErrorMessage = "模板名称不能为空")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "模板类型不能为空")]
    public string Type { get; set; } = "table";

    public string? Description { get; set; }
    public Guid? DatasourceId { get; set; }
    public string? SqlQuery { get; set; }

    [Required(ErrorMessage = "列配置不能为空")]
    public List<ColumnConfigDto> Columns { get; set; } = new();
}