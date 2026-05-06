namespace EasyWeChatModels.Dto;

/// <summary>
/// 审核驳回 DTO
/// </summary>
public class AuditRejectDto
{
    /// <summary>
    /// 驳回原因
    /// </summary>
    /// <example>商品信息不完整，请补充商品描述</example>
    public string RejectReason { get; set; } = string.Empty;
}