using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 角色菜单关联实体类
/// </summary>
/// <remarks>
/// 用于建立角色与菜单的多对多关系，一个角色可以拥有多个菜单权限
/// </remarks>
[SugarTable("RoleMenu", "角色菜单关联表")]
public class RoleMenu
{
    /// <summary>
    /// 关联ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 角色ID
    /// </summary>
    /// <remarks>
    /// 关联的角色ID，外键关联Role表
    /// </remarks>
    [SugarColumn(ColumnDescription = "角色 ID")]
    public Guid RoleId { get; set; }

    /// <summary>
    /// 菜单ID
    /// </summary>
    /// <remarks>
    /// 关联的菜单ID，外键关联Menu表
    /// </remarks>
    [SugarColumn(ColumnDescription = "菜单 ID")]
    public Guid MenuId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 关联记录创建时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}