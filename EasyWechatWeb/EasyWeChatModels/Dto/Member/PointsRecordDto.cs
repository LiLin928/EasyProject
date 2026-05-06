namespace EasyWeChatModels.Dto;

/// <summary>
/// 积分记录DTO
/// </summary>
public class PointsRecordDto
{
    /// <summary>
    /// 记录ID
    /// </summary>
    public Guid Id { get; set; }

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
    /// 积分变动
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 变动后余额
    /// </summary>
    public int Balance { get; set; }

    /// <summary>
    /// 变动类型
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 变动类型文本
    /// </summary>
    public string TypeText => Type switch
    {
        "review" => "评价奖励",
        "order" => "订单奖励",
        "exchange" => "积分兑换",
        "refund" => "退款扣除",
        "system" => "系统调整",
        _ => "其他"
    };

    /// <summary>
    /// 原因描述
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; } = string.Empty;
}