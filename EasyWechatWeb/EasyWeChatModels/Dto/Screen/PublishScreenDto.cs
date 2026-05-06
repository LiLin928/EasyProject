using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 发布大屏请求 DTO
/// </summary>
/// <remarks>
/// 用于发布大屏生成公开访问链接
/// </remarks>
public class PublishScreenDto
{
    /// <summary>
    /// 大屏ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    [Required(ErrorMessage = "大屏ID不能为空")]
    public Guid ScreenId { get; set; }
}