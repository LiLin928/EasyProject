namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新订单参数
/// </summary>
public class UpdateOrderDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 收货人姓名
    /// </summary>
    public string? ReceiverName { get; set; }

    /// <summary>
    /// 收货人电话
    /// </summary>
    public string? ReceiverPhone { get; set; }

    /// <summary>
    /// 收货地址
    /// </summary>
    public string? ReceiverAddress { get; set; }

    /// <summary>
    /// 后台备注
    /// </summary>
    public string? AdminRemark { get; set; }
}