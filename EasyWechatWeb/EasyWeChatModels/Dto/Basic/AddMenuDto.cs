using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 添加菜单请求 DTO
/// </summary>
/// <remarks>
/// 用于创建新菜单时提交的菜单信息
/// </remarks>
public class AddMenuDto
{
    /// <summary>
    /// 父菜单ID
    /// </summary>
    /// <remarks>
    /// 父级菜单ID，根菜单为Guid.Empty
    /// </remarks>
    /// <example>00000000-0000-0000-0000-000000000000</example>
    public Guid ParentId { get; set; } = Guid.Empty;

    /// <summary>
    /// 菜单名称（显示标题）
    /// </summary>
    /// <remarks>
    /// 菜单的显示名称/标题，长度限制50字符
    /// </remarks>
    /// <example>用户管理</example>
    [Required(ErrorMessage = "菜单名称不能为空")]
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 菜单编码（路由名称）
    /// </summary>
    /// <remarks>
    /// 菜单的唯一编码标识，用于路由 name 属性和权限控制
    /// </remarks>
    /// <example>user</example>
    public string? MenuCode { get; set; }

    /// <summary>
    /// 路由路径
    /// </summary>
    /// <remarks>
    /// 前端路由路径
    /// </remarks>
    /// <example>/system/user</example>
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    /// <remarks>
    /// 前端组件路径，相对于 views 目录，如 'basic/user/index'
    /// </remarks>
    /// <example>basic/user/index</example>
    public string? Component { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    /// <remarks>
    /// 菜单图标名称或图标类名
    /// </remarks>
    /// <example>user</example>
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>
    /// 菜单显示顺序，数值越小越靠前
    /// </remarks>
    /// <example>1</example>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 类型
    /// </summary>
    /// <remarks>
    /// 菜单类型：1-菜单（可展开的菜单项），2-按钮（具体的操作权限）。默认值为1
    /// </remarks>
    /// <example>1</example>
    public int Type { get; set; } = 1;

    /// <summary>
    /// 是否隐藏
    /// </summary>
    /// <remarks>
    /// 是否在菜单中隐藏：0-显示，1-隐藏。默认值为0
    /// </remarks>
    /// <example>0</example>
    public int Hidden { get; set; } = 0;

    /// <summary>
    /// 是否固定标签页
    /// </summary>
    /// <remarks>
    /// 是否固定在标签页（不可关闭）：0-可关闭，1-固定。默认值为0
    /// </remarks>
    /// <example>0</example>
    public int Affix { get; set; } = 0;
}