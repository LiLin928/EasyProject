using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 用户实体类
/// </summary>
/// <remarks>
/// 用于存储系统用户信息，包括登录凭证、个人资料等
/// </remarks>
[SugarTable("User", "用户表")]
public class User
{
    /// <summary>
    /// 用户ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 用户名（唯一）
    /// </summary>
    /// <remarks>
    /// 用于登录的用户名，长度限制50字符，必须唯一
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "用户名")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    /// <remarks>
    /// 用户登录密码，存储加密后的密码哈希值，长度限制255字符
    /// </remarks>
    [SugarColumn(Length = 255, ColumnDescription = "密码")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    /// <remarks>
    /// 用户的真实姓名，长度限制50字符
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "真实姓名")]
    public string? RealName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    /// <remarks>
    /// 用户手机号码，长度限制20字符
    /// </remarks>
    [SugarColumn(Length = 20, IsNullable = true, ColumnDescription = "手机号")]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    /// <remarks>
    /// 用户邮箱地址，长度限制100字符
    /// </remarks>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "邮箱")]
    public string? Email { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    /// <remarks>
    /// 用户头像URL地址，长度限制255字符
    /// </remarks>
    [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "头像")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 用户状态：1-正常，0-禁用。默认值为1
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态：1-正常，0-禁用")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 所属部门ID
    /// </summary>
    [SugarColumn(ColumnDescription = "所属部门ID")]
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 用户记录创建时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <remarks>
    /// 用户信息最后更新时间，可为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    /// <remarks>
    /// 用户最后一次登录系统的时间，可为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "最后登录时间")]
    public DateTime? LastLoginTime { get; set; }
}