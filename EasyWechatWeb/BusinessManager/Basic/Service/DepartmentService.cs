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
/// 部门服务实现类
/// </summary>
/// <remarks>
/// 实现部门相关的业务逻辑，包括部门的增删改查、树形结构构建等功能。
/// 继承自<see cref="BaseService{Department}"/>，使用SqlSugar进行数据库操作。
/// 部门采用树形结构，通过ParentId关联上级部门。
/// </remarks>
public class DepartmentService : BaseService<Department>, IDepartmentService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<DepartmentService> _logger { get; set; } = null!;

    /// <summary>
    /// 获取部门树形结构
    /// </summary>
    /// <returns>树形部门列表</returns>
    /// <remarks>
    /// 查询所有状态为有效的部门，按Sort升序排序。
    /// 通过BuildTree方法递归构建树形结构。
    /// </remarks>
    public async Task<List<DepartmentDto>> GetTreeAsync()
    {
        var list = await _db.Queryable<Department>()
            .Where(d => d.Status == 1)
            .OrderBy(d => d.Sort)
            .ToListAsync();

        var dtoList = list.Adapt<List<DepartmentDto>>();

        // 统计每个部门的成员数量
        foreach (var dto in dtoList)
        {
            dto.MemberCount = await _db.Queryable<User>()
                .Where(u => u.DepartmentId == dto.Id)
                .CountAsync();
        }

        return BuildTree(dtoList, null);
    }

    /// <summary>
    /// 构建部门树形结构
    /// </summary>
    /// <param name="departments">扁平的部门列表</param>
    /// <param name="parentId">上级部门ID，null表示顶层</param>
    /// <returns>树形结构的部门列表</returns>
    /// <remarks>
    /// 递归构建树形结构：
    /// 1. 筛选指定上级的所有部门
    /// 2. 对每个部门递归查找其子部门
    /// 3. 将子部门设置到Children属性
    /// </remarks>
    private List<DepartmentDto> BuildTree(List<DepartmentDto> departments, Guid? parentId)
    {
        var result = departments.Where(d => d.ParentId == parentId).ToList();
        foreach (var dept in result)
        {
            dept.Children = BuildTree(departments, dept.Id);
        }
        return result;
    }

    /// <summary>
    /// 获取部门详情
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <returns>部门DTO；部门不存在返回null</returns>
    public new async Task<DepartmentDto?> GetByIdAsync(Guid id)
    {
        var dept = await base.GetByIdAsync(id);
        if (dept == null)
        {
            return null;
        }

        var dto = dept.Adapt<DepartmentDto>();

        // 统计成员数量
        dto.MemberCount = await _db.Queryable<User>()
            .Where(u => u.DepartmentId == id)
            .CountAsync();

        return dto;
    }

    /// <summary>
    /// 创建部门
    /// </summary>
    /// <param name="dto">新增部门参数</param>
    /// <returns>新创建部门的ID</returns>
    /// <remarks>
    /// 创建流程：
    /// 1. 检查部门编码是否重复
    /// 2. 计算层级和完整路径
    /// 3. 如果指定了主管，获取主管姓名
    /// 4. 插入记录并返回ID
    /// </remarks>
    public async Task<Guid> CreateAsync(AddDepartmentDto dto)
    {
        // 检查编码是否重复
        if (!string.IsNullOrEmpty(dto.Code))
        {
            var exists = await GetFirstAsync(d => d.Code == dto.Code);
            if (exists != null)
            {
                throw BusinessException.BadRequest($"部门编码 '{dto.Code}' 已存在");
            }
        }

        var dept = dto.Adapt<Department>();
        dept.CreateTime = DateTime.Now;

        // 计算层级和完整路径
        if (dto.ParentId.HasValue && dto.ParentId.Value != Guid.Empty)
        {
            var parent = await base.GetByIdAsync(dto.ParentId.Value);
            if (parent != null)
            {
                dept.Level = parent.Level + 1;
                dept.FullPath = string.IsNullOrEmpty(parent.FullPath)
                    ? parent.Name
                    : $"{parent.FullPath}/{dto.Name}";
            }
            else
            {
                dept.Level = 1;
                dept.FullPath = dto.Name;
            }
        }
        else
        {
            dept.Level = 1;
            dept.FullPath = dto.Name;
        }

        // 获取主管姓名
        if (dto.ManagerId.HasValue)
        {
            var manager = await _db.Queryable<User>()
                .Where(u => u.Id == dto.ManagerId.Value)
                .FirstAsync();
            dept.LeaderName = manager?.RealName;
        }

        return await InsertAsync(dept);
    }

    /// <summary>
    /// 更新部门信息
    /// </summary>
    /// <param name="dto">更新部门参数</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 更新流程：
    /// 1. 验证部门是否存在
    /// 2. 检查编码是否重复
    /// 3. 重新计算层级和完整路径（如果上级部门改变）
    /// 4. 更新主管姓名
    /// 5. 更新记录
    /// </remarks>
    public async Task<int> UpdateAsync(UpdateDepartmentDto dto)
    {
        var dept = await base.GetByIdAsync(dto.Id);
        if (dept == null)
        {
            throw BusinessException.NotFound("部门不存在");
        }

        // 检查编码是否重复
        if (!string.IsNullOrEmpty(dto.Code) && dto.Code != dept.Code)
        {
            var exists = await GetFirstAsync(d => d.Code == dto.Code && d.Id != dto.Id);
            if (exists != null)
            {
                throw BusinessException.BadRequest($"部门编码 '{dto.Code}' 已存在");
            }
        }

        // 检查是否将部门移动到自己的子部门下
        if (dto.ParentId.HasValue && dto.ParentId.Value != Guid.Empty)
        {
            if (dto.ParentId.Value == dto.Id)
            {
                throw BusinessException.BadRequest("不能将部门设置为自己的上级部门");
            }
            // 检查是否移动到子孙部门
            var children = await GetChildrenIdsAsync(dto.Id);
            if (children.Contains(dto.ParentId.Value))
            {
                throw BusinessException.BadRequest("不能将部门移动到自己的子部门下");
            }
        }

        dept.Name = dto.Name;
        dept.Code = dto.Code;
        dept.ParentId = dto.ParentId;
        dept.ManagerId = dto.ManagerId;
        dept.Phone = dto.Phone;
        dept.Email = dto.Email;
        dept.Description = dto.Description;
        dept.Sort = dto.Sort;
        dept.Status = dto.Status;
        dept.UpdateTime = DateTime.Now;

        // 重新计算层级和完整路径
        if (dto.ParentId.HasValue && dto.ParentId.Value != Guid.Empty)
        {
            var parent = await base.GetByIdAsync(dto.ParentId.Value);
            if (parent != null)
            {
                dept.Level = parent.Level + 1;
                dept.FullPath = string.IsNullOrEmpty(parent.FullPath)
                    ? parent.Name
                    : $"{parent.FullPath}/{dto.Name}";
            }
        }
        else
        {
            dept.Level = 1;
            dept.FullPath = dto.Name;
        }

        // 更新主管姓名
        if (dto.ManagerId.HasValue)
        {
            var manager = await _db.Queryable<User>()
                .Where(u => u.Id == dto.ManagerId.Value)
                .FirstAsync();
            dept.LeaderName = manager?.RealName;
        }
        else
        {
            dept.LeaderName = null;
        }

        return await base.UpdateAsync(dept);
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    /// <param name="id">要删除的部门ID</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 删除前检查：
    /// 1. 部门是否存在
    /// 2. 是否存在子部门
    /// 3. 是否存在成员
    /// 存在子部门或成员时不允许删除
    /// </remarks>
    public new async Task<int> DeleteAsync(Guid id)
    {
        var dept = await base.GetByIdAsync(id);
        if (dept == null)
        {
            throw BusinessException.NotFound("部门不存在");
        }

        // 检查是否有子部门
        var children = await GetFirstAsync(d => d.ParentId == id);
        if (children != null)
        {
            throw BusinessException.BadRequest("存在子部门，无法删除");
        }

        // 检查是否有成员
        var memberCount = await _db.Queryable<User>()
            .Where(u => u.DepartmentId == id)
            .CountAsync();
        if (memberCount > 0)
        {
            throw BusinessException.BadRequest($"部门下有 {memberCount} 个成员，无法删除");
        }

        return await base.DeleteAsync(id);
    }

    /// <summary>
    /// 获取部门成员列表
    /// </summary>
    /// <param name="deptId">部门ID</param>
    /// <returns>部门下的用户列表</returns>
    /// <remarks>
    /// 返回该部门下的所有用户基本信息
    /// </remarks>
    public async Task<List<UserDto>> GetDepartmentUsersAsync(Guid deptId)
    {
        var users = await _db.Queryable<User>()
            .Where(u => u.DepartmentId == deptId && u.Status == 1)
            .ToListAsync();

        var userDtos = users.Adapt<List<UserDto>>();

        // 获取用户角色
        foreach (var userDto in userDtos)
        {
            var roleData = await _db.Queryable<UserRole, Role>((ur, r) => new JoinQueryInfos(JoinType.Left, ur.RoleId == r.Id))
                .Where((ur, r) => ur.UserId == userDto.Id)
                .Select((ur, r) => new { RoleName = r.RoleName, RoleId = r.Id })
                .ToListAsync();
            userDto.Roles = roleData.Select(x => x.RoleName).ToList();
            userDto.RoleIds = roleData.Select(x => x.RoleId).ToList();
        }

        return userDtos;
    }

    /// <summary>
    /// 获取所有子部门ID（递归）
    /// </summary>
    /// <param name="parentId">上级部门ID</param>
    /// <returns>所有子部门ID列表</returns>
    private async Task<List<Guid>> GetChildrenIdsAsync(Guid parentId)
    {
        var children = await _db.Queryable<Department>()
            .Where(d => d.ParentId == parentId)
            .Select(d => d.Id)
            .ToListAsync();

        var result = new List<Guid>(children);
        foreach (var childId in children)
        {
            var grandChildren = await GetChildrenIdsAsync(childId);
            result.AddRange(grandChildren);
        }

        return result;
    }
}