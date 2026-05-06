using System.ComponentModel;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 角色导入数据 DTO
/// </summary>
/// <remarks>
/// 用于角色数据导入时的数据模型。
/// 包含导入所需的字段和验证规则。
/// 导入时会进行数据验证，验证失败会记录错误信息。
/// </remarks>
public class RoleImportDto
{
    /// <summary>
    /// 角色名称（必填）
    /// </summary>
    /// <remarks>
    /// 角色的显示名称，必须唯一且不能为空。
    /// 长度限制50字符。
    /// </remarks>
    /// <example>普通用户</example>
    [Description("角色名称")]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码（必填）
    /// </summary>
    /// <remarks>
    /// 角色的唯一编码标识，必须唯一且不能为空。
    /// 长度限制50字符。
    /// </remarks>
    /// <example>user</example>
    [Description("角色编码")]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// 角色的详细描述信息，可选。
    /// </remarks>
    /// <example>普通用户角色，拥有基本权限</example>
    [Description("描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 角色状态：1-正常，0-禁用。
    /// 可选，默认为1。
    /// </remarks>
    /// <example>1</example>
    [Description("状态")]
    public int Status { get; set; } = 1;
}