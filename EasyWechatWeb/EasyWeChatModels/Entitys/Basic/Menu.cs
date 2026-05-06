using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 菜单实体类
/// </summary>
/// <remarks>
/// 用于存储系统菜单信息，支持树形结构的菜单和按钮权限配置
/// </remarks>
[SugarTable("Menu", "菜单表")]
public class Menu
{
    /// <summary>
    /// 菜单ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 父级ID
    /// </summary>
    /// <remarks>
    /// 父级菜单ID，根菜单为Guid.Empty，用于构建菜单树形结构
    /// </remarks>
    [SugarColumn(ColumnDescription = "父级 ID")]
    public Guid ParentId { get; set; } = Guid.Empty;

    /// <summary>
    /// 菜单名称（显示标题）
    /// </summary>
    /// <remarks>
    /// 菜单的显示名称/标题，长度限制50字符
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "菜单名称")]
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 菜单编码（路由名称）
    /// </summary>
    /// <remarks>
    /// 菜单的唯一编码标识，用于路由 name 属性和权限控制，长度限制50字符
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "菜单编码")]
    public string? MenuCode { get; set; }

    /// <summary>
    /// 路由路径
    /// </summary>
    /// <remarks>
    /// 前端路由路径，长度限制200字符
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "路由路径")]
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    /// <remarks>
    /// 前端组件路径，相对于 views 目录，如 'basic/user/index'，长度限制200字符
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "组件路径")]
    public string? Component { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    /// <remarks>
    /// 菜单图标名称或图标类名，长度限制50字符
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "图标")]
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>
    /// 菜单显示顺序，数值越小越靠前，默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 类型
    /// </summary>
    /// <remarks>
    /// 菜单类型：1-菜单（可展开的菜单项），2-按钮（具体的操作权限）。默认值为1
    /// </remarks>
    [SugarColumn(ColumnDescription = "类型：1-菜单，2-按钮")]
    public int Type { get; set; } = 1;

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 菜单状态：1-正常，0-禁用。默认值为1
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态：1-正常，0-禁用")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 是否隐藏
    /// </summary>
    /// <remarks>
    /// 是否在菜单中隐藏：0-显示，1-隐藏。隐藏的菜单不在侧边栏显示，但路由仍可访问。默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否隐藏：0-显示，1-隐藏")]
    public int Hidden { get; set; } = 0;

    /// <summary>
    /// 是否固定标签页
    /// </summary>
    /// <remarks>
    /// 是否固定在标签页（不可关闭）：0-可关闭，1-固定。默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否固定：0-可关闭，1-固定")]
    public int Affix { get; set; } = 0;

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 菜单记录创建时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <remarks>
    /// 菜单记录最后更新时间
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}