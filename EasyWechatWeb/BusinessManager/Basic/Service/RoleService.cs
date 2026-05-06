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
/// 角色服务实现类
/// </summary>
/// <remarks>
/// 实现角色相关的业务逻辑，包括角色的增删改查、用户分配等功能。
/// 继承自<see cref="BaseService{Role}"/>，使用SqlSugar进行数据库操作。
/// 角色是权限体系的核心，用户通过角色获得菜单访问权限。
/// </remarks>
public class RoleService : BaseService<Role>, IRoleService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<RoleService> _logger { get; set; } = null!;

    /// <summary>
    /// 获取所有角色列表
    /// </summary>
    /// <returns>角色DTO列表</returns>
    /// <remarks>
    /// 查询所有角色记录并转换为DTO。
    /// 使用Mapster进行实体到DTO的映射。
    /// 不进行分页，返回全部角色。
    /// </remarks>
    public new async Task<List<RoleDto>> GetListAsync()
    {
        var list = await base.GetListAsync();
        return list.Adapt<List<RoleDto>>();
    }

    /// <summary>
    /// 获取角色列表（分页查询）
    /// </summary>
    /// <param name="dto">查询参数，包含分页、角色名称、状态、排序等</param>
    /// <returns>分页角色列表</returns>
    public async Task<PageResponse<RoleDto>> GetPageListAsync(QueryRoleDto dto)
    {
        var total = new RefAsync<int>();
        var query = _db.Queryable<Role>()
            .WhereIF(!string.IsNullOrEmpty(dto.RoleName), r => r.RoleName.Contains(dto.RoleName!))
            .WhereIF(!string.IsNullOrEmpty(dto.RoleCode), r => r.RoleCode.Contains(dto.RoleCode!))
            .WhereIF(dto.Status.HasValue, r => r.Status == dto.Status!.Value);

        // 处理排序
        if (!string.IsNullOrEmpty(dto.SortField) && !string.IsNullOrEmpty(dto.SortOrder))
        {
            var isAsc = dto.SortOrder == "ascending";
            query = dto.SortField switch
            {
                "roleName" => isAsc ? query.OrderBy(r => r.RoleName) : query.OrderByDescending(r => r.RoleName),
                "createTime" => isAsc ? query.OrderBy(r => r.CreateTime) : query.OrderByDescending(r => r.CreateTime),
                _ => query.OrderByDescending(r => r.CreateTime)
            };
        }
        else
        {
            query = query.OrderByDescending(r => r.CreateTime);
        }

        var list = await query.ToPageListAsync(dto.PageIndex, dto.PageSize, total);
        var roleDtos = list.Adapt<List<RoleDto>>();

        return PageResponse<RoleDto>.Create(roleDtos, total.Value, dto.PageIndex, dto.PageSize);
    }

    /// <summary>
    /// 获取角色详情
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>角色DTO；角色不存在返回null</returns>
    /// <remarks>
    /// 查询指定ID的角色记录并转换为DTO。
    /// 使用Mapster进行实体到DTO的映射。
    /// </remarks>
    public new async Task<RoleDto?> GetByIdAsync(Guid id)
    {
        var role = await base.GetByIdAsync(id);
        return role?.Adapt<RoleDto>();
    }

    /// <summary>
    /// 添加角色
    /// </summary>
    /// <param name="dto">添加角色参数DTO</param>
    /// <returns>新创建角色的ID</returns>
    /// <exception cref="BusinessException">
    /// 角色名称或编码已存在时抛出BadRequest异常
    /// </exception>
    /// <remarks>
    /// 添加流程：
    /// <list type="number">
    ///     <item>检查角色名称和编码的唯一性</item>
    ///     <item>映射DTO到实体</item>
    ///     <item>设置创建时间和默认状态为1</item>
    ///     <item>插入记录并返回ID</item>
    /// </list>
    /// </remarks>
    public async Task<Guid> AddAsync(AddRoleDto dto)
    {
        var exists = await GetFirstAsync(r => r.RoleName == dto.RoleName || r.RoleCode == dto.RoleCode);
        if (exists != null)
        {
            throw BusinessException.BadRequest("角色名称或编码已存在");
        }

        var role = dto.Adapt<Role>();
        role.CreateTime = DateTime.Now;
        role.Status = 1;

        return await InsertAsync(role);
    }

    /// <summary>
    /// 更新角色信息
    /// </summary>
    /// <param name="dto">更新角色参数DTO</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="BusinessException">
    /// 角色不存在时抛出NotFound异常
    /// </exception>
    /// <remarks>
    /// 支持部分更新，只更新DTO中非空的字段。
    /// 可更新字段包括：角色名称、角色编码、描述、状态。
    /// </remarks>
    public async Task<int> UpdateAsync(UpdateRoleDto dto)
    {
        var role = await base.GetByIdAsync(dto.Id);
        if (role == null)
        {
            throw BusinessException.NotFound("角色不存在");
        }

        if (dto.RoleName != null) role.RoleName = dto.RoleName;
        if (dto.RoleCode != null) role.RoleCode = dto.RoleCode;
        if (dto.Description != null) role.Description = dto.Description;
        if (dto.Status.HasValue) role.Status = dto.Status.Value;

        return await base.UpdateAsync(role);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="id">要删除的角色ID</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="BusinessException">
    /// 角色不存在时抛出NotFound异常
    /// </exception>
    /// <remarks>
    /// 删除角色时会级联删除：
    /// <list type="bullet">
    ///     <item>UserRole表中的用户角色关联记录</item>
    ///     <item>RoleMenu表中的角色菜单关联记录</item>
    /// </list>
    /// 这是物理删除，数据将永久移除。
    /// </remarks>
    public new async Task<int> DeleteAsync(Guid id)
    {
        var role = await base.GetByIdAsync(id);
        if (role == null)
        {
            throw BusinessException.NotFound("角色不存在");
        }

        // 删除用户角色关联
        await _db.Deleteable<UserRole>().Where(ur => ur.RoleId == id).ExecuteCommandAsync();
        // 删除角色菜单关联
        await _db.Deleteable<RoleMenu>().Where(rm => rm.RoleId == id).ExecuteCommandAsync();

        return await base.DeleteAsync(id);
    }

    /// <summary>
    /// 批量删除角色
    /// </summary>
    /// <param name="ids">要删除的角色ID列表</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 批量删除角色，会级联删除关联的UserRole和RoleMenu记录。
    /// </remarks>
    public async Task<int> DeleteBatchAsync(List<Guid> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return 0;
        }

        // 删除用户角色关联
        await _db.Deleteable<UserRole>().Where(ur => ids.Contains(ur.RoleId)).ExecuteCommandAsync();
        // 删除角色菜单关联
        await _db.Deleteable<RoleMenu>().Where(rm => ids.Contains(rm.RoleId)).ExecuteCommandAsync();
        // 删除角色
        return await _db.Deleteable<Role>().Where(r => ids.Contains(r.Id)).ExecuteCommandAsync();
    }

    /// <summary>
    /// 为角色分配用户
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="userIds">要分配的用户ID列表</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="BusinessException">
    /// 角色不存在时抛出NotFound异常
    /// </exception>
    /// <remarks>
    /// 分配流程：
    /// <list type="number">
    ///     <item>验证角色是否存在</item>
    ///     <item>删除该角色原有的所有用户分配</item>
    ///     <item>批量插入新的用户角色关联记录</item>
    /// </list>
    /// 每次调用会完全覆盖之前的分配关系。
    /// 如果userIds为空列表，则只删除不添加（相当于清空分配）。
    /// </remarks>
    public async Task<int> AssignUsersAsync(Guid roleId, List<Guid> userIds)
    {
        var role = await base.GetByIdAsync(roleId);
        if (role == null)
        {
            throw BusinessException.NotFound("角色不存在");
        }

        // 删除原有分配
        await _db.Deleteable<UserRole>().Where(ur => ur.RoleId == roleId).ExecuteCommandAsync();

        // 添加新分配
        if (userIds.Count > 0)
        {
            var userRoles = userIds.Select(userId => new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                CreateTime = DateTime.Now
            }).ToList();

            return await _db.Insertable(userRoles).ExecuteCommandAsync();
        }

        return 0;
    }

    /// <summary>
    /// 获取角色的菜单ID列表
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>菜单ID列表</returns>
    public async Task<List<Guid>> GetMenuIdsAsync(Guid roleId)
    {
        var menuIds = await _db.Queryable<RoleMenu>()
            .Where(rm => rm.RoleId == roleId)
            .Select(rm => rm.MenuId)
            .ToListAsync();
        return menuIds;
    }

    /// <summary>
    /// 更新角色的菜单权限
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="menuIds">菜单ID列表</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="BusinessException">
    /// 角色不存在时抛出NotFound异常
    /// </exception>
    public async Task<int> UpdateMenuAsync(Guid roleId, List<Guid> menuIds)
    {
        var role = await base.GetByIdAsync(roleId);
        if (role == null)
        {
            throw BusinessException.NotFound("角色不存在");
        }

        // 删除原有权限
        await _db.Deleteable<RoleMenu>().Where(rm => rm.RoleId == roleId).ExecuteCommandAsync();

        // 添加新权限
        if (menuIds.Count > 0)
        {
            var roleMenus = menuIds.Select(menuId => new RoleMenu
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