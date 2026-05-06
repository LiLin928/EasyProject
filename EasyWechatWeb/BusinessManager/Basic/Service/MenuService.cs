using BusinessManager.Basic.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace BusinessManager.Basic.Service;

/// <summary>
/// 菜单服务实现类
/// </summary>
/// <remarks>
/// 实现菜单相关的业务逻辑，包括菜单的增删改查、树形结构构建、权限分配等功能。
/// 继承自<see cref="BaseService{Menu}"/>，使用SqlSugar进行数据库操作。
/// 菜单采用树形结构，通过ParentId关联父级菜单。
/// </remarks>
public class MenuService : BaseService<Menu>, IMenuService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<MenuService> _logger { get; set; } = null!;

    /// <summary>
    /// 获取菜单列表（树形结构）
    /// </summary>
    /// <returns>树形菜单DTO列表</returns>
    /// <remarks>
    /// 查询所有状态为有效(Status=1)的菜单。
    /// 结果按Sort字段升序排序。
    /// 通过BuildTree方法递归构建树形结构，顶层菜单ParentId为Guid.Empty。
    /// </remarks>
    public async Task<List<MenuDto>> GetTreeAsync()
    {
        var list = await _db.Queryable<Menu>()
            .Where(m => m.Status == 1)
            .OrderBy(m => m.Sort)
            .ToListAsync();

        return BuildTree(list.Adapt<List<MenuDto>>(), Guid.Empty);
    }

    /// <summary>
    /// 获取用户菜单（树形结构）
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户有权限访问的树形菜单DTO列表</returns>
    /// <remarks>
    /// 查询流程：
    /// <list type="number">
    ///     <item>通过UserRole表获取用户关联的角色ID列表</item>
    ///     <item>通过RoleMenu表获取这些角色关联的菜单ID列表</item>
    ///     <item>查询这些菜单的详细信息</item>
    ///     <item>构建树形结构返回</item>
    /// </list>
    /// 如果用户无角色或角色无菜单权限，返回空列表。
    /// </remarks>
    public async Task<List<MenuDto>> GetUserMenuTreeAsync(Guid userId)
    {
        // 获取用户角色
        var roleIds = await _db.Queryable<UserRole>()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (roleIds.Count == 0)
        {
            return new List<MenuDto>();
        }

        // 获取角色菜单
        var menuIds = await _db.Queryable<RoleMenu>()
            .Where(rm => roleIds.Contains(rm.RoleId))
            .Select(rm => rm.MenuId)
            .Distinct()
            .ToListAsync();

        if (menuIds.Count == 0)
        {
            return new List<MenuDto>();
        }

        var menus = await _db.Queryable<Menu>()
            .Where(m => menuIds.Contains(m.Id) && m.Status == 1)
            .OrderBy(m => m.Sort)
            .ToListAsync();

        return BuildTree(menus.Adapt<List<MenuDto>>(), Guid.Empty);
    }

    /// <summary>
    /// 构建菜单树形结构
    /// </summary>
    /// <param name="menus">扁平的菜单列表</param>
    /// <param name="parentId">父级菜单ID，Guid.Empty表示顶层</param>
    /// <returns>树形结构的菜单列表</returns>
    /// <remarks>
    /// 递归构建树形结构：
    /// <list type="number">
    ///     <item>筛选指定父级的所有菜单</item>
    ///     <item>对每个菜单递归查找其子菜单</item>
    ///     <item>将子菜单设置到Children属性</item>
    /// </list>
    /// 顶层菜单的ParentId为Guid.Empty。
    /// </remarks>
    private List<MenuDto> BuildTree(List<MenuDto> menus, Guid parentId)
    {
        var result = menus.Where(m => m.ParentId == parentId).ToList();
        foreach (var menu in result)
        {
            menu.Children = BuildTree(menus, menu.Id);
        }
        return result;
    }

    /// <summary>
    /// 获取菜单详情
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>菜单DTO；菜单不存在返回null</returns>
    /// <remarks>
    /// 查询指定ID的菜单记录并转换为DTO。
    /// 使用Mapster进行实体到DTO的映射。
    /// </remarks>
    public new async Task<MenuDto?> GetByIdAsync(Guid id)
    {
        var menu = await base.GetByIdAsync(id);
        return menu?.Adapt<MenuDto>();
    }

    /// <summary>
    /// 添加菜单
    /// </summary>
    /// <param name="dto">添加菜单参数DTO</param>
    /// <returns>新创建菜单的ID</returns>
    /// <remarks>
    /// 添加流程：
    /// <list type="number">
    ///     <item>使用Mapster映射DTO到实体</item>
    ///     <item>设置创建时间为当前时间</item>
    ///     <item>设置默认状态为1（有效）</item>
    ///     <item>插入记录并返回ID</item>
    /// </list>
    /// ParentId为Guid.Empty表示顶层菜单，否则表示子菜单。
    /// </remarks>
    public async Task<Guid> AddAsync(AddMenuDto dto)
    {
        var menu = dto.Adapt<Menu>();
        menu.CreateTime = DateTime.Now;
        menu.Status = 1;

        return await InsertAsync(menu);
    }

    /// <summary>
    /// 更新菜单信息
    /// </summary>
    /// <param name="dto">更新菜单参数DTO</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="BusinessException">
    /// 菜单不存在时抛出NotFound异常
    /// </exception>
    /// <remarks>
    /// 支持部分更新，只更新DTO中非空的字段。
    /// 可更新字段包括：菜单名称、菜单编码、路径、图标、排序、状态。
    /// </remarks>
    public async Task<int> UpdateAsync(UpdateMenuDto dto)
    {
        var menu = await base.GetByIdAsync(dto.Id);
        if (menu == null)
        {
            throw BusinessException.NotFound("菜单不存在");
        }

        if (dto.ParentId.HasValue) menu.ParentId = dto.ParentId.Value;
        if (dto.MenuName != null) menu.MenuName = dto.MenuName;
        if (dto.MenuCode != null) menu.MenuCode = dto.MenuCode;
        if (dto.Path != null) menu.Path = dto.Path;
        if (dto.Component != null) menu.Component = dto.Component;
        if (dto.Icon != null) menu.Icon = dto.Icon;
        if (dto.Sort.HasValue) menu.Sort = dto.Sort.Value;
        if (dto.Status.HasValue) menu.Status = dto.Status.Value;
        if (dto.Hidden.HasValue) menu.Hidden = dto.Hidden.Value;
        if (dto.Affix.HasValue) menu.Affix = dto.Affix.Value;
        menu.UpdateTime = DateTime.Now;

        return await base.UpdateAsync(menu);
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    /// <param name="id">要删除的菜单ID</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="BusinessException">
    /// 菜单不存在时抛出NotFound异常；
    /// 存在子菜单时抛出BadRequest异常
    /// </exception>
    /// <remarks>
    /// 删除前置检查：
    /// <list type="number">
    ///     <item>检查菜单是否存在</item>
    ///     <item>检查是否存在子菜单（ParentId等于当前菜单ID）</item>
    /// </list>
    /// 删除时会级联删除RoleMenu表中的关联记录。
    /// 存在子菜单时不允许删除，需要先删除子菜单。
    /// </remarks>
    public new async Task<int> DeleteAsync(Guid id)
    {
        var menu = await base.GetByIdAsync(id);
        if (menu == null)
        {
            throw BusinessException.NotFound("菜单不存在");
        }

        // 检查是否有子菜单
        var children = await GetFirstAsync(m => m.ParentId == id);
        if (children != null)
        {
            throw BusinessException.BadRequest("存在子菜单，无法删除");
        }

        // 删除角色菜单关联
        await _db.Deleteable<RoleMenu>().Where(rm => rm.MenuId == id).ExecuteCommandAsync();

        return await base.DeleteAsync(id);
    }

    /// <summary>
    /// 为菜单分配角色权限
    /// </summary>
    /// <param name="menuId">菜单ID</param>
    /// <param name="roleIds">要分配的角色ID列表</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="BusinessException">
    /// 菜单不存在时抛出NotFound异常
    /// </exception>
    /// <remarks>
    /// 分配流程：
    /// <list type="number">
    ///     <item>验证菜单是否存在</item>
    ///     <item>删除该菜单原有的所有角色分配</item>
    ///     <item>批量插入新的角色菜单关联记录</item>
    /// </list>
    /// 每次调用会完全覆盖之前的分配关系。
    /// 如果roleIds为空列表，则只删除不添加（相当于清空权限）。
    /// </remarks>
    public async Task<int> AssignRolesAsync(Guid menuId, List<Guid> roleIds)
    {
        var menu = await base.GetByIdAsync(menuId);
        if (menu == null)
        {
            throw BusinessException.NotFound("菜单不存在");
        }

        // 删除原有分配
        await _db.Deleteable<RoleMenu>().Where(rm => rm.MenuId == menuId).ExecuteCommandAsync();

        // 添加新分配
        if (roleIds.Count > 0)
        {
            var roleMenus = roleIds.Select(roleId => new RoleMenu
            {
                RoleId = roleId,
                MenuId = menuId,
                CreateTime = DateTime.Now
            }).ToList();

            return await _db.Insertable(roleMenus).ExecuteCommandAsync();
        }

        return 0;
    }
}