using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新用户请求 DTO
/// </summary>
/// <remarks>
/// 用于更新用户信息时提交的数据，所有字段均为可选
/// </remarks>
public class UpdateUserDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    [Required(ErrorMessage = "用户ID不能为空")]
    public Guid Id { get; set; }

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
    /// 状态
    /// </summary>
    /// <remarks>
    /// 用户状态：1-正常，0-禁用
    /// </remarks>
    /// <example>1</example>
    public int? Status { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    /// <remarks>
    /// 要分配给用户的角色ID列表，传入时会更新用户角色关联
    /// </remarks>
    /// <example>["00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000002"]</example>
    public List<Guid>? RoleIds { get; set; }
}