namespace EasyWeChatModels.Dto;

/// <summary>
/// 商品评价DTO
/// </summary>
public class ProductReviewDto
{
    /// <summary>
    /// 评价ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 商品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 商品图片
    /// </summary>
    public string? ProductImage { get; set; }

    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 用户昵称
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 用户头像
    /// </summary>
    public string? UserAvatar { get; set; }

    /// <summary>
    /// 评分
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// 评价内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 评价图片列表
    /// </summary>
    public List<string>? Images { get; set; }

    /// <summary>
    /// 商家回复
    /// </summary>
    public string? Reply { get; set; }

    /// <summary>
    /// 回复时间
    /// </summary>
    public string? ReplyTime { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; set; } = "pending";

    /// <summary>
    /// 是否匿名
    /// </summary>
    public bool IsAnonymous { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; } = string.Empty;
}