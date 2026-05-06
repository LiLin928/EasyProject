using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 售后记录实体类
/// </summary>
/// <remarks>
/// 用于存储订单的售后信息，包括退款、退货、换货等
/// </remarks>
[SugarTable("Refund", "售后记录表")]
public class Refund
{
    /// <summary>
    /// 售后ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 售后编号
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "售后编号")]
    public string RefundNo { get; set; } = string.Empty;

    /// <summary>
    /// 订单ID
    /// </summary>
    [SugarColumn(ColumnDescription = "订单ID")]
    public Guid OrderId { get; set; }

    /// <summary>
    /// 订单编号
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "订单编号")]
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 售后类型
    /// </summary>
    /// <remarks>
    /// refund_only-仅退款，return_refund-退货退款，exchange-换货
    /// </remarks>
    [SugarColumn(ColumnDescription = "售后类型：refund_only-仅退款，return_refund-退货退款，exchange-换货")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 退款金额
    /// </summary>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "退款金额")]
    public decimal RefundAmount { get; set; }

    /// <summary>
    /// 退款原因
    /// </summary>
    [SugarColumn(Length = 200, ColumnDescription = "退款原因")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// 问题描述
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "问题描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// pending-待审核，approved-已通过，rejected-已拒绝，returning-退货中，refunding-退款中，completed-已完成，cancelled-已取消
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态")]
    public string Status { get; set; } = "pending";

    #region 用户退货物流信息

    /// <summary>
    /// 用户退货快递公司
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "用户退货快递公司")]
    public string? ReturnShipCompany { get; set; }

    /// <summary>
    /// 用户退货快递单号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "用户退货快递单号")]
    public string? ReturnShipNo { get; set; }

    /// <summary>
    /// 用户退货时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "用户退货时间")]
    public DateTime? ReturnShipTime { get; set; }

    #endregion

    #region 商家换货物流信息

    /// <summary>
    /// 商家换货快递公司
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "商家换货快递公司")]
    public string? ExchangeShipCompany { get; set; }

    /// <summary>
    /// 商家换货快递单号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "商家换货快递单号")]
    public string? ExchangeShipNo { get; set; }

    /// <summary>
    /// 商家换货发货时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "商家换货发货时间")]
    public DateTime? ExchangeShipTime { get; set; }

    #endregion

    #region 审核信息

    /// <summary>
    /// 审核人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "审核人")]
    public string? Approver { get; set; }

    /// <summary>
    /// 审核时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "审核时间")]
    public DateTime? ApproveTime { get; set; }

    /// <summary>
    /// 审核备注
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "审核备注")]
    public string? ApproveRemark { get; set; }

    #endregion

    /// <summary>
    /// 退款时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "退款时间")]
    public DateTime? RefundTime { get; set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "完成时间")]
    public DateTime? CompleteTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}