using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 角色信息 DTO
/// </summary>
/// <remarks>
/// 用于返回角色基本信息
/// </remarks>
public class RoleDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    /// <example>1</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    /// <example>管理员</example>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    /// <remarks>
    /// 角色的唯一编码标识，用于程序中判断角色权限
    /// </remarks>
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
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <example>2024-01-01 00:00:00</example>
    public DateTime? CreateTime { get; set; }
}