using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.WeChatPro;

/// <summary>
/// 微信购物车控制器
/// 提供购物车的增删改查功能
/// </summary>
[ApiController]
[Route("api/wechat/cart")]
[Authorize]
public class WeChatCartController : BaseController
{
    /// <summary>
    /// 微信购物车服务接口
    /// </summary>
    public IWeChatCartService _cartService { get; set; } = null!;
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatCartController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取购物车列表
    /// </summary>
    /// <returns>购物车状态，包含商品列表、总数量、总金额等</returns>
    /// <response code="200">成功获取购物车列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取当前用户的购物车状态，包括所有购物车项、总数量、总金额、选中数量、选中金额等。
    /// </remarks>
    /// <example>
    /// POST /api/wechat/cart/list
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<CartStateDto>), 200)]
    public async Task<ApiResponse<CartStateDto>> GetList()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<CartStateDto>("请先登录", 401);
            }

            var result = await _cartService.GetCartListAsync(userId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取购物车列表失败");
            return Error<CartStateDto>("获取购物车列表失败");
        }
    }

    /// <summary>
    /// 添加商品到购物车
    /// </summary>
    /// <param name="dto">添加购物车请求参数，包含商品ID和数量</param>
    /// <returns>购物车项ID</returns>
    /// <response code="200">添加成功，返回购物车项ID</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 将商品添加到购物车，如果商品已存在则增加数量。
    /// </remarks>
    /// <example>
    /// POST /api/wechat/cart/add
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "productId": "xxx-xxx-xxx",
    ///     "count": 1
    /// }
    /// </example>
    [HttpPost("add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Add([FromBody] AddCartDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<Guid>("请先登录", 401);
            }

            var result = await _cartService.AddToCartAsync(userId, dto);
            return Success(result, "添加成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加购物车失败");
            return Error<Guid>("添加购物车失败");
        }
    }

    /// <summary>
    /// 更新购物车项
    /// </summary>
    /// <param name="id">购物车项ID</param>
    /// <param name="dto">更新购物车请求参数，包含数量和选中状态</param>
    /// <returns>更新结果</returns>
    /// <response code="200">更新成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">购物车项不存在</response>
    /// <remarks>
    /// 更新购物车项的数量或选中状态。
    /// 数量不能小于1。
    /// </remarks>
    /// <example>
    /// PUT /api/wechat/cart/{id}
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "count": 2,
    ///     "selected": true
    /// }
    /// </example>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> Update(Guid id, [FromBody] UpdateCartDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<bool>("请先登录", 401);
            }

            dto.Id = id;
            var result = await _cartService.UpdateCartItemAsync(userId, id, dto);
            if (!result)
            {
                return Error<bool>("购物车项不存在", 404);
            }
            return Success(result, "更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新购物车失败: {Id}", id);
            return Error<bool>("更新购物车失败");
        }
    }

    /// <summary>
    /// 删除购物车项
    /// </summary>
    /// <param name="id">购物车项ID</param>
    /// <returns>删除结果</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">购物车项不存在</response>
    /// <remarks>
    /// 从购物车中删除指定的商品项。
    /// </remarks>
    /// <example>
    /// POST /api/wechat/cart/delete
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

            var result = await _cartService.DeleteCartItemAsync(userId, id);
            if (!result)
            {
                return Error<bool>("购物车项不存在", 404);
            }
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除购物车失败: {Id}", id);
            return Error<bool>("删除购物车失败");
        }
    }

    /// <summary>
    /// 清空购物车
    /// </summary>
    /// <returns>清空结果</returns>
    /// <response code="200">清空成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 清空当前用户购物车中的所有商品。
    /// </remarks>
    /// <example>
    /// POST /api/wechat/cart/clear
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("clear")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> Clear()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<bool>("请先登录", 401);
            }

            var result = await _cartService.ClearCartAsync(userId);
            return Success(result, "清空成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清空购物车失败");
            return Error<bool>("清空购物车失败");
        }
    }
}