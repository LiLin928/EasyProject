using BusinessManager.Basic.IService;
using BusinessManager.Basic.Service;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EasyWeChatWeb.Controllers.Basic;

/// <summary>
/// 用户控制器
/// 提供用户的增删改查、分页查询等功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : BaseController
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public IUserService _userService { get; set; } = null!;
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<UserController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取当前登录用户信息
    /// </summary>
    /// <returns>当前用户详细信息</returns>
    /// <response code="200">成功获取用户信息</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 根据当前登录用户的 Token 获取用户信息。
    /// </remarks>
    /// <example>
    /// GET /api/user/info
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("info")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    public async Task<ApiResponse<UserDto>> GetCurrentUser()
    {
        try
        {
            // 从 Token 中获取用户 ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value
                           ?? User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Error<UserDto>("无法获取用户信息", 401);
            }

            var result = await _userService.GetByIdAsync(userId);
            if (result == null)
            {
                return Error<UserDto>("用户不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取当前用户信息失败");
            return Error<UserDto>("获取用户信息失败");
        }
    }

    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    /// <param name="dto">查询参数，包含分页、用户名、昵称、状态、排序等</param>
    /// <returns>分页后的用户列表</returns>
    /// <response code="200">成功获取用户列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 支持多条件查询：
    /// - userName: 用户名模糊匹配
    /// - realName: 昵称模糊匹配
    /// - status: 状态精确匹配（1-启用，0-禁用）
    /// - sortField/sortOrder: 支持按 userName、realName、email、phone、createTime 排序
    /// 返回结果默认按创建时间倒序排列。
    /// </remarks>
    /// <example>
    /// GET /api/user/list?pageIndex=1&amp;pageSize=10&amp;userName=admin&amp;status=1&amp;sortField=createTime&amp;sortOrder=descending
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<UserDto>>), 200)]
    public async Task<ApiResponse<PageResponse<UserDto>>> GetList([FromBody] QueryUserDto dto)
    {
        try
        {
            var result = await _userService.GetPageListAsync(dto);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户列表失败");
            return Error<PageResponse<UserDto>>("获取用户列表失败");
        }
    }

    /// <summary>
    /// 获取用户详情
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户详细信息</returns>
    /// <response code="200">成功获取用户详情</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">用户不存在</response>
    /// <remarks>
    /// 根据用户ID获取用户的详细信息，包括基本信息、角色信息等。
    /// </remarks>
    /// <example>
    /// GET /api/user/detail/1
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    public async Task<ApiResponse<UserDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _userService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<UserDto>("用户不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户详情失败: {Id}", id);
            return Error<UserDto>("获取用户详情失败");
        }
    }

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="dto">添加用户请求参数，包含用户名、密码、昵称等信息</param>
    /// <returns>新创建的用户ID</returns>
    /// <response code="200">添加成功，返回新用户ID</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 创建新用户，用户名必须唯一。
    /// 密码会自动进行加密存储。
    /// </remarks>
    /// <example>
    /// POST /api/user/add
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "userName": "newuser",
    ///     "password": "123456",
    ///     "nickName": "新用户",
    ///     "email": "user@example.com",
    ///     "phone": "13800138000"
    /// }
    /// </example>
    [HttpPost("add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Add([FromBody] AddUserDto dto)
    {
        try
        {
            var id = await _userService.AddAsync(dto);
            return Success(id, "添加成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加用户失败");
            return Error<Guid>("添加用户失败");
        }
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="dto">更新用户请求参数，包含用户ID和需要更新的字段</param>
    /// <returns>更新的用户ID</returns>
    /// <response code="200">更新成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 更新用户信息，只更新请求中提供的字段。
    /// 用户名不能重复。
    /// 如需修改密码，请使用专门的修改密码接口。
    /// </remarks>
    /// <example>
    /// PUT /api/user/update
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "id": 1,
    ///     "nickName": "新昵称",
    ///     "email": "newemail@example.com",
    ///     "phone": "13900139000"
    /// }
    /// </example>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateUserDto dto)
    {
        try
        {
            var result = await _userService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用户失败: {Id}", dto.Id);
            return Error<int>("更新用户失败");
        }
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">要删除的用户ID</param>
    /// <returns>删除的用户ID</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 删除用户数据。
    /// 删除后用户将无法登录。
    /// 不能删除 admin 用户。
    /// </remarks>
    /// <example>
    /// POST /api/user/delete
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
            var result = await _userService.DeleteAsync(id);
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除用户失败: {Id}", id);
            return Error<int>("删除用户失败");
        }
    }

    /// <summary>
    /// 批量删除用户
    /// </summary>
    /// <param name="ids">要删除的用户ID列表</param>
    /// <returns>删除的用户数量</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 批量删除用户，admin用户不会被删除。
    /// </remarks>
    /// <example>
    /// POST /api/user/deleteBatch
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// ["3fa85f64-5717-4562-b3fc-2c963f66afa6", "3fa85f64-5717-4562-b3fc-2c963f66afa7"]
    /// </example>
    [HttpPost("deleteBatch")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> DeleteBatch([FromBody] List<Guid> ids)
    {
        try
        {
            var result = await _userService.DeleteBatchAsync(ids);
            return Success(result, $"成功删除 {result} 个用户");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量删除用户失败");
            return Error<int>("批量删除用户失败");
        }
    }

    /// <summary>
    /// 更新用户状态
    /// </summary>
    /// <param name="dto">更新用户状态参数，包含用户ID和状态值</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">更新成功</response>
    /// <response code="400">不能禁用管理员账号</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 启用或禁用用户账号。
    /// 禁用后用户将无法登录系统。
    /// 不能禁用admin账号。
    /// </remarks>
    /// <example>
    /// POST /api/user/updateStatus
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///     "status": 0
    /// }
    /// </example>
    [HttpPost("updateStatus")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> UpdateStatus([FromBody] UpdateUserStatusDto dto)
    {
        try
        {
            var result = await _userService.UpdateStatusAsync(dto.Id, dto.Status);
            return Success(result, dto.Status == 1 ? "启用成功" : "禁用成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用户状态失败: {Id}", dto.Id);
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 重置用户密码
    /// </summary>
    /// <param name="dto">重置密码参数</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">重置成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 将用户密码重置为指定的新密码。
    /// 新密码将自动进行MD5加密存储。
    /// </remarks>
    /// <example>
    /// POST /api/user/resetPassword
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///     "newPassword": "123456"
    /// }
    /// </example>
    [HttpPost("resetPassword")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        try
        {
            var result = await _userService.ResetPasswordAsync(dto.Id, dto.NewPassword);
            return Success(result, "密码重置成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重置用户密码失败: {Id}", dto.Id);
            return Error<int>("重置密码失败");
        }
    }
}