using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 会员等级控制器
/// </summary>
/// <remarks>
/// 提供会员等级管理功能接口
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MemberLevelController : BaseController
{
    /// <summary>
    /// 会员等级服务接口
    /// </summary>
    public IMemberLevelService _memberLevelService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<MemberLevelController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取会员等级列表
    /// </summary>
    /// <returns>会员等级列表</returns>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<List<MemberLevelDto>>), 200)]
    public async Task<ApiResponse<List<MemberLevelDto>>> GetList()
    {
        try
        {
            var result = await _memberLevelService.GetAllAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取会员等级列表失败");
            return Error<List<MemberLevelDto>>("获取会员等级列表失败");
        }
    }

    /// <summary>
    /// 获取会员等级详情
    /// </summary>
    /// <param name="id">等级ID</param>
    /// <returns>会员等级详情</returns>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<MemberLevelDto>), 200)]
    public async Task<ApiResponse<MemberLevelDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _memberLevelService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<MemberLevelDto>("会员等级不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取会员等级详情失败: {Id}", id);
            return Error<MemberLevelDto>("获取会员等级详情失败");
        }
    }

    /// <summary>
    /// 创建会员等级
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的等级ID</returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Create([FromBody] AddMemberLevelDto dto)
    {
        try
        {
            var id = await _memberLevelService.AddAsync(dto);
            return Success(id, "创建成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建会员等级失败");
            return Error<Guid>("创建会员等级失败");
        }
    }

    /// <summary>
    /// 更新会员等级
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateMemberLevelDto dto)
    {
        try
        {
            var result = await _memberLevelService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新会员等级失败: {Id}", dto.Id);
            return Error<int>("更新会员等级失败");
        }
    }

    /// <summary>
    /// 删除会员等级
    /// </summary>
    /// <param name="id">等级ID</param>
    /// <returns>影响的行数</returns>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] Guid id)
    {
        try
        {
            var result = await _memberLevelService.DeleteAsync(id);
            return Success(result, "删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除会员等级失败: {Id}", id);
            return Error<int>("删除会员等级失败");
        }
    }

    /// <summary>
    /// 批量删除会员等级
    /// </summary>
    /// <param name="ids">等级ID列表</param>
    /// <returns>影响的行数</returns>
    [HttpPost("delete-batch")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> DeleteBatch([FromBody] List<Guid> ids)
    {
        try
        {
            var result = await _memberLevelService.DeleteBatchAsync(ids);
            return Success(result, "批量删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量删除会员等级失败");
            return Error<int>("批量删除会员等级失败");
        }
    }
}