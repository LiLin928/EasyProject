using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 添加用户请求 DTO
/// </summary>
/// <remarks>
/// 用于创建新用户时提交的用户信息
/// </remarks>
public class AddUserDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    /// <remarks>
    /// 用户名必须唯一，长度限制50字符
    /// </remarks>
    /// <example>zhangsan</example>
    [Required(ErrorMessage = "用户名不能为空")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    /// <remarks>
    /// 用户登录密码，将进行加密存储
    /// </remarks>
    /// <example>123456</example>
    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    /// <example>张三</example>
    public string? RealName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    /// <example>13800138000</example>
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    /// <example>zhangsan@example.com</example>
    public string? Email { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    /// <remarks>
    /// 要分配给用户的角色ID列表
    /// </remarks>
    /// <example>["00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000002"]</example>
    public List<Guid>? RoleIds { get; set; }
}