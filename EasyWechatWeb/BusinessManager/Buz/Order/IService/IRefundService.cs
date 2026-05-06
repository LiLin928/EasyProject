using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 售后服务接口
/// </summary>
/// <remarks>
/// 提供售后管理、审核、退款、换货等功能
/// </remarks>
public interface IRefundService
{
    /// <summary>
    /// 获取售后分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页售后列表</returns>
    Task<PageResponse<RefundDto>> GetPageListAsync(QueryRefundDto query);

    /// <summary>
    /// 根据ID获取售后详情
    /// </summary>
    /// <param name="id">售后ID</param>
    /// <returns>售后信息</returns>
    Task<RefundDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 通过审核
    /// </summary>
    /// <param name="dto">审核参数</param>
    /// <returns>影响的行数</returns>
    Task<int> ApproveAsync(ApproveRefundDto dto);

    /// <summary>
    /// 拒绝审核
    /// </summary>
    /// <param name="dto">拒绝参数</param>
    /// <returns>影响的行数</returns>
    Task<int> RejectAsync(RejectRefundDto dto);

    /// <summary>
    /// 确认收到退货
    /// </summary>
    /// <param name="id">售后ID</param>
    /// <param name="remark">备注</param>
    /// <returns>影响的行数</returns>
    Task<int> ConfirmReceiveAsync(Guid id, string? remark);

    /// <summary>
    /// 换货发货
    /// </summary>
    /// <param name="dto">发货参数</param>
    /// <returns>影响的行数</returns>
    Task<int> ExchangeShipAsync(ExchangeShipDto dto);

    /// <summary>
    /// 完成售后
    /// </summary>
    /// <param name="id">售后ID</param>
    /// <returns>影响的行数</returns>
    Task<int> CompleteAsync(Guid id);

    /// <summary>
    /// 创建售后申请（后台代客申请）
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的售后ID</returns>
    Task<Guid> CreateAsync(CreateRefundDto dto);

    /// <summary>
    /// 获取换货物流详情
    /// </summary>
    /// <param name="id">售后ID</param>
    /// <returns>换货物流详情</returns>
    Task<ExchangeShipDetailDto> GetExchangeShipDetailAsync(Guid id);
}