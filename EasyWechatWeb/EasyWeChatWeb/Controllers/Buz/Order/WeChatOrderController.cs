using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.WeChatPro;

/// <summary>
/// 微信订单控制器
/// 提供订单的创建、查询、取消等功能
/// </summary>
[ApiController]
[Route("api/wechat/order")]
[Authorize]
public class WeChatOrderController : BaseController
{
    /// <summary>
    /// 微信订单服务接口
    /// </summary>
    public IWeChatOrderService _orderService { get; set; } = null!;
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatOrderController> _logger { get; set; } = null!;

    /// <summary>
    /// 创建订单
    /// </summary>
    /// <param name="dto">创建订单请求参数，包含购物车项ID列表、地址ID、备注等</param>
    /// <returns>订单ID</returns>
    /// <response code="200">创建成功，返回订单ID</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">购物车为空或地址无效</response>
    /// <remarks>
    /// 从购物车创建订单，需要提供购物车项ID列表和收货地址。
    /// 创建成功后，购物车中的对应商品会被移除。
    /// </remarks>
    /// <example>
    /// POST /api/wechat/order/create
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "cartItemIds": ["xxx-xxx-xxx", "yyy-yyy-yyy"],
    ///     "addressId": "aaa-aaa-aaa",
    ///     "remark": "尽快发货"
    /// }
    /// </example>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Create([FromBody] CreateOrderDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<Guid>("请先登录", 401);
            }

            if (dto.CartItemIds == null || dto.CartItemIds.Count == 0)
            {
                return Error<Guid>("购物车为空", 400);
            }

            var result = await _orderService.CreateOrderAsync(userId, dto);
            return Success(result, "订单创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建订单失败");
            return Error<Guid>("创建订单失败");
        }
    }

    /// <summary>
    /// 获取订单列表（分页）
    /// </summary>
    /// <param name="query">查询参数，包含分页和状态筛选</param>
    /// <returns>订单分页列表</returns>
    /// <response code="200">成功获取订单列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取当前用户的订单列表，支持按状态筛选。
    /// 状态：1-待支付，2-已支付，3-配送中，4-已完成，5-已取消，6-退款中，7-已退款。
    /// </remarks>
    /// <example>
    /// POST /api/wechat/order/list
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "pageIndex": 1,
    ///     "pageSize": 10,
    ///     "status": 2
    /// }
    /// </example>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<OrderDto>>), 200)]
    public async Task<ApiResponse<PageResponse<OrderDto>>> GetList([FromBody] QueryOrderDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<PageResponse<OrderDto>>("请先登录", 401);
            }

            var result = await _orderService.GetOrderListAsync(userId, query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取订单列表失败");
            return Error<PageResponse<OrderDto>>("获取订单列表失败");
        }
    }

    /// <summary>
    /// 获取订单详情
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>订单详细信息</returns>
    /// <response code="200">成功获取订单详情</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">订单不存在</response>
    /// <remarks>
    /// 获取订单的详细信息，包括订单项、收货地址、物流信息、支付时间等。
    /// </remarks>
    /// <example>
    /// GET /api/wechat/order/{id}
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
    public async Task<ApiResponse<OrderDto>> GetDetail(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<OrderDto>("请先登录", 401);
            }

            var result = await _orderService.GetOrderByIdAsync(userId, id);
            if (result == null)
            {
                return Error<OrderDto>("订单不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取订单详情失败: {Id}", id);
            return Error<OrderDto>("获取订单详情失败");
        }
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>取消结果</returns>
    /// <response code="200">取消成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">订单状态不允许取消</response>
    /// <response code="404">订单不存在</response>
    /// <remarks>
    /// 取消订单，只有待支付状态的订单可以取消。
    /// 取消后订单状态变为已取消。
    /// </remarks>
    /// <example>
    /// PUT /api/wechat/order/{id}/cancel
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPut("{id}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> Cancel(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<bool>("请先登录", 401);
            }

            var result = await _orderService.CancelOrderAsync(userId, id);
            if (!result)
            {
                return Error<bool>("订单不存在或状态不允许取消", 400);
            }
            return Success(result, "取消成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取消订单失败: {Id}", id);
            return Error<bool>("取消订单失败");
        }
    }
}