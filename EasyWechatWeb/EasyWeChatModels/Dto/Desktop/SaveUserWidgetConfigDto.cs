using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 保存用户桌面配置 DTO
/// </summary>
public class SaveUserWidgetConfigDto
{
    /// <summary>
    /// 组件配置列表
    /// </summary>
    public List<UserWidgetConfigItem> Widgets { get; set; } = new();
}

/// <summary>
/// 用户组件配置项
/// </summary>
public class UserWidgetConfigItem
{
    /// <summary>
    /// 组件ID
    /// </summary>
    public Guid WidgetId { get; set; }

    /// <summary>
    /// 宽度(栅格数1-12)
    /// </summary>
    public int Width { get; set; } = 3;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 排序序号
    /// </summary>
    public int SortOrder { get; set; } = 0;
}