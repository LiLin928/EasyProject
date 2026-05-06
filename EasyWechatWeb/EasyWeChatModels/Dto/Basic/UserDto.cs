using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 用户信息 DTO
/// </summary>
/// <remarks>
/// 用于返回用户基本信息，不包含敏感信息如密码
/// </remarks>
public class UserDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    /// <example>admin</example>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    /// <example>管理员</example>
    public string? RealName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    /// <example>13800138000</example>
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    /// <example>admin@example.com</example>
    public string? Email { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    /// <example>https://example.com/avatar.png</example>
    public string? Avatar { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 用户状态：1-正常，0-禁用
    /// </remarks>
    /// <example>1</example>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <example>2024-01-01 00:00:00</example>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 角色名称列表
    /// </summary>
    /// <remarks>
    /// 用户拥有的角色名称列表
    /// </remarks>
    /// <example>["admin", "user"]</example>
    public List<string>? Roles { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    /// <remarks>
    /// 用户拥有的角色ID列表，用于编辑时回显和更新
    /// </remarks>
    /// <example>["00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000002"]</example>
    public List<Guid>? RoleIds { get; set; }
}