using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新部门请求 DTO
/// </summary>
/// <remarks>
/// 用于更新部门信息时提交的数据，支持部分更新
/// </remarks>
public class UpdateDepartmentDto
{
    /// <summary>
    /// 部门ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    [Required(ErrorMessage = "部门ID不能为空")]
    public Guid Id { get; set; }

    /// <summary>
    /// 上级部门ID
    /// </summary>
    /// <remarks>
    /// 上级部门ID，根部门传 null 或 Guid.Empty
    /// </remarks>
    /// <example>null</example>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    /// <remarks>
    /// 部门名称，长度限制100字符
    /// </remarks>
    /// <example>技术部</example>
    [Required(ErrorMessage = "部门名称不能为空")]
    [MaxLength(100, ErrorMessage = "部门名称不能超过100字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 部门编码
    /// </summary>
    /// <remarks>
    /// 部门编码，用于唯一标识
    /// </remarks>
    /// <example>TECH</example>
    [MaxLength(50, ErrorMessage = "部门编码不能超过50字符")]
    public string? Code { get; set; }

    /// <summary>
    /// 部门主管ID
    /// </summary>
    /// <remarks>
    /// 部门主管/负责人的用户ID
    /// </remarks>
    /// <example>00000000-0000-0000-0000-000000000010</example>
    public Guid? ManagerId { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    /// <example>13800138000</example>
    [MaxLength(20, ErrorMessage = "联系电话不能超过20字符")]
    public string? Phone { get; set; }

    /// <summary>
    /// 部门邮箱
    /// </summary>
    /// <example>tech@example.com</example>
    [MaxLength(100, ErrorMessage = "部门邮箱不能超过100字符")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? Email { get; set; }

    /// <summary>
    /// 部门描述
    /// </summary>
    /// <example>技术研发部门</example>
    [MaxLength(500, ErrorMessage = "部门描述不能超过500字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>
    /// 显示顺序，数值越小越靠前
    /// </remarks>
    /// <example>1</example>
    public int Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 部门状态：1-正常，0-禁用
    /// </remarks>
    /// <example>1</example>
    public int Status { get; set; }
}