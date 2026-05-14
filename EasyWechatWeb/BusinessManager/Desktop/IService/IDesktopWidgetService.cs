using EasyWeChatModels.Dto;

namespace BusinessManager.Desktop.IService;

public interface IDesktopWidgetService
{
    /// <summary>
    /// 获取组件列表(分页)
    /// </summary>
    Task<(List<DesktopWidgetDto> list, int total)> GetListAsync(QueryDesktopWidgetDto query);

    /// <summary>
    /// 获取组件详情
    /// </summary>
    Task<DesktopWidgetDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建组件
    /// </summary>
    Task<Guid> AddAsync(AddDesktopWidgetDto dto);

    /// <summary>
    /// 更新组件
    /// </summary>
    Task<bool> UpdateAsync(UpdateDesktopWidgetDto dto);

    /// <summary>
    /// 删除组件
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 获取所有启用的组件列表(用于分配)
    /// </summary>
    Task<List<DesktopWidgetDto>> GetEnabledListAsync();
}