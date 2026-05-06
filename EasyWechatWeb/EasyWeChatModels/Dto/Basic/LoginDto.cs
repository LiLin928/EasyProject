using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 登录请求 DTO
/// </summary>
/// <remarks>
/// 用于用户登录时提交的用户名和密码信息
/// </remarks>
public class LoginDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    /// <example>admin</example>
    [Required(ErrorMessage = "用户名不能为空")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    /// <example>123456</example>
    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;
}