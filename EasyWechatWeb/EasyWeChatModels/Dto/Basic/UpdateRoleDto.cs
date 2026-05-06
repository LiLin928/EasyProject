using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新角色请求 DTO
/// </summary>
/// <remarks>
/// 用于更新角色信息时提交的数据，所有字段均为可选
/// </remarks>
public class UpdateRoleDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    /// <example>1</example>
    [Required(ErrorMessage = "角色ID不能为空")]
    public Guid Id { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    /// <example>管理员</example>
    public string? RoleName { get; set; }

    /// <summary>
    /// 角色编码
    /// </summary>
    /// <example>admin</example>
    public string? RoleCode { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <example>系统管理员角色，拥有所有权限</example>
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 角色状态：1-正常，0-禁用
    /// </remarks>
    /// <example>1</example>
    public int? Status { get; set; }
}