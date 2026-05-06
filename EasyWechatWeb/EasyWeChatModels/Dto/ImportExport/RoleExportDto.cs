using System.ComponentModel;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 角色导出数据 DTO
/// </summary>
/// <remarks>
/// 用于角色数据导出时的数据模型。
/// 只包含需要导出的字段。
/// 属性的 Description 特性用于定义 Excel 列标题。
/// </remarks>
public class RoleExportDto
{
    /// <summary>
    /// 角色名称
    /// </summary>
    /// <remarks>
    /// 角色的显示名称。
    /// </remarks>
    /// <example>管理员</example>
    [Description("角色名称")]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    /// <remarks>
    /// 角色的唯一编码标识。
    /// </remarks>
    /// <example>admin</example>
    [Description("角色编码")]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// 角色的详细描述信息。
    /// </remarks>
    /// <example>系统管理员角色，拥有所有权限</example>
    [Description("描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 角色状态文本描述。
    /// 导出时将数值转换为文字便于阅读。
    /// </remarks>
    /// <example>正常</example>
    [Description("状态")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 角色创建的时间。
    /// </remarks>
    /// <example>2024-01-01 12:00:00</example>
    [Description("创建时间")]
    public DateTime? CreateTime { get; set; }
}