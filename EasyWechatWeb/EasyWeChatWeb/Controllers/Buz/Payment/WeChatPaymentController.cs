using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.WeChatPro;

/// <summary>
/// 微信支付控制器
/// 提供支付创建功能
/// </summary>
[ApiController]
[Route("api/wechat/payment")]
[Authorize]
public class WeChatPaymentController : BaseController
{
    /// <summary>
    /// 微信支付服务接口
    /// </summary>
    public IWeChatPaymentService _paymentService { get; set; } = null!;
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatPaymentController> _logger { get; set; } = null!;

    /// <summary>
    /// 创建支付
    /// </summary>
    /// <param name="dto">创建支付请求参数，包含订单ID和支付方式</param>
    /// <returns>支付结果</returns>
    /// <response code="200">支付创建成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">订单状态不允许支付</response>
    /// <response code="404">订单不存在</response>
    /// <remarks>
    /// 为订单创建支付，支持微信支付、支付宝、余额支付。
    /// 支付方式：1-微信，2-支付宝，3-余额。
    /// 只有待支付状态的订单可以创建支付。
    /// </remarks>
    /// <example>
    /// POST /api/wechat/payment/create
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "orderId": "xxx-xxx-xxx",
    ///     "paymentMethod": 1
    /// }
    /// </example>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResultDto>), 200)]
    public async Task<ApiResponse<PaymentResultDto>> Create([FromBody] CreatePaymentDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Error<PaymentResultDto>("请先登录", 401);
            }

            var result = await _paymentService.CreatePaymentAsync(userId, dto);
            if (!result.Success)
            {
                return Error<PaymentResultDto>(result.Message ?? "支付创建失败", 400);
            }
            return Success(result, "支付创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建支付失败");
            return Error<PaymentResultDto>("创建支付失败");
        }
    }
}