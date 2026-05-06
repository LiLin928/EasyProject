using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 微信支付服务接口
/// </summary>
public interface IWeChatPaymentService
{
    /// <summary>
    /// 创建支付
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="dto">支付信息</param>
    /// <returns>支付结果</returns>
    Task<PaymentResultDto> CreatePaymentAsync(Guid userId, CreatePaymentDto dto);
}