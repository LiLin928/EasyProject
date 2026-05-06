namespace EasyWeChatModels.Dto;

/// <summary>
/// 提交审核 DTO
/// </summary>
public class SubmitAuditDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 审核点编码
    /// </summary>
    /// <example>PRODUCT_AUDIT</example>
    public string AuditPointCode { get; set; } = string.Empty;
}