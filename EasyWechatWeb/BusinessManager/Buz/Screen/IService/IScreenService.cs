using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Basic.IService;

/// <summary>
/// 大屏服务接口
/// </summary>
/// <remarks>
/// 提供大屏的增删改查、发布管理、分享权限、组件管理等功能。
/// 大屏是可视化数据展示的核心载体，支持多种组件类型和灵活的布局配置。
/// </remarks>
public interface IScreenService
{
    #region 大屏 CRUD

    /// <summary>
    /// 获取大屏列表（分页）
    /// </summary>
    /// <param name="query">查询参数，包含分页信息和筛选条件</param>
    /// <returns>分页大屏列表，包含大屏基本信息</returns>
    Task<PageResponse<ScreenListDto>> GetPageListAsync(QueryScreenDto query);

    /// <summary>
    /// 获取大屏详情
    /// </summary>
    /// <param name="id">大屏ID</param>
    /// <returns>大屏详细配置，包含所有组件；大屏不存在返回null</returns>
    Task<ScreenConfigDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建大屏
    /// </summary>
    /// <param name="dto">创建大屏参数，包含名称、描述、样式等</param>
    /// <returns>新创建大屏的ID</returns>
    Task<Guid> CreateAsync(CreateScreenDto dto);

    /// <summary>
    /// 更新大屏信息
    /// </summary>
    /// <param name="dto">更新大屏参数，包含大屏ID和需要更新的字段</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateAsync(UpdateScreenDto dto);

    /// <summary>
    /// 删除大屏（批量）
    /// </summary>
    /// <param name="ids">要删除的大屏ID列表</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteAsync(List<Guid> ids);

    /// <summary>
    /// 复制大屏
    /// </summary>
    /// <param name="id">要复制的大屏ID</param>
    /// <returns>复制后新大屏的完整配置</returns>
    Task<ScreenConfigDto> CopyAsync(Guid id);

    #endregion

    #region 分享权限

    /// <summary>
    /// 分享大屏
    /// </summary>
    /// <param name="dto">分享配置参数，包含大屏ID和权限配置</param>
    /// <returns>影响的行数</returns>
    Task<int> ShareAsync(ShareScreenDto dto);

    /// <summary>
    /// 获取可分享的用户列表
    /// </summary>
    /// <param name="screenId">大屏ID</param>
    /// <returns>可分享的用户列表</returns>
    Task<List<UserDto>> GetShareableUsersAsync(Guid screenId);

    /// <summary>
    /// 获取可分享的角色列表
    /// </summary>
    /// <param name="screenId">大屏ID</param>
    /// <returns>可分享的角色列表</returns>
    Task<List<RoleDto>> GetShareableRolesAsync(Guid screenId);

    #endregion

    #region 发布管理

    /// <summary>
    /// 发布大屏
    /// </summary>
    /// <param name="screenId">要发布的大屏ID</param>
    /// <returns>发布结果，包含发布ID和访问URL</returns>
    Task<PublishResultDto> PublishAsync(Guid screenId);

    /// <summary>
    /// 下架大屏
    /// </summary>
    /// <param name="screenId">大屏ID（与publishId二选一）</param>
    /// <param name="publishId">发布ID（与screenId二选一）</param>
    /// <returns>影响的行数</returns>
    Task<int> UnpublishAsync(Guid? screenId, Guid? publishId);

    /// <summary>
    /// 重新发布大屏
    /// </summary>
    /// <param name="publishId">发布ID</param>
    /// <returns>影响的行数</returns>
    Task<int> RepublishAsync(Guid publishId);

    /// <summary>
    /// 获取已发布的大屏配置（前端兼容格式）
    /// </summary>
    /// <param name="publishId">发布ID</param>
    /// <returns>已发布大屏的完整配置；不存在返回null</returns>
    Task<ScreenFrontendDto?> GetPublishedAsync(Guid publishId);

    /// <summary>
    /// 获取大屏发布状态信息
    /// </summary>
    /// <param name="screenId">大屏ID</param>
    /// <returns>发布状态信息；未发布返回null</returns>
    Task<PublishInfoDto?> GetPublishInfoAsync(Guid screenId);

    /// <summary>
    /// 获取发布记录列表（分页）
    /// </summary>
    /// <param name="query">查询参数，包含分页信息</param>
    /// <returns>分页发布记录列表</returns>
    Task<PageResponse<ScreenPublishDto>> GetPublishListAsync(QueryScreenDto query);

    #endregion

    #region 数据源（Mock模式）

    /// <summary>
    /// 获取数据源选项列表
    /// </summary>
    /// <returns>可用的数据源选项列表</returns>
    Task<List<DatasourceOptionDto>> GetDatasourcesAsync();

    /// <summary>
    /// 执行SQL查询
    /// </summary>
    /// <param name="dto">执行SQL参数，包含数据源ID和SQL语句</param>
    /// <returns>SQL查询结果，包含数据和列信息</returns>
    Task<SqlResultDto> ExecuteSqlAsync(ExecuteSqlDto dto);

    /// <summary>
    /// 验证SQL语法
    /// </summary>
    /// <param name="dto">验证SQL参数，包含数据源ID和SQL语句</param>
    /// <returns>验证结果，包含是否有效和错误信息</returns>
    Task<SqlValidateDto> ValidateSqlAsync(ValidateSqlDto dto);

    #endregion

    #region 配置验证

    /// <summary>
    /// 验证大屏组件配置
    /// </summary>
    /// <param name="componentsJson">组件列表JSON字符串</param>
    /// <returns>验证结果，包含是否有效和错误列表</returns>
    Task<ValidateResultDto> ValidateConfigAsync(string componentsJson);

    /// <summary>
    /// 验证单个组件配置
    /// </summary>
    /// <param name="component">组件配置对象</param>
    /// <returns>验证结果，包含是否有效和错误列表</returns>
    Task<ValidateResultDto> ValidateComponentAsync(object component);

    #endregion

    #region 组件管理

    /// <summary>
    /// 添加组件
    /// </summary>
    /// <param name="dto">创建组件参数，包含大屏ID、组件类型和配置</param>
    /// <returns>新创建组件的ID</returns>
    Task<Guid> AddComponentAsync(CreateComponentDto dto);

    /// <summary>
    /// 更新组件
    /// </summary>
    /// <param name="dto">更新组件参数，包含组件ID和需要更新的字段</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateComponentAsync(UpdateComponentDto dto);

    /// <summary>
    /// 删除组件
    /// </summary>
    /// <param name="componentId">要删除的组件ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteComponentAsync(Guid componentId);

    #endregion
}