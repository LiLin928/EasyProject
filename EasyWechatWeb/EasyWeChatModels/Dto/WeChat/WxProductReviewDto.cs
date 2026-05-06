namespace EasyWeChatModels.Dto;

/// <summary>
/// 微信商品评价 DTO
/// </summary>
public class WxProductReviewDto
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
    /// 用户名（匿名时显示为"用户***"）
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 用户头像
    /// </summary>
    public string? UserAvatar { get; set; }

    /// <summary>
    /// 评分（1-5星）
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
    /// 回复时间（Unix毫秒时间戳）
    /// </summary>
    public long? ReplyTime { get; set; }

    /// <summary>
    /// 是否匿名
    /// </summary>
    public bool IsAnonymous { get; set; }

    /// <summary>
    /// 创建时间（Unix毫秒时间戳）
    /// </summary>
    public long CreateTime { get; set; }
}