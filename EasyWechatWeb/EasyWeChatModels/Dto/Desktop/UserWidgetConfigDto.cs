using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 用户组件配置 DTO
/// </summary>
public class UserWidgetConfigDto
{
    /// <summary>
    /// 配置ID
    /// </summary>
    public Guid Id { get; set; }

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
    /// 用户自定义宽度(栅格数)
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 默认高度
    /// </summary>
    public int DefaultHeight { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 数据源类型
    /// </summary>
    public DataSourceType DataSourceType { get; set; }

    /// <summary>
    /// 数据源配置
    /// </summary>
    public string? DataSourceConfig { get; set; }

    /// <summary>
    /// 交互配置
    /// </summary>
    public string? InteractionConfig { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    public int SortOrder { get; set; }
}