namespace EasyWeChatModels.Dto;

/// <summary>
/// 提交评价 DTO
/// </summary>
public class SubmitReviewDto
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 评分（1-5）
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// 评价内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 评价图片
    /// </summary>
    public List<string>? Images { get; set; }

    /// <summary>
    /// 是否匿名
    /// </summary>
    public bool IsAnonymous { get; set; }
}