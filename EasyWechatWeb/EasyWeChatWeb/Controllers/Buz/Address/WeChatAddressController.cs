using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.WeChatPro;

/// <summary>
/// 微信地址控制器
/// 提供收货地址的增删改查功能
/// </summary>
[ApiController]
[Route("api/wechat/address")]
[Authorize]
public class WeChatAddressController : BaseController
{
    /// <summary>
    /// 微信地址服务接口
    /// </summary>
    public IWeChatAddressService _addressService { get; set; } = null!;
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatAddressController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取地址列表
    /// </summary>
    /// <returns>用户的所有收货地址列表</returns>
    /// <response code="200">成功获取地址列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取当前用户的所有收货地址，按创建时间排序。
    /// </remarks>
    /// <example>
    /// POST /api/wechat/address/list
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<List<AddressDto>>), 200)]
    public async Task<ApiResponse<List<AddressDto>>> GetList()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<List<AddressDto>>("请先登录", 401);
            }

            var result = await _addressService.GetAddressListAsync(userId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取地址列表失败");
            return Error<List<AddressDto>>("获取地址列表失败");
        }
    }

    /// <summary>
    /// 获取地址详情
    /// </summary>
    /// <param name="id">地址ID</param>
    /// <returns>地址详细信息</returns>
    /// <response code="200">成功获取地址详情</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">地址不存在</response>
    /// <remarks>
    /// 获取指定地址的详细信息，包括收货人、手机号、省市区、详细地址等。
    /// </remarks>
    /// <example>
    /// GET /api/wechat/address/{id}
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AddressDto>), 200)]
    public async Task<ApiResponse<AddressDto>> GetDetail(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<AddressDto>("请先登录", 401);
            }

            var result = await _addressService.GetAddressByIdAsync(userId, id);
            if (result == null)
            {
                return Error<AddressDto>("地址不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取地址详情失败: {Id}", id);
            return Error<AddressDto>("获取地址详情失败");
        }
    }

    /// <summary>
    /// 保存地址（新增或更新）
    /// </summary>
    /// <param name="dto">保存地址请求参数，包含收货人、手机号、地址信息等</param>
    /// <returns>地址ID</returns>
    /// <response code="200">保存成功，返回地址ID</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 新增或更新收货地址。如果 dto.Id 有值则更新，否则新增。
    /// 设置为默认地址时，其他地址会自动取消默认。
    /// </remarks>
    /// <example>
    /// POST /api/wechat/address/save
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "name": "张三",
    ///     "phone": "13800138000",
    ///     "province": "广东省",
    ///     "city": "深圳市",
    ///     "district": "南山区",
    ///     "detail": "某某小区1栋101室",
    ///     "isDefault": true
    /// }
    /// </example>
    [HttpPost("save")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Save([FromBody] SaveAddressDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<Guid>("请先登录", 401);
            }

            var result = await _addressService.SaveAddressAsync(userId, dto);
            return Success(result, "保存成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存地址失败");
            return Error<Guid>("保存地址失败");
        }
    }

    /// <summary>
    /// 删除地址
    /// </summary>
    /// <param name="id">地址ID</param>
    /// <returns>删除结果</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">地址不存在</response>
    /// <remarks>
    /// 删除指定的收货地址。
    /// </remarks>
    /// <example>
    /// POST /api/wechat/address/delete
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    /// </example>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> Delete([FromBody] Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<bool>("请先登录", 401);
            }

            var result = await _addressService.DeleteAddressAsync(userId, id);
            if (!result)
            {
                return Error<bool>("地址不存在", 404);
            }
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除地址失败: {Id}", id);
            return Error<bool>("删除地址失败");
        }
    }
}