using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 订单实体类
/// </summary>
/// <remarks>
/// 用于存储用户订单信息，包括订单状态、金额、物流等
/// </remarks>
[SugarTable("Order", "订单表")]
public class Order
{
    /// <summary>
    /// 订单ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 订单编号
    /// </summary>
    /// <remarks>
    /// 唯一订单编号，用于订单查询和显示
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "订单编号")]
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 用户ID
    /// </summary>
    /// <remarks>
    /// 下单用户的外键
    /// </remarks>
    [SugarColumn(ColumnDescription = "用户ID")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 收货地址ID
    /// </summary>
    /// <remarks>
    /// 收货地址的外键（可选，后台下单可能不关联地址记录）
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "收货地址ID")]
    public Guid? AddressId { get; set; }

    /// <summary>
    /// 收货人姓名
    /// </summary>
    /// <remarks>
    /// 收货人姓名，后台下单时直接填写
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "收货人姓名")]
    public string? ReceiverName { get; set; }

    /// <summary>
    /// 收货人电话
    /// </summary>
    /// <remarks>
    /// 收货人联系电话
    /// </remarks>
    [SugarColumn(Length = 20, IsNullable = true, ColumnDescription = "收货人电话")]
    public string? ReceiverPhone { get; set; }

    /// <summary>
    /// 收货地址
    /// </summary>
    /// <remarks>
    /// 完整收货地址，后台下单时直接填写
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "收货地址")]
    public string? ReceiverAddress { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    /// <remarks>
    /// 订单状态：0-待付款，1-待发货，2-待收货，3-已完成，4-已取消，5-已退款
    /// </remarks>
    [SugarColumn(ColumnDescription = "订单状态：0-待付款，1-待发货，2-待收货，3-已完成，4-已取消，5-已退款")]
    public int Status { get; set; } = 0;

    /// <summary>
    /// 订单总金额
    /// </summary>
    /// <remarks>
    /// 订单的总金额，保留两位小数
    /// </remarks>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "订单总金额")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 订单备注
    /// </summary>
    /// <remarks>
    /// 用户下单时填写的备注信息，长度限制200字符，可为空
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "订单备注")]
    public string? Remark { get; set; }

    /// <summary>
    /// 后台备注
    /// </summary>
    /// <remarks>
    /// 管理员对订单的备注信息，长度限制200字符，可为空
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "后台备注")]
    public string? AdminRemark { get; set; }

    /// <summary>
    /// 已退款金额
    /// </summary>
    /// <remarks>
    /// 订单已退款的金额，保留两位小数，可为空
    /// </remarks>
    [SugarColumn(DecimalDigits = 2, IsNullable = true, ColumnDescription = "已退款金额")]
    public decimal? RefundAmount { get; set; }

    /// <summary>
    /// 确认收货时间
    /// </summary>
    /// <remarks>
    /// 用户确认收货的时间，可为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "确认收货时间")]
    public DateTime? ConfirmTime { get; set; }

    /// <summary>
    /// 订单来源
    /// </summary>
    /// <remarks>
    /// 订单来源：pc-后台，wechat-小程序
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "订单来源")]
    public string Source { get; set; } = "wechat";

    /// <summary>
    /// 物流公司
    /// </summary>
    /// <remarks>
    /// 物流公司名称，长度限制50字符，可为空
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "物流公司")]
    public string? LogisticsCompany { get; set; }

    /// <summary>
    /// 物流单号
    /// </summary>
    /// <remarks>
    /// 物流跟踪单号，长度限制50字符，可为空
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "物流单号")]
    public string? LogisticsNumber { get; set; }

    /// <summary>
    /// 支付时间
    /// </summary>
    /// <remarks>
    /// 订单支付完成时间，可为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "支付时间")]
    public DateTime? PaymentTime { get; set; }

    /// <summary>
    /// 发货时间
    /// </summary>
    /// <remarks>
    /// 订单发货时间，可为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "发货时间")]
    public DateTime? DeliveryTime { get; set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    /// <remarks>
    /// 订单完成时间，可为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "完成时间")]
    public DateTime? CompleteTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 订单创建时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <remarks>
    /// 订单最后更新时间，可为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}