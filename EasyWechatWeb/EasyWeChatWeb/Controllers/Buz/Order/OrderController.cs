using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 订单控制器
/// </summary>
/// <remarks>
/// 提供订单管理、发货、物流跟踪等功能接口
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : BaseController
{
    /// <summary>
    /// 订单服务接口
    /// </summary>
    public IOrderService _orderService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<OrderController> _logger { get; set; } = null!;

    #region 订单管理

    /// <summary>
    /// 获取订单列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页订单列表</returns>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<OrderDto>>), 200)]
    public async Task<ApiResponse<PageResponse<OrderDto>>> GetList([FromBody] QueryOrderDto query)
    {
        try
        {
            var result = await _orderService.GetPageListAsync(query);
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
    /// <returns>订单详情</returns>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
    public async Task<ApiResponse<OrderDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _orderService.GetByIdAsync(id);
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
    /// 创建订单（后台代客下单）
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的订单ID</returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Create([FromBody] CreateOrderDto dto)
    {
        try
        {
            var id = await _orderService.CreateAsync(dto);
            return Success(id, "订单创建成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建订单失败");
            return Error<Guid>("创建订单失败");
        }
    }

    /// <summary>
    /// 更新订单
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateOrderDto dto)
    {
        try
        {
            var result = await _orderService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新订单失败: {Id}", dto.Id);
            return Error<int>("更新订单失败");
        }
    }

    /// <summary>
    /// 订单发货
    /// </summary>
    /// <param name="dto">发货参数</param>
    /// <returns>影响的行数</returns>
    [HttpPost("ship")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Ship([FromBody] ShipDto dto)
    {
        try
        {
            var result = await _orderService.ShipAsync(dto);
            return Success(result, "发货成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "订单发货失败: {OrderId}", dto.OrderId);
            return Error<int>("订单发货失败");
        }
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>影响的行数</returns>
    [HttpPut("cancel/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Cancel(Guid id)
    {
        try
        {
            var result = await _orderService.CancelAsync(id);
            return Success(result, "取消成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取消订单失败: {Id}", id);
            return Error<int>("取消订单失败");
        }
    }

    /// <summary>
    /// 确认收货
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>影响的行数</returns>
    [HttpPut("confirm/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Confirm(Guid id)
    {
        try
        {
            var result = await _orderService.ConfirmAsync(id);
            return Success(result, "确认收货成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "确认收货失败: {Id}", id);
            return Error<int>("确认收货失败");
        }
    }

    /// <summary>
    /// 获取物流轨迹
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>物流轨迹信息</returns>
    [HttpGet("track/{id}")]
    [ProducesResponseType(typeof(ApiResponse<ShipTrackDto>), 200)]
    public async Task<ApiResponse<ShipTrackDto>> GetTrack(Guid id)
    {
        try
        {
            var result = await _orderService.GetShipTrackAsync(id);
            return Success(result);
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<ShipTrackDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取物流轨迹失败: {Id}", id);
            return Error<ShipTrackDto>("获取物流轨迹失败");
        }
    }

    /// <summary>
    /// 获取订单统计
    /// </summary>
    /// <returns>订单统计信息</returns>
    [HttpPost("statistics")]
    [ProducesResponseType(typeof(ApiResponse<OrderStatisticsDto>), 200)]
    public async Task<ApiResponse<OrderStatisticsDto>> GetStatistics()
    {
        try
        {
            var result = await _orderService.GetStatisticsAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取订单统计失败");
            return Error<OrderStatisticsDto>("获取订单统计失败");
        }
    }

    /// <summary>
    /// 导出订单
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>Excel文件</returns>
    [HttpPost("export")]
    public async Task<IActionResult> Export([FromBody] QueryOrderDto query)
    {
        try
        {
            var fileBytes = await _orderService.ExportAsync(query);
            var fileName = $"订单导出_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "导出订单失败");
            return BadRequest("导出订单失败");
        }
    }

    /// <summary>
    /// 获取订单评价列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页评价列表</returns>
    [HttpPost("review/list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<OrderReviewDto>>), 200)]
    public async Task<ApiResponse<PageResponse<OrderReviewDto>>> GetReviewList(
        [FromBody] QueryOrderReviewDto query)
    {
        try
        {
            var result = await _orderService.GetReviewListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取订单评价列表失败");
            return Error<PageResponse<OrderReviewDto>>("获取订单评价列表失败");
        }
    }

    /// <summary>
    /// 获取订单评价详情
    /// </summary>
    /// <param name="id">评价ID</param>
    /// <returns>评价详情</returns>
    [HttpGet("review/detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<OrderReviewDto>), 200)]
    public async Task<ApiResponse<OrderReviewDto>> GetReviewDetail(Guid id)
    {
        try
        {
            var result = await _orderService.GetReviewDetailAsync(id);
            if (result == null)
            {
                return Error<OrderReviewDto>("评价不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取订单评价详情失败: {Id}", id);
            return Error<OrderReviewDto>("获取订单评价详情失败");
        }
    }

    #endregion
}