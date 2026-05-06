namespace EasyWeChatModels.Dto;

/// <summary>
/// 订单状态常量
/// </summary>
public static class OrderStatus
{
    public const int Pending = 0;      // 待付款
    public const int Paid = 1;         // 待发货
    public const int Shipped = 2;      // 待收货
    public const int Completed = 3;    // 已完成
    public const int Cancelled = 4;    // 已取消
    public const int Refunded = 5;     // 已退款

    /// <summary>
    /// 获取状态文本
    /// </summary>
    public static string GetText(int status) => status switch
    {
        0 => "待付款",
        1 => "待发货",
        2 => "待收货",
        3 => "已完成",
        4 => "已取消",
        5 => "已退款",
        _ => "未知"
    };
}