using System.ComponentModel.DataAnnotations;
using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 新增桌面组件 DTO
/// </summary>
public class AddDesktopWidgetDto
{
    /// <summary>
    /// 组件名称
    /// </summary>
    [Required(ErrorMessage = "组件名称不能为空")]
    [MaxLength(50, ErrorMessage = "组件名称最长50字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 组件类型
    /// </summary>
    [Required(ErrorMessage = "组件类型不能为空")]
    public WidgetType Type { get; set; }

    /// <summary>
    /// 图标名称
    /// </summary>
    [MaxLength(50)]
    public string? Icon { get; set; }

    /// <summary>
    /// 默认宽度(栅格数1-12)
    /// </summary>
    [Range(1, 12, ErrorMessage = "宽度必须在1-12之间")]
    public int DefaultWidth { get; set; } = 3;

    /// <summary>
    /// 默认高度(像素)
    /// </summary>
    [Range(60, 500, ErrorMessage = "高度必须在60-500之间")]
    public int DefaultHeight { get; set; } = 120;

    /// <summary>
    /// 数据源类型
    /// </summary>
    public DataSourceType DataSourceType { get; set; } = DataSourceType.Api;

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
    public int Status { get; set; } = 1;
}