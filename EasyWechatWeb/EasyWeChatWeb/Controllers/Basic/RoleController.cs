using BusinessManager.Basic.IService;
using BusinessManager.Basic.Service;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Basic;

/// <summary>
/// 角色控制器
/// 提供角色的增删改查、用户分配等功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleController : BaseController
{
    /// <summary>
    /// 角色服务接口
    /// </summary>
    public IRoleService _roleService { get; set; } = null!;
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<RoleController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取角色列表（分页）
    /// </summary>
    /// <param name="dto">查询参数，包含分页、角色名称、状态、排序等</param>
    /// <returns>分页后的角色列表</returns>
    /// <response code="200">成功获取角色列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 支持多条件查询：
    /// - roleName: 角色名称模糊匹配
    /// - roleCode: 角色编码模糊匹配
    /// - status: 状态精确匹配（1-启用，0-禁用）
    /// - sortField/sortOrder: 支持按 roleName、createTime 排序
    /// 返回结果默认按创建时间倒序排列。
    /// </remarks>
    /// <example>
    /// POST /api/role/list
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "pageIndex": 1,
    ///     "pageSize": 10,
    ///     "roleName": "管理员",
    ///     "roleCode": "admin",
    ///     "status": 1
    /// }
    /// </example>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<RoleDto>>), 200)]
    public async Task<ApiResponse<PageResponse<RoleDto>>> GetList([FromBody] QueryRoleDto dto)
    {
        try
        {
            var result = await _roleService.GetPageListAsync(dto);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取角色列表失败");
            return Error<PageResponse<RoleDto>>("获取角色列表失败");
        }
    }

    /// <summary>
    /// 获取角色详情
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>角色详细信息</returns>
    /// <response code="200">成功获取角色详情</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">角色不存在</response>
    /// <remarks>
    /// 根据角色ID获取角色的详细信息，包括角色名称、描述、权限等。
    /// </remarks>
    /// <example>
    /// GET /api/role/detail/1
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), 200)]
    public async Task<ApiResponse<RoleDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _roleService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<RoleDto>("角色不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取角色详情失败: {Id}", id);
            return Error<RoleDto>("获取角色详情失败");
        }
    }

    /// <summary>
    /// 添加角色
    /// </summary>
    /// <param name="dto">添加角色请求参数，包含角色名称、描述等信息</param>
    /// <returns>新创建的角色ID</returns>
    /// <response code="200">添加成功，返回新角色ID</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 创建新角色，角色名称必须唯一。
    /// 角色用于控制用户的权限和菜单访问。
    /// </remarks>
    /// <example>
    /// POST /api/role/add
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "roleName": "管理员",
    ///     "description": "系统管理员角色"
    /// }
    /// </example>
    [HttpPost("add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Add([FromBody] AddRoleDto dto)
    {
        try
        {
            var id = await _roleService.AddAsync(dto);
            return Success(id, "添加成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加角色失败");
            return Error<Guid>("添加角色失败");
        }
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="dto">更新角色请求参数，包含角色ID和需要更新的字段</param>
    /// <returns>更新的角色ID</returns>
    /// <response code="200">更新成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 更新角色信息，只更新请求中提供的字段。
    /// 角色名称不能重复。
    /// </remarks>
    /// <example>
    /// PUT /api/role/update
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "id": 1,
    ///     "roleName": "新角色名",
    ///     "description": "更新后的描述"
    /// }
    /// </example>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateRoleDto dto)
    {
        try
        {
            var result = await _roleService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新角色失败: {Id}", dto.Id);
            return Error<int>("更新角色失败");
        }
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="id">要删除的角色ID</param>
    /// <returns>删除的角色ID</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 删除角色，删除前需确保该角色没有关联的用户。
    /// 系统预设角色不能删除。
    /// </remarks>
    /// <example>
    /// POST /api/role/delete
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    /// </example>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] Guid id)
    {
        try
        {
            var result = await _roleService.DeleteAsync(id);
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除角色失败: {Id}", id);
            return Error<int>("删除角色失败");
        }
    }

    /// <summary>
    /// 批量删除角色
    /// </summary>
    /// <param name="ids">要删除的角色ID列表</param>
    /// <returns>删除的角色数量</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 批量删除角色，删除前需确保角色没有关联的用户。
    /// </remarks>
    /// <example>
    /// POST /api/role/delete-batch
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// ["3fa85f64-5717-4562-b3fc-2c963f66afa6", "4fa85f64-5717-4562-b3fc-2c963f66afa7"]
    /// </example>
    [HttpPost("delete-batch")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> DeleteBatch([FromBody] List<Guid> ids)
    {
        try
        {
            var result = await _roleService.DeleteBatchAsync(ids);
            return Success(result, "批量删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量删除角色失败");
            return Error<int>("批量删除角色失败");
        }
    }

    /// <summary>
    /// 分配用户到角色
    /// </summary>
    /// <param name="dto">分配用户请求参数，包含角色ID和用户ID列表</param>
    /// <returns>分配的用户数量</returns>
    /// <response code="200">分配成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 将指定的用户分配到角色。
    /// 用户将获得该角色对应的所有权限。
    /// 一个用户可以拥有多个角色。
    /// </remarks>
    /// <example>
    /// POST /api/role/assign-users
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "roleId": 1,
    ///     "userIds": [1, 2, 3]
    /// }
    /// </example>
    [HttpPost("assign-users")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> AssignUsers([FromBody] AssignUsersDto dto)
    {
        try
        {
            var result = await _roleService.AssignUsersAsync(dto.RoleId, dto.UserIds);
            return Success(result, "分配成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "分配用户失败");
            return Error<int>("分配用户失败");
        }
    }

    /// <summary>
    /// 获取角色的菜单ID列表
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>菜单ID列表</returns>
    /// <response code="200">成功获取菜单列表</response>
    /// <response code="401">未授权，需要先登录</response>
    [HttpPost("menu/{id}")]
    [ProducesResponseType(typeof(ApiResponse<List<Guid>>), 200)]
    public async Task<ApiResponse<List<Guid>>> GetMenuIds(Guid id)
    {
        try
        {
            var menuIds = await _roleService.GetMenuIdsAsync(id);
            return Success(menuIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取角色菜单失败: {Id}", id);
            return Error<List<Guid>>("获取角色菜单失败");
        }
    }

    /// <summary>
    /// 更新角色的菜单权限
    /// </summary>
    /// <param name="dto">更新菜单请求参数，包含角色ID和菜单ID列表</param>
    /// <returns>更新的菜单数量</returns>
    /// <response code="200">更新成功</response>
    /// <response code="401">未授权，需要先登录</response>
    [HttpPost("updateMenu")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> UpdateMenu([FromBody] UpdateRoleMenuDto dto)
    {
        try
        {
            var result = await _roleService.UpdateMenuAsync(dto.Id, dto.MenuIds);
            return Success(result, "更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新角色菜单失败: {Id}", dto.Id);
            return Error<int>("更新角色菜单失败");
        }
    }
}

/// <summary>
/// 分配用户请求 DTO
/// </summary>
public class AssignUsersDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// 用户ID列表
    /// </summary>
    public List<Guid> UserIds { get; set; } = new();
}

/// <summary>
/// 更新角色菜单请求 DTO
/// </summary>
public class UpdateRoleMenuDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 菜单ID列表
    /// </summary>
    public List<Guid> MenuIds { get; set; } = new();
}