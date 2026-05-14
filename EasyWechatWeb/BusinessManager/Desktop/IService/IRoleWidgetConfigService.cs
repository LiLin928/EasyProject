using EasyWeChatModels.Dto;

namespace BusinessManager.Desktop.IService;

public interface IRoleWidgetConfigService
{
    /// <summary>
    /// 获取角色的组件配置列表
    /// </summary>
    Task<List<RoleWidgetConfigDto>> GetByRoleIdAsync(Guid roleId);

    /// <summary>
    /// 保存角色组件配置(批量)
    /// </summary>
    Task<bool> SaveAsync(AddRoleWidgetConfigDto dto);

    /// <summary>
    /// 获取角色可用的组件列表(用于分配选择)
    /// </summary>
    Task<List<AvailableWidgetDto>> GetAvailableWidgetsAsync(Guid roleId);
}