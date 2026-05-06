using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 菜单信息 DTO
/// </summary>
/// <remarks>
/// 用于返回菜单基本信息，支持树形结构。
/// 字段命名与前端 MockMenu 类型保持一致。
/// </remarks>
public class MenuDto
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 父菜单ID
    /// </summary>
    /// <remarks>
    /// 父级菜单ID，根菜单为Guid.Empty
    /// </remarks>
    /// <example>00000000-0000-0000-0000-000000000000</example>
    public Guid ParentId { get; set; }

    /// <summary>
    /// 菜单名称（显示标题）
    /// </summary>
    /// <remarks>
    /// 菜单的显示名称/标题
    /// </remarks>
    /// <example>用户管理</example>
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 菜单编码（路由名称）
    /// </summary>
    /// <remarks>
    /// 菜单的唯一编码标识，用于路由 name 属性
    /// </remarks>
    /// <example>user</example>
    public string? MenuCode { get; set; }

    /// <summary>
    /// 路由路径
    /// </summary>
    /// <remarks>
    /// 前端路由路径
    /// </remarks>
    /// <example>/system</example>
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    /// <remarks>
    /// 前端组件路径，相对于 views 目录
    /// </remarks>
    /// <example>basic/user/index</example>
    public string? Component { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    /// <remarks>
    /// 菜单图标名称或图标类名
    /// </remarks>
    /// <example>setting</example>
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>
    /// 菜单显示顺序，数值越小越靠前
    /// </remarks>
    /// <example>1</example>
    public int Sort { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    /// <remarks>
    /// 菜单类型：1-菜单（可展开的菜单项），2-按钮（具体的操作权限）
    /// </remarks>
    /// <example>1</example>
    public int Type { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 菜单状态：1-正常，0-禁用
    /// </remarks>
    /// <example>1</example>
    public int Status { get; set; }

    /// <summary>
    /// 是否隐藏
    /// </summary>
    /// <remarks>
    /// 是否在菜单中隐藏：0-显示，1-隐藏
    /// </remarks>
    /// <example>0</example>
    public int Hidden { get; set; }

    /// <summary>
    /// 是否固定标签页
    /// </summary>
    /// <remarks>
    /// 是否固定在标签页（不可关闭）：0-可关闭，1-固定
    /// </remarks>
    /// <example>0</example>
    public int Affix { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 子菜单列表
    /// </summary>
    /// <remarks>
    /// 当前菜单的子菜单集合，用于构建菜单树形结构
    /// </remarks>
    public List<MenuDto>? Children { get; set; }
}