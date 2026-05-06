using System.ComponentModel;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 用户导出数据 DTO
/// </summary>
/// <remarks>
/// 用于用户数据导出时的数据模型。
/// 只包含需要导出的字段，不包含敏感信息如密码。
/// 属性的 Description 特性用于定义 Excel 列标题。
/// </remarks>
public class UserExportDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    /// <remarks>
    /// 用户登录账号，必须唯一。
    /// </remarks>
    /// <example>admin</example>
    [Description("用户名")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    /// <remarks>
    /// 用户真实姓名，用于显示。
    /// </remarks>
    /// <example>管理员</example>
    [Description("真实姓名")]
    public string? RealName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    /// <remarks>
    /// 用户手机号码。
    /// </remarks>
    /// <example>13800138000</example>
    [Description("手机号")]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    /// <remarks>
    /// 用户邮箱地址。
    /// </remarks>
    /// <example>admin@example.com</example>
    [Description("邮箱")]
    public string? Email { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 用户状态文本描述。
    /// 导出时将数值转换为文字便于阅读。
    /// </remarks>
    /// <example>正常</example>
    [Description("状态")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 用户创建的时间。
    /// </remarks>
    /// <example>2024-01-01 12:00:00</example>
    [Description("创建时间")]
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 角色列表
    /// </summary>
    /// <remarks>
    /// 用户拥有的角色名称，多个角色用逗号分隔。
    /// </remarks>
    /// <example>管理员,普通用户</example>
    [Description("角色")]
    public string? Roles { get; set; }
}