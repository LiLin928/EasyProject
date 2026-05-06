namespace EasyWeChatModels.Dto;

/// <summary>
/// 订单 DTO
/// </summary>
public class OrderDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

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
    /// 地址ID
    /// </summary>
    public Guid? AddressId { get; set; }

    /// <summary>
    /// 收货地址详情
    /// </summary>
    public AddressDto? Address { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态文本
    /// </summary>
    public string StatusText => OrderStatus.GetText(Status);

    /// <summary>
    /// 订单项列表
    /// </summary>
    public List<OrderItemDto>? Items { get; set; }

    /// <summary>
    /// 订单金额
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 支付时间
    /// </summary>
    public string? PayTime { get; set; }

    /// <summary>
    /// 物流公司
    /// </summary>
    public string? LogisticsCompany { get; set; }

    /// <summary>
    /// 物流单号
    /// </summary>
    public string? LogisticsNumber { get; set; }

    /// <summary>
    /// 发货时间
    /// </summary>
    public string? DeliveryTime { get; set; }

    /// <summary>
    /// 物流信息
    /// </summary>
    public LogisticsDto? Logistics { get; set; }

    /// <summary>
    /// 订单备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 后台备注
    /// </summary>
    public string? AdminRemark { get; set; }

    /// <summary>
    /// 已退款金额
    /// </summary>
    public decimal? RefundAmount { get; set; }

    /// <summary>
    /// 订单来源
    /// </summary>
    public string Source { get; set; } = "wechat";

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; } = string.Empty;
}