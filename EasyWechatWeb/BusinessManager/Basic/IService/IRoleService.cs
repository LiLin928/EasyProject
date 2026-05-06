using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Basic.IService;

/// <summary>
/// 角色服务接口
/// </summary>
/// <remarks>
/// 提供角色的增删改查、角色权限分配等功能。
/// 角色是权限管理的核心，用户通过角色关联获得菜单访问权限。
/// </remarks>
public interface IRoleService
{
    /// <summary>
    /// 获取所有角色列表
    /// </summary>
    /// <returns>角色列表，包含角色ID、名称、编码、描述和状态</returns>
    Task<List<RoleDto>> GetListAsync();

    /// <summary>
    /// 获取角色列表（分页查询）
    /// </summary>
    /// <param name="dto">查询参数，包含分页、角色名称、状态、排序等</param>
    /// <returns>分页角色列表</returns>
    Task<PageResponse<RoleDto>> GetPageListAsync(QueryRoleDto dto);

    /// <summary>
    /// 获取角色详情
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>角色详细信息；角色不存在返回null</returns>
    Task<RoleDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 添加角色
    /// </summary>
    /// <param name="dto">添加角色参数，包含角色名称、编码和描述</param>
    /// <returns>新创建角色的ID</returns>
    Task<Guid> AddAsync(AddRoleDto dto);

    /// <summary>
    /// 更新角色信息
    /// </summary>
    /// <param name="dto">更新角色参数，包含角色ID和需要更新的字段</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateAsync(UpdateRoleDto dto);

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="id">要删除的角色ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 批量删除角色
    /// </summary>
    /// <param name="ids">要删除的角色ID列表</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteBatchAsync(List<Guid> ids);

    /// <summary>
    /// 为角色分配用户
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="userIds">要分配的用户ID列表</param>
    /// <returns>影响的行数</returns>
    Task<int> AssignUsersAsync(Guid roleId, List<Guid> userIds);

    /// <summary>
    /// 获取角色的菜单ID列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>菜单ID列表</returns>
    Task<List<Guid>> GetMenuIdsAsync(Guid roleId);

    /// <summary>
    /// 更新角色的菜单权限
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="menuIds">菜单ID列表</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateMenuAsync(Guid roleId, List<Guid> menuIds);
}