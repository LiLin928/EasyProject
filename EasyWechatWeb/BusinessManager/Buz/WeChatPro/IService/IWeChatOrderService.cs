using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 微信订单服务接口
/// </summary>
public interface IWeChatOrderService
{
    /// <summary>
    /// 创建订单
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="dto">订单信息</param>
    /// <returns>订单ID</returns>
    Task<Guid> CreateOrderAsync(Guid userId, CreateOrderDto dto);

    /// <summary>
    /// 获取订单列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="query">查询参数</param>
    /// <returns>订单分页列表</returns>
    Task<PageResponse<OrderDto>> GetOrderListAsync(Guid userId, QueryOrderDto query);

    /// <summary>
    /// 获取订单详情
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="id">订单ID</param>
    /// <returns>订单详情</returns>
    Task<OrderDto?> GetOrderByIdAsync(Guid userId, Guid id);

    /// <summary>
    /// 取消订单
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="id">订单ID</param>
    /// <returns>是否成功</returns>
    Task<bool> CancelOrderAsync(Guid userId, Guid id);
}