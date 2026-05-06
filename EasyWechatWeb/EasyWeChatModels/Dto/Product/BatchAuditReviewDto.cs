namespace EasyWeChatModels.Dto;

/// <summary>
/// 批量审核评价参数
/// </summary>
public class BatchAuditReviewDto
{
    /// <summary>
    /// 评价ID列表
    /// </summary>
    public List<Guid> Ids { get; set; } = new();

    /// <summary>
    /// 审核状态：approved-通过，rejected-拒绝
    /// </summary>
    public string Status { get; set; } = string.Empty;
}