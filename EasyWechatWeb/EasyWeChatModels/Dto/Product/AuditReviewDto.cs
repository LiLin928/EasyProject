namespace EasyWeChatModels.Dto;

/// <summary>
/// 审核评价参数
/// </summary>
public class AuditReviewDto
{
    /// <summary>
    /// 评价ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; set; } = string.Empty;
}