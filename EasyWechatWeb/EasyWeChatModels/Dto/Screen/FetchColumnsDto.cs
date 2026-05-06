using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 获取列信息参数 DTO
/// </summary>
public class FetchColumnsDto
{
    [Required(ErrorMessage = "数据源ID不能为空")]
    public Guid DatasourceId { get; set; }

    [Required(ErrorMessage = "SQL语句不能为空")]
    public string SqlQuery { get; set; } = string.Empty;
}