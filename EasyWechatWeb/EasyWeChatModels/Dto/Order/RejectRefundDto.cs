namespace EasyWeChatModels.Dto;

/// <summary>
/// 拒绝审核参数
/// </summary>
public class RejectRefundDto
{
    /// <summary>
    /// 售后ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 拒绝原因
    /// </summary>
    public string Remark { get; set; } = string.Empty;
}