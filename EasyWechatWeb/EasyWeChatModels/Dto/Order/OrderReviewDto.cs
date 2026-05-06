namespace EasyWeChatModels.Dto;

/// <summary>
/// 订单评价DTO
/// </summary>
public class OrderReviewDto
{
    /// <summary>
    /// 评价ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 订单ID
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 商品名称
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 用户头像
    /// </summary>
    public string? UserAvatar { get; set; }

    /// <summary>
    /// 商品质量评分
    /// </summary>
    public int ProductQuality { get; set; }

    /// <summary>
    /// 描述相符评分
    /// </summary>
    public int DescriptionMatch { get; set; }

    /// <summary>
    /// 性价比评分
    /// </summary>
    public int CostPerformance { get; set; }

    /// <summary>
    /// 发货速度评分
    /// </summary>
    public int ShippingSpeed { get; set; }

    /// <summary>
    /// 物流服务评分
    /// </summary>
    public int LogisticsService { get; set; }

    /// <summary>
    /// 客服态度评分
    /// </summary>
    public int CustomerService { get; set; }

    /// <summary>
    /// 综合评分
    /// </summary>
    public decimal OverallRating { get; set; }

    /// <summary>
    /// 评价内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 图片列表
    /// </summary>
    public List<string>? Images { get; set; }

    /// <summary>
    /// 是否匿名
    /// </summary>
    public bool IsAnonymous { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; set; } = "normal";

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; } = string.Empty;
}