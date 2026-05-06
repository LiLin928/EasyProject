namespace EasyWeChatModels.Dto;

/// <summary>
/// 审核驳回请求 DTO
/// </summary>
public class AuditRejectRequestDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 驳回原因
    /// </summary>
    /// <example>商品信息不完整，请补充商品描述</example>
    public string RejectReason { get; set; } = string.Empty;
}