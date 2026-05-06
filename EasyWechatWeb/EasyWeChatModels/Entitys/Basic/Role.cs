using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 角色实体类
/// </summary>
/// <remarks>
/// 用于存储系统角色信息，用于权限管理和用户角色分配
/// </remarks>
[SugarTable("Role", "角色表")]
public class Role
{
    /// <summary>
    /// 角色ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 角色名称
    /// </summary>
    /// <remarks>
    /// 角色的显示名称，长度限制50字符
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "角色名称")]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    /// <remarks>
    /// 角色的唯一编码标识，用于程序中判断角色权限，长度限制50字符
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "角色编码")]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// 角色的详细描述信息，长度限制200字符，可为空
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 角色状态：1-正常，0-禁用。默认值为1
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态：1-正常，0-禁用")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 角色记录创建时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}