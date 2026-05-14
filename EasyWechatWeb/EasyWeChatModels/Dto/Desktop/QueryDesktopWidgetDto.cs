using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 桌面组件查询参数 DTO
/// </summary>
public class QueryDesktopWidgetDto
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 组件名称(模糊搜索)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 组件类型
    /// </summary>
    public WidgetType? Type { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}