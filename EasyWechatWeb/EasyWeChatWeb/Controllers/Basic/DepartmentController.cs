using BusinessManager.Basic.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Basic;

/// <summary>
/// 部门控制器
/// </summary>
/// <remarks>
/// 提供部门的增删改查、树形结构查询、部门成员查询等API接口
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentController : BaseController
{
    /// <summary>
    /// 部门服务接口
    /// </summary>
    public IDepartmentService _departmentService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<DepartmentController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取部门树形结构
    /// </summary>
    /// <returns>树形部门列表</returns>
    /// <response code="200">成功获取部门树</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 返回所有有效状态的部门，按Sort排序，组成树形结构。
    /// 每个节点包含子节点集合(Children)和成员数量(MemberCount)。
    /// </remarks>
    [HttpGet("tree")]
    [ProducesResponseType(typeof(ApiResponse<List<DepartmentDto>>), 200)]
    public async Task<ApiResponse<List<DepartmentDto>>> GetTree()
    {
        try
        {
            var result = await _departmentService.GetTreeAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取部门树失败");
            return Error<List<DepartmentDto>>("获取部门树失败");
        }
    }

    /// <summary>
    /// 获取部门详情
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <returns>部门详细信息</returns>
    /// <response code="200">成功获取部门详情</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">部门不存在</response>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), 200)]
    public async Task<ApiResponse<DepartmentDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _departmentService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<DepartmentDto>("部门不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取部门详情失败: {Id}", id);
            return Error<DepartmentDto>("获取部门详情失败");
        }
    }

    /// <summary>
    /// 获取部门成员列表
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <returns>部门下的用户列表</returns>
    /// <response code="200">成功获取部门成员</response>
    /// <response code="401">未授权，需要先登录</response>
    [HttpGet("users/{id}")]
    [ProducesResponseType(typeof(ApiResponse<List<UserDto>>), 200)]
    public async Task<ApiResponse<List<UserDto>>> GetUsers(Guid id)
    {
        try
        {
            var result = await _departmentService.GetDepartmentUsersAsync(id);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取部门成员失败: {Id}", id);
            return Error<List<UserDto>>("获取部门成员失败");
        }
    }

    /// <summary>
    /// 新增部门
    /// </summary>
    /// <param name="dto">新增部门参数</param>
    /// <returns>新创建的部门ID</returns>
    /// <response code="200">添加成功，返回新部门ID</response>
    /// <response code="400">部门编码已存在</response>
    /// <response code="401">未授权，需要先登录</response>
    [HttpPost("add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Add([FromBody] AddDepartmentDto dto)
    {
        try
        {
            var id = await _departmentService.CreateAsync(dto);
            return Success(id, "添加成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加部门失败");
            return Error<Guid>(ex.Message);
        }
    }

    /// <summary>
    /// 更新部门信息
    /// </summary>
    /// <param name="dto">更新部门参数</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">更新成功</response>
    /// <response code="400">部门编码已存在或不能移动到子部门</response>
    /// <response code="404">部门不存在</response>
    /// <response code="401">未授权，需要先登录</response>
    [HttpPost("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateDepartmentDto dto)
    {
        try
        {
            var result = await _departmentService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新部门失败: {Id}", dto.Id);
            return Error<int>(ex.Message);
        }
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    /// <param name="id">要删除的部门ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">删除成功</response>
    /// <response code="400">存在子部门或成员，无法删除</response>
    /// <response code="404">部门不存在</response>
    /// <response code="401">未授权，需要先登录</response>
    [HttpDelete("delete/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete(Guid id)
    {
        try
        {
            var result = await _departmentService.DeleteAsync(id);
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除部门失败: {Id}", id);
            return Error<int>(ex.Message);
        }
    }
}