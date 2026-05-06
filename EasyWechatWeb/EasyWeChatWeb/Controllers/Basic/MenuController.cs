using BusinessManager.Basic.IService;
using BusinessManager.Basic.Service;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Basic;

/// <summary>
/// 菜单控制器
/// 提供菜单的增删改查、权限分配、用户菜单获取等功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MenuController : BaseController
{
    /// <summary>
    /// 菜单服务接口
    /// </summary>
    public IMenuService _menuService { get; set; } = null!;
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<MenuController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取菜单列表（树形）
    /// </summary>
    /// <returns>树形结构的菜单列表</returns>
    /// <response code="200">成功获取菜单列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取系统中所有菜单的树形结构列表。
    /// 菜单按层级组织，支持多级嵌套。
    /// 用于后台管理界面的菜单配置。
    /// </remarks>
    /// <example>
    /// POST /api/menu/list
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<List<MenuDto>>), 200)]
    public async Task<ApiResponse<List<MenuDto>>> GetList()
    {
        try
        {
            var result = await _menuService.GetTreeAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取菜单列表失败");
            return Error<List<MenuDto>>("获取菜单列表失败");
        }
    }

    /// <summary>
    /// 获取当前用户的菜单
    /// </summary>
    /// <returns>当前用户有权访问的菜单树形列表</returns>
    /// <response code="200">成功获取用户菜单</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 根据当前登录用户的角色，返回其有权访问的菜单列表。
    /// 用于前端动态渲染用户可见的菜单。
    /// 返回树形结构，便于前端直接使用。
    /// </remarks>
    /// <example>
    /// GET /api/menu/user-menu
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("user-menu")]
    [ProducesResponseType(typeof(ApiResponse<List<MenuDto>>), 200)]
    public async Task<ApiResponse<List<MenuDto>>> GetUserMenu()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _menuService.GetUserMenuTreeAsync(userId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户菜单失败");
            return Error<List<MenuDto>>("获取用户菜单失败");
        }
    }

    /// <summary>
    /// 获取菜单详情
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>菜单详细信息</returns>
    /// <response code="200">成功获取菜单详情</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">菜单不存在</response>
    /// <remarks>
    /// 根据菜单ID获取菜单的详细信息，包括菜单名称、路径、图标、排序等。
    /// </remarks>
    /// <example>
    /// GET /api/menu/detail/1
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<MenuDto>), 200)]
    public async Task<ApiResponse<MenuDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _menuService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<MenuDto>("菜单不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取菜单详情失败: {Id}", id);
            return Error<MenuDto>("获取菜单详情失败");
        }
    }

    /// <summary>
    /// 添加菜单
    /// </summary>
    /// <param name="dto">添加菜单请求参数，包含菜单名称、路径、图标等信息</param>
    /// <returns>新创建的菜单ID</returns>
    /// <response code="200">添加成功，返回新菜单ID</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 创建新菜单，支持设置父菜单实现多级菜单。
    /// 菜单路径必须唯一。
    /// 排序值越小，菜单显示越靠前。
    /// </remarks>
    /// <example>
    /// POST /api/menu/add
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "name": "用户管理",
    ///     "path": "/system/user",
    ///     "icon": "user",
    ///     "parentId": 0,
    ///     "sort": 1
    /// }
    /// </example>
    [HttpPost("add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Add([FromBody] AddMenuDto dto)
    {
        try
        {
            var id = await _menuService.AddAsync(dto);
            return Success(id, "添加成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加菜单失败");
            return Error<Guid>("添加菜单失败");
        }
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    /// <param name="dto">更新菜单请求参数，包含菜单ID和需要更新的字段</param>
    /// <returns>更新的菜单ID</returns>
    /// <response code="200">更新成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 更新菜单信息，只更新请求中提供的字段。
    /// 菜单路径不能与其他菜单重复。
    /// </remarks>
    /// <example>
    /// PUT /api/menu/update
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "id": 1,
    ///     "name": "新菜单名",
    ///     "icon": "new-icon"
    /// }
    /// </example>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateMenuDto dto)
    {
        try
        {
            var result = await _menuService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新菜单失败: {Id}", dto.Id);
            return Error<int>("更新菜单失败");
        }
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    /// <param name="id">要删除的菜单ID</param>
    /// <returns>删除的菜单ID</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 删除菜单，如果菜单有子菜单，子菜单也会被一并删除。
    /// 删除菜单会同时移除该菜单与角色的关联关系。
    /// </remarks>
    /// <example>
    /// POST /api/menu/delete
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
            var result = await _menuService.DeleteAsync(id);
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除菜单失败: {Id}", id);
            return Error<int>("删除菜单失败");
        }
    }

    /// <summary>
    /// 分配角色权限
    /// </summary>
    /// <param name="dto">分配权限请求参数，包含菜单ID和角色ID列表</param>
    /// <returns>分配的角色数量</returns>
    /// <response code="200">分配成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 将菜单的访问权限分配给指定的角色。
    /// 角色下的用户将能够访问该菜单。
    /// 每次调用会覆盖之前的权限设置。
    /// </remarks>
    /// <example>
    /// POST /api/menu/assign-roles
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "menuId": 1,
    ///     "roleIds": [1, 2, 3]
    /// }
    /// </example>
    [HttpPost("assign-roles")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> AssignRoles([FromBody] AssignRolesDto dto)
    {
        try
        {
            var result = await _menuService.AssignRolesAsync(dto.MenuId, dto.RoleIds);
            return Success(result, "分配成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "分配权限失败");
            return Error<int>("分配权限失败");
        }
    }
}

/// <summary>
/// 分配权限请求 DTO
/// </summary>
public class AssignRolesDto
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid MenuId { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    public List<Guid> RoleIds { get; set; } = new();
}