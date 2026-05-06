using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 分享大屏配置请求 DTO
/// </summary>
/// <remarks>
/// 用于设置大屏的分享权限
/// </remarks>
public class ShareScreenDto
{
    /// <summary>
    /// 大屏ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    [Required(ErrorMessage = "大屏ID不能为空")]
    public Guid Id { get; set; }

    /// <summary>
    /// 权限配置（JSON格式）
    /// </summary>
    /// <example>{"view":["user1","user2"],"edit":["user1"]}</example>
    [Required(ErrorMessage = "权限配置不能为空")]
    public string Permissions { get; set; } = "{}";
}