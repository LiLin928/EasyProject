using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 订单服务接口
/// </summary>
/// <remarks>
/// 提供订单管理、发货、物流跟踪等功能
/// </remarks>
public interface IOrderService
{
    /// <summary>
    /// 获取订单分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页订单列表</returns>
    Task<PageResponse<OrderDto>> GetPageListAsync(QueryOrderDto query);

    /// <summary>
    /// 根据ID获取订单详情
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>订单信息</returns>
    Task<OrderDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建订单（后台代客下单）
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的订单ID</returns>
    Task<Guid> CreateAsync(CreateOrderDto dto);

    /// <summary>
    /// 更新订单
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateAsync(UpdateOrderDto dto);

    /// <summary>
    /// 订单发货
    /// </summary>
    /// <param name="dto">发货参数</param>
    /// <returns>影响的行数</returns>
    Task<int> ShipAsync(ShipDto dto);

    /// <summary>
    /// 取消订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>影响的行数</returns>
    Task<int> CancelAsync(Guid id);

    /// <summary>
    /// 确认收货
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>影响的行数</returns>
    Task<int> ConfirmAsync(Guid id);

    /// <summary>
    /// 获取物流轨迹
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns>物流轨迹信息</returns>
    Task<ShipTrackDto> GetShipTrackAsync(Guid id);

    /// <summary>
    /// 获取订单统计
    /// </summary>
    /// <returns>订单统计信息</returns>
    Task<OrderStatisticsDto> GetStatisticsAsync();

    /// <summary>
    /// 导出订单
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>Excel文件字节数组</returns>
    Task<byte[]> ExportAsync(QueryOrderDto query);

    /// <summary>
    /// 获取订单评价列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页评价列表</returns>
    Task<PageResponse<OrderReviewDto>> GetReviewListAsync(QueryOrderReviewDto query);

    /// <summary>
    /// 获取订单评价详情
    /// </summary>
    /// <param name="id">评价ID</param>
    /// <returns>评价详情</returns>
    Task<OrderReviewDto?> GetReviewDetailAsync(Guid id);
}