using EasyWeChatModels.Dto;

namespace BusinessManager.Basic.IService;

/// <summary>
/// 部门服务接口
/// </summary>
/// <remarks>
/// 提供部门的增删改查、树形结构查询、部门成员查询等功能。
/// 部门采用树形结构组织，支持无限层级。
/// </remarks>
public interface IDepartmentService
{
    /// <summary>
    /// 获取部门树形结构
    /// </summary>
    /// <returns>树形部门列表，每个节点包含子节点集合</returns>
    Task<List<DepartmentDto>> GetTreeAsync();

    /// <summary>
    /// 获取部门详情
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <returns>部门详细信息；部门不存在返回null</returns>
    Task<DepartmentDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建部门
    /// </summary>
    /// <param name="dto">新增部门参数</param>
    /// <returns>新创建部门的ID</returns>
    Task<Guid> CreateAsync(AddDepartmentDto dto);

    /// <summary>
    /// 更新部门信息
    /// </summary>
    /// <param name="dto">更新部门参数</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateAsync(UpdateDepartmentDto dto);

    /// <summary>
    /// 删除部门
    /// </summary>
    /// <param name="id">要删除的部门ID</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="Exception">存在子部门或成员时不能删除</exception>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 获取部门成员列表
    /// </summary>
    /// <param name="deptId">部门ID</param>
    /// <returns>部门下的用户列表</returns>
    Task<List<UserDto>> GetDepartmentUsersAsync(Guid deptId);
}