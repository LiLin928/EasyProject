using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 删除大屏请求 DTO
/// </summary>
/// <remarks>
/// 用于批量删除大屏
/// </remarks>
public class DeleteScreenDto
{
    /// <summary>
    /// 要删除的大屏ID列表
    /// </summary>
    /// <example>["00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000002"]</example>
    [Required(ErrorMessage = "大屏ID列表不能为空")]
    public List<Guid> Ids { get; set; } = new();
}