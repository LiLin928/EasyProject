using EasyWeChatModels.Dto;

namespace BusinessManager.Basic.IService;

/// <summary>
/// 菜单服务接口
/// </summary>
/// <remarks>
/// 提供菜单的增删改查、树形结构查询、角色权限分配等功能。
/// 菜单采用树形结构组织，支持无限层级。
/// 用户通过角色关联获得菜单访问权限。
/// </remarks>
public interface IMenuService
{
    /// <summary>
    /// 获取菜单列表（树形结构）
    /// </summary>
    /// <returns>树形菜单列表，每个节点包含子节点集合</returns>
    Task<List<MenuDto>> GetTreeAsync();

    /// <summary>
    /// 获取用户菜单（树形结构）
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户有权限访问的树形菜单列表</returns>
    Task<List<MenuDto>> GetUserMenuTreeAsync(Guid userId);

    /// <summary>
    /// 获取菜单详情
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>菜单详细信息；菜单不存在返回null</returns>
    Task<MenuDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 添加菜单
    /// </summary>
    /// <param name="dto">添加菜单参数，包含菜单名称、编码、路径、图标、父级ID和排序</param>
    /// <returns>新创建菜单的ID</returns>
    Task<Guid> AddAsync(AddMenuDto dto);

    /// <summary>
    /// 更新菜单信息
    /// </summary>
    /// <param name="dto">更新菜单参数，包含菜单ID和需要更新的字段</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateAsync(UpdateMenuDto dto);

    /// <summary>
    /// 删除菜单
    /// </summary>
    /// <param name="id">要删除的菜单ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 为菜单分配角色权限
    /// </summary>
    /// <param name="menuId">菜单ID</param>
    /// <param name="roleIds">要分配的角色ID列表</param>
    /// <returns>影响的行数</returns>
    Task<int> AssignRolesAsync(Guid menuId, List<Guid> roleIds);
}