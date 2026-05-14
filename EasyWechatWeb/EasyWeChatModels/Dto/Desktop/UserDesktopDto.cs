namespace EasyWeChatModels.Dto;

/// <summary>
/// 用户桌面数据 DTO
/// </summary>
public class UserDesktopDto
{
    /// <summary>
    /// 用户已启用的组件列表
    /// </summary>
    public List<UserWidgetConfigDto> Widgets { get; set; } = new();

    /// <summary>
    /// 用户可用的全部组件列表(角色组件库)
    /// </summary>
    public List<AvailableWidgetDto> AvailableWidgets { get; set; } = new();
}

/// <summary>
/// 可用组件 DTO
/// </summary>
public class AvailableWidgetDto
{
    /// <summary>
    /// 组件ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 组件名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 组件类型
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 默认宽度
    /// </summary>
    public int DefaultWidth { get; set; }

    /// <summary>
    /// 默认高度
    /// </summary>
    public int DefaultHeight { get; set; }

    /// <summary>
    /// 用户是否已启用
    /// </summary>
    public bool IsUserEnabled { get; set; }
}