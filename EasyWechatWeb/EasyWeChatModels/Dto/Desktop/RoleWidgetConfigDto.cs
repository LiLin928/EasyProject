using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 角色组件配置 DTO
/// </summary>
public class RoleWidgetConfigDto
{
    /// <summary>
    /// 配置ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// 组件ID
    /// </summary>
    public Guid WidgetId { get; set; }

    /// <summary>
    /// 组件名称
    /// </summary>
    public string WidgetName { get; set; } = string.Empty;

    /// <summary>
    /// 组件类型
    /// </summary>
    public WidgetType WidgetType { get; set; }

    /// <summary>
    /// 默认宽度
    /// </summary>
    public int DefaultWidth { get; set; }

    /// <summary>
    /// 默认高度
    /// </summary>
    public int DefaultHeight { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }
}