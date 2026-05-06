using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新菜单请求 DTO
/// </summary>
/// <remarks>
/// 用于更新菜单信息时提交的数据，所有字段均为可选
/// </remarks>
public class UpdateMenuDto
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    [Required(ErrorMessage = "菜单ID不能为空")]
    public Guid Id { get; set; }

    /// <summary>
    /// 父菜单ID
    /// </summary>
    /// <remarks>
    /// 父级菜单ID，根菜单为Guid.Empty
    /// </remarks>
    /// <example>00000000-0000-0000-0000-000000000000</example>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 菜单名称（显示标题）
    /// </summary>
    /// <example>用户管理</example>
    public string? MenuName { get; set; }

    /// <summary>
    /// 菜单编码（路由名称）
    /// </summary>
    /// <example>user</example>
    public string? MenuCode { get; set; }

    /// <summary>
    /// 路由路径
    /// </summary>
    /// <example>/system/user</example>
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
    /// <example>user</example>
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>
    /// 菜单显示顺序，数值越小越靠前
    /// </remarks>
    /// <example>1</example>
    public int? Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 菜单状态：1-正常，0-禁用
    /// </remarks>
    /// <example>1</example>
    public int? Status { get; set; }

    /// <summary>
    /// 是否隐藏
    /// </summary>
    /// <remarks>
    /// 是否在菜单中隐藏：0-显示，1-隐藏
    /// </remarks>
    /// <example>0</example>
    public int? Hidden { get; set; }

    /// <summary>
    /// 是否固定标签页
    /// </summary>
    /// <remarks>
    /// 是否固定在标签页（不可关闭）：0-可关闭，1-固定
    /// </remarks>
    /// <example>0</example>
    public int? Affix { get; set; }
}