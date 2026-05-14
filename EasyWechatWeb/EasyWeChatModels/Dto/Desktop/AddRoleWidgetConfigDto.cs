using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 保存角色组件配置 DTO
/// </summary>
public class AddRoleWidgetConfigDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [Required(ErrorMessage = "角色ID不能为空")]
    public Guid RoleId { get; set; }

    /// <summary>
    /// 组件配置列表
    /// </summary>
    public List<RoleWidgetConfigItem> Widgets { get; set; } = new();
}

/// <summary>
/// 角色组件配置项
/// </summary>
public class RoleWidgetConfigItem
{
    /// <summary>
    /// 组件ID
    /// </summary>
    public Guid WidgetId { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }
}