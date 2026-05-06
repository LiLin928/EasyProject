using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 支付记录实体类
/// </summary>
/// <remarks>
/// 用于存储订单支付记录信息
/// </remarks>
[SugarTable("Payment", "支付记录表")]
public class Payment
{
    /// <summary>
    /// 支付ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 支付单号
    /// </summary>
    /// <remarks>
    /// 唯一支付单号，用于支付查询和显示
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "支付单号")]
    public string PaymentNo { get; set; } = string.Empty;

    /// <summary>
    /// 订单ID
    /// </summary>
    /// <remarks>
    /// 关联订单的外键
    /// </remarks>
    [SugarColumn(ColumnDescription = "订单ID")]
    public Guid OrderId { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    /// <remarks>
    /// 支付用户的外键
    /// </remarks>
    [SugarColumn(ColumnDescription = "用户ID")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 支付金额
    /// </summary>
    /// <remarks>
    /// 本次支付的金额，保留两位小数
    /// </remarks>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "支付金额")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 支付方式
    /// </summary>
    /// <remarks>
    /// 支付方式：1-微信支付，2-支付宝，3-余额支付
    /// </remarks>
    [SugarColumn(ColumnDescription = "支付方式：1-微信支付，2-支付宝，3-余额支付")]
    public int PaymentMethod { get; set; } = 1;

    /// <summary>
    /// 支付状态
    /// </summary>
    /// <remarks>
    /// 支付状态：0-待支付，1-支付成功，2-支付失败，3-已退款
    /// </remarks>
    [SugarColumn(ColumnDescription = "支付状态：0-待支付，1-支付成功，2-支付失败，3-已退款")]
    public int Status { get; set; } = 0;

    /// <summary>
    /// 第三方支付ID
    /// </summary>
    /// <remarks>
    /// 第三方支付平台返回的交易号，长度限制100字符，可为空
    /// </remarks>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "第三方支付ID")]
    public string? ThirdPartyPaymentId { get; set; }

    /// <summary>
    /// 支付时间
    /// </summary>
    /// <remarks>
    /// 支付完成时间，可为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "支付时间")]
    public DateTime? PaymentTime { get; set; }

    /// <summary>
    /// 支付消息
    /// </summary>
    /// <remarks>
    /// 支付返回的消息或错误信息，长度限制500字符，可为空
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "支付消息")]
    public string? Message { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 支付记录创建时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}