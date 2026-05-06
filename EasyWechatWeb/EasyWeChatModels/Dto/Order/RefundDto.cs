namespace EasyWeChatModels.Dto;

/// <summary>
/// 售后DTO
/// </summary>
public class RefundDto
{
    /// <summary>
    /// 售后ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 售后编号
    /// </summary>
    public string RefundNo { get; set; } = string.Empty;

    /// <summary>
    /// 订单ID
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 售后类型
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 售后商品列表
    /// </summary>
    public List<RefundItemDto>? Items { get; set; }

    /// <summary>
    /// 换货商品列表
    /// </summary>
    public List<ExchangeItemDto>? ExchangeItems { get; set; }

    /// <summary>
    /// 退款金额
    /// </summary>
    public decimal RefundAmount { get; set; }

    /// <summary>
    /// 退款原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// 问题描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; set; } = "pending";

    /// <summary>
    /// 用户退货快递公司
    /// </summary>
    public string? ReturnShipCompany { get; set; }

    /// <summary>
    /// 用户退货快递单号
    /// </summary>
    public string? ReturnShipNo { get; set; }

    /// <summary>
    /// 用户退货时间
    /// </summary>
    public string? ReturnShipTime { get; set; }

    /// <summary>
    /// 商家换货快递公司
    /// </summary>
    public string? ExchangeShipCompany { get; set; }

    /// <summary>
    /// 商家换货快递单号
    /// </summary>
    public string? ExchangeShipNo { get; set; }

    /// <summary>
    /// 商家换货发货时间
    /// </summary>
    public string? ExchangeShipTime { get; set; }

    /// <summary>
    /// 审核人
    /// </summary>
    public string? Approver { get; set; }

    /// <summary>
    /// 审核时间
    /// </summary>
    public string? ApproveTime { get; set; }

    /// <summary>
    /// 审核备注
    /// </summary>
    public string? ApproveRemark { get; set; }

    /// <summary>
    /// 退款时间
    /// </summary>
    public string? RefundTime { get; set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    public string? CompleteTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; } = string.Empty;
}