using System.ComponentModel;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 用户导入数据 DTO
/// </summary>
/// <remarks>
/// 用于用户数据导入时的数据模型。
/// 包含导入所需的字段和验证规则。
/// 导入时会进行数据验证，验证失败会记录错误信息。
/// </remarks>
public class UserImportDto
{
    /// <summary>
    /// 用户名（必填）
    /// </summary>
    /// <remarks>
    /// 用户登录账号，必须唯一且不能为空。
    /// 长度限制50字符。
    /// </remarks>
    /// <example>newuser</example>
    [Description("用户名")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    /// <remarks>
    /// 用户真实姓名，可选。
    /// </remarks>
    /// <example>新用户</example>
    [Description("真实姓名")]
    public string? RealName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    /// <remarks>
    /// 用户手机号码，可选。
    /// </remarks>
    /// <example>13900139000</example>
    [Description("手机号")]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    /// <remarks>
    /// 用户邮箱地址，可选。
    /// </remarks>
    /// <example>newuser@example.com</example>
    [Description("邮箱")]
    public string? Email { get; set; }

    /// <summary>
    /// 密码（必填）
    /// </summary>
    /// <remarks>
    /// 用户登录密码，导入时必填。
    /// 会自动进行加密存储。
    /// </remarks>
    /// <example>123456</example>
    [Description("密码")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 用户状态：1-正常，0-禁用。
    /// 可选，默认为1。
    /// </remarks>
    /// <example>1</example>
    [Description("状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 角色名称列表
    /// </summary>
    /// <remarks>
    /// 要分配的角色名称，多个角色用逗号分隔。
    /// 可选，导入时会根据角色名称查找对应角色ID。
    /// </remarks>
    /// <example>管理员,普通用户</example>
    [Description("角色")]
    public string? Roles { get; set; }
}