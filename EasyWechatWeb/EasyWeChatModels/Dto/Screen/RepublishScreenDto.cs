using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 重新上架请求 DTO
/// </summary>
/// <remarks>
/// 用于重新发布已下架的大屏
/// </remarks>
public class RepublishScreenDto
{
    /// <summary>
    /// 发布ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    [Required(ErrorMessage = "发布ID不能为空")]
    public Guid PublishId { get; set; }
}