namespace EasyWeChatModels.Dto;

/// <summary>
/// 通过审核参数
/// </summary>
public class ApproveRefundDto
{
    /// <summary>
    /// 售后ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 审核备注
    /// </summary>
    public string? Remark { get; set; }
}