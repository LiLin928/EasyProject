using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 添加角色请求 DTO
/// </summary>
/// <remarks>
/// 用于创建新角色时提交的角色信息
/// </remarks>
public class AddRoleDto
{
    /// <summary>
    /// 角色名称
    /// </summary>
    /// <remarks>
    /// 角色的显示名称，长度限制50字符
    /// </remarks>
    /// <example>普通用户</example>
    [Required(ErrorMessage = "角色名称不能为空")]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    /// <remarks>
    /// 角色的唯一编码标识，用于程序中判断角色权限
    /// </remarks>
    /// <example>user</example>
    public string? RoleCode { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <example>普通用户角色，拥有基本权限</example>
    public string? Description { get; set; }
}