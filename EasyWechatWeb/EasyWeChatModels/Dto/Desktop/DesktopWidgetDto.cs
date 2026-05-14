using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 桌面组件 DTO
/// </summary>
public class DesktopWidgetDto
{
    /// <summary>
    /// ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 组件名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 组件类型
    /// </summary>
    public WidgetType Type { get; set; }

    /// <summary>
    /// 图标名称
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 默认宽度(栅格数)
    /// </summary>
    public int DefaultWidth { get; set; }

    /// <summary>
    /// 默认高度(像素)
    /// </summary>
    public int DefaultHeight { get; set; }

    /// <summary>
    /// 数据源类型
    /// </summary>
    public DataSourceType DataSourceType { get; set; }

    /// <summary>
    /// 数据源配置(JSON)
    /// </summary>
    public string? DataSourceConfig { get; set; }

    /// <summary>
    /// 交互配置(JSON)
    /// </summary>
    public string? InteractionConfig { get; set; }

    /// <summary>
    /// 状态: 0禁用 1启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}