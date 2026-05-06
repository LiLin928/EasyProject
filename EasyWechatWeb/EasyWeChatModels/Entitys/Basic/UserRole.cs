using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 用户角色关联实体类
/// </summary>
/// <remarks>
/// 用于建立用户与角色的多对多关系，一个用户可以拥有多个角色
/// </remarks>
[SugarTable("UserRole", "用户角色关联表")]
public class UserRole
{
    /// <summary>
    /// 关联ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 用户ID
    /// </summary>
    /// <remarks>
    /// 关联的用户ID，外键关联User表
    /// </remarks>
    [SugarColumn(ColumnDescription = "用户 ID")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    /// <remarks>
    /// 关联的角色ID，外键关联Role表
    /// </remarks>
    [SugarColumn(ColumnDescription = "角色 ID")]
    public Guid RoleId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 关联记录创建时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}