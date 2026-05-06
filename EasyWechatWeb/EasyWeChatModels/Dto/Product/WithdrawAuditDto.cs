namespace EasyWeChatModels.Dto;

/// <summary>
/// 撤回审核 DTO
/// </summary>
public class WithdrawAuditDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 撤回原因
    /// </summary>
    /// <example>商品信息有误，需要重新编辑</example>
    public string? Reason { get; set; }
}