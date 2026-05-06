namespace EasyWeChatModels.Dto;

/// <summary>
/// 回复评价参数
/// </summary>
public class ReplyReviewDto
{
    /// <summary>
    /// 评价ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 回复内容
    /// </summary>
    public string Reply { get; set; } = string.Empty;
}