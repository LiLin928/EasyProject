using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 售后控制器
/// </summary>
/// <remarks>
/// 提供售后管理、审核、退款、换货等功能接口
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RefundController : BaseController
{
    /// <summary>
    /// 售后服务接口
    /// </summary>
    public IRefundService _refundService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<RefundController> _logger { get; set; } = null!;

    #region 售后管理

    /// <summary>
    /// 获取售后列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页售后列表</returns>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<RefundDto>>), 200)]
    public async Task<ApiResponse<PageResponse<RefundDto>>> GetList([FromBody] QueryRefundDto query)
    {
        try
        {
            var result = await _refundService.GetPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取售后列表失败");
            return Error<PageResponse<RefundDto>>("获取售后列表失败");
        }
    }

    /// <summary>
    /// 获取售后详情
    /// </summary>
    /// <param name="id">售后ID</param>
    /// <returns>售后详情</returns>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<RefundDto>), 200)]
    public async Task<ApiResponse<RefundDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _refundService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<RefundDto>("售后记录不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取售后详情失败: {Id}", id);
            return Error<RefundDto>("获取售后详情失败");
        }
    }

    /// <summary>
    /// 通过审核
    /// </summary>
    /// <param name="dto">审核参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("approve")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Approve([FromBody] ApproveRefundDto dto)
    {
        try
        {
            var result = await _refundService.ApproveAsync(dto);
            return Success(result, "审核通过");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "审核通过失败: {Id}", dto.Id);
            return Error<int>("审核通过失败");
        }
    }

    /// <summary>
    /// 拒绝审核
    /// </summary>
    /// <param name="dto">拒绝参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("reject")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Reject([FromBody] RejectRefundDto dto)
    {
        try
        {
            var result = await _refundService.RejectAsync(dto);
            return Success(result, "已拒绝");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "拒绝审核失败: {Id}", dto.Id);
            return Error<int>("拒绝审核失败");
        }
    }

    /// <summary>
    /// 确认收到退货
    /// </summary>
    /// <param name="id">售后ID</param>
    /// <param name="remark">备注</param>
    /// <returns>影响的行数</returns>
    [HttpPut("confirm-receive/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> ConfirmReceive(Guid id, [FromQuery] string? remark)
    {
        try
        {
            var result = await _refundService.ConfirmReceiveAsync(id, remark);
            return Success(result, "确认收货成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "确认收到退货失败: {Id}", id);
            return Error<int>("确认收到退货失败");
        }
    }

    /// <summary>
    /// 换货发货
    /// </summary>
    /// <param name="dto">发货参数</param>
    /// <returns>影响的行数</returns>
    [HttpPost("exchange-ship")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> ExchangeShip([FromBody] ExchangeShipDto dto)
    {
        try
        {
            var result = await _refundService.ExchangeShipAsync(dto);
            return Success(result, "换货发货成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "换货发货失败: {Id}", dto.Id);
            return Error<int>("换货发货失败");
        }
    }

    /// <summary>
    /// 完成售后
    /// </summary>
    /// <param name="id">售后ID</param>
    /// <returns>影响的行数</returns>
    [HttpPut("complete/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Complete(Guid id)
    {
        try
        {
            var result = await _refundService.CompleteAsync(id);
            return Success(result, "售后已完成");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "完成售后失败: {Id}", id);
            return Error<int>("完成售后失败");
        }
    }

    /// <summary>
    /// 创建售后申请（后台代客申请）
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的售后ID</returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Create([FromBody] CreateRefundDto dto)
    {
        try
        {
            var result = await _refundService.CreateAsync(dto);
            return Success(result, "创建成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建售后申请失败");
            return Error<Guid>("创建售后申请失败");
        }
    }

    /// <summary>
    /// 获取换货物流详情
    /// </summary>
    /// <param name="id">售后ID</param>
    /// <returns>物流详情</returns>
    [HttpGet("ship-detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<ExchangeShipDetailDto>), 200)]
    public async Task<ApiResponse<ExchangeShipDetailDto>> GetShipDetail(Guid id)
    {
        try
        {
            var result = await _refundService.GetExchangeShipDetailAsync(id);
            return Success(result);
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<ExchangeShipDetailDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取换货物流详情失败: {Id}", id);
            return Error<ExchangeShipDetailDto>("获取换货物流详情失败");
        }
    }

    #endregion
}