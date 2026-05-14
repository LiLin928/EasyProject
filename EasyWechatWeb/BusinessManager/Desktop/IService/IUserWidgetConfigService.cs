using EasyWeChatModels.Dto;

namespace BusinessManager.Desktop.IService;

public interface IUserWidgetConfigService
{
    /// <summary>
    /// 获取用户桌面数据
    /// </summary>
    Task<UserDesktopDto> GetUserDesktopAsync(Guid userId);

    /// <summary>
    /// 保存用户桌面配置
    /// </summary>
    Task<bool> SaveAsync(Guid userId, SaveUserWidgetConfigDto dto);

    /// <summary>
    /// 重置用户桌面为角色默认布局
    /// </summary>
    Task<bool> ResetAsync(Guid userId);

    /// <summary>
    /// 初始化用户桌面(从角色模板复制)
    /// </summary>
    Task<bool> InitFromRoleAsync(Guid userId, Guid roleId);
}