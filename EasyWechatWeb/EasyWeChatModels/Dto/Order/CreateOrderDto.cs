namespace EasyWeChatModels.Dto;

/// <summary>
/// 创建订单 DTO（后台代客下单）
/// </summary>
public class CreateOrderDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 用户手机号
    /// </summary>
    public string? UserPhone { get; set; }

    /// <summary>
    /// 购物车项ID列表（小程序下单时使用）
    /// </summary>
    public List<Guid>? CartItemIds { get; set; }

    /// <summary>
    /// 收货地址ID（小程序下单时使用）
    /// </summary>
    public Guid? AddressId { get; set; }

    /// <summary>
    /// 收货人姓名
    /// </summary>
    public string ReceiverName { get; set; } = string.Empty;

    /// <summary>
    /// 收货人电话
    /// </summary>
    public string ReceiverPhone { get; set; } = string.Empty;

    /// <summary>
    /// 收货地址
    /// </summary>
    public string ReceiverAddress { get; set; } = string.Empty;

    /// <summary>
    /// 订单商品列表（后台代客下单时使用）
    /// </summary>
    public List<CreateOrderItemDto>? Items { get; set; }

    /// <summary>
    /// 订单备注
    /// </summary>
    public string? Remark { get; set; }
}