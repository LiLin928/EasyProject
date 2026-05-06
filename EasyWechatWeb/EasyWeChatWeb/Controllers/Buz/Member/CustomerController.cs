using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 客户控制器
/// </summary>
/// <remarks>
/// 提供客户管理、积分调整、等级调整等功能接口
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerController : BaseController
{
    /// <summary>
    /// 客户服务接口
    /// </summary>
    public ICustomerService _customerService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<CustomerController> _logger { get; set; } = null!;

    #region 客户管理

    /// <summary>
    /// 获取客户列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页客户列表</returns>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<CustomerDto>>), 200)]
    public async Task<ApiResponse<PageResponse<CustomerDto>>> GetList([FromBody] QueryCustomerDto query)
    {
        try
        {
            var result = await _customerService.GetPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取客户列表失败");
            return Error<PageResponse<CustomerDto>>("获取客户列表失败");
        }
    }

    /// <summary>
    /// 获取客户详情
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <returns>客户详情</returns>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDetailDto>), 200)]
    public async Task<ApiResponse<CustomerDetailDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _customerService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<CustomerDetailDto>("客户不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取客户详情失败: {Id}", id);
            return Error<CustomerDetailDto>("获取客户详情失败");
        }
    }

    /// <summary>
    /// 创建客户
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的客户ID</returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Create([FromBody] AddCustomerDto dto)
    {
        try
        {
            var id = await _customerService.AddAsync(dto);
            return Success(id, "创建成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建客户失败");
            return Error<Guid>("创建客户失败");
        }
    }

    /// <summary>
    /// 更新客户
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateCustomerDto dto)
    {
        try
        {
            var result = await _customerService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新客户失败: {Id}", dto.Id);
            return Error<int>("更新客户失败");
        }
    }

    /// <summary>
    /// 删除客户
    /// </summary>
    /// <param name="ids">客户ID列表</param>
    /// <returns>影响的行数</returns>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] List<Guid> ids)
    {
        try
        {
            var result = await _customerService.DeleteAsync(ids);
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除客户失败");
            return Error<int>("删除客户失败");
        }
    }

    /// <summary>
    /// 调整客户积分
    /// </summary>
    /// <param name="dto">调整参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("adjust-points")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> AdjustPoints([FromBody] AdjustPointsDto dto)
    {
        try
        {
            var result = await _customerService.AdjustPointsAsync(dto);
            return Success(result, "积分调整成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调整客户积分失败");
            return Error<int>("调整客户积分失败");
        }
    }

    /// <summary>
    /// 调整客户等级
    /// </summary>
    /// <param name="dto">调整参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("adjust-level")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> AdjustLevel([FromBody] AdjustLevelDto dto)
    {
        try
        {
            var result = await _customerService.AdjustLevelAsync(dto);
            return Success(result, "等级调整成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调整客户等级失败");
            return Error<int>("调整客户等级失败");
        }
    }

    /// <summary>
    /// 更新客户状态
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("update-status")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> UpdateStatus([FromBody] UpdateCustomerStatusDto dto)
    {
        try
        {
            var result = await _customerService.UpdateStatusAsync(dto.Id, dto.Status);
            return Success(result, "更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新客户状态失败");
            return Error<int>("更新客户状态失败");
        }
    }

    #endregion

    #region 地址管理

    /// <summary>
    /// 获取客户地址列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>地址列表</returns>
    [HttpPost("address/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<List<AddressDto>>), 200)]
    public async Task<ApiResponse<List<AddressDto>>> GetAddresses(Guid userId)
    {
        try
        {
            var result = await _customerService.GetAddressesAsync(userId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取客户地址失败: {UserId}", userId);
            return Error<List<AddressDto>>("获取客户地址失败");
        }
    }

    #endregion

    #region 购物车管理

    /// <summary>
    /// 获取客户购物车
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>购物车列表</returns>
    [HttpPost("cart/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<List<CartDto>>), 200)]
    public async Task<ApiResponse<List<CartDto>>> GetCart(Guid userId)
    {
        try
        {
            var result = await _customerService.GetCartsAsync(userId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取客户购物车失败: {UserId}", userId);
            return Error<List<CartDto>>("获取客户购物车失败");
        }
    }

    #endregion

    #region 收藏管理

    /// <summary>
    /// 获取客户收藏列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="groupId">分组ID（可选）</param>
    /// <returns>收藏列表</returns>
    [HttpGet("favorite/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<List<UserFavoriteDto>>), 200)]
    public async Task<ApiResponse<List<UserFavoriteDto>>> GetFavorites(Guid userId, [FromQuery] Guid? groupId)
    {
        try
        {
            var result = await _customerService.GetFavoritesAsync(userId, groupId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取客户收藏失败: {UserId}", userId);
            return Error<List<UserFavoriteDto>>("获取客户收藏失败");
        }
    }

    /// <summary>
    /// 获取客户收藏分组列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>分组列表</returns>
    [HttpGet("favorite-group/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<List<FavoriteGroupDto>>), 200)]
    public async Task<ApiResponse<List<FavoriteGroupDto>>> GetFavoriteGroups(Guid userId)
    {
        try
        {
            var result = await _customerService.GetFavoriteGroupsAsync(userId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取收藏分组失败: {UserId}", userId);
            return Error<List<FavoriteGroupDto>>("获取收藏分组失败");
        }
    }

    #endregion
}