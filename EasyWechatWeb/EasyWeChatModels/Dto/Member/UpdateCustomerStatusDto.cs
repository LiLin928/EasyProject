namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新客户状态参数
/// </summary>
public class UpdateCustomerStatusDto
{
    /// <summary>
    /// 客户ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 状态：0-禁用，1-启用
    /// </summary>
    public int Status { get; set; }
}