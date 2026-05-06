namespace EasyWeChatModels.Dto;

/// <summary>
/// 调整积分参数
/// </summary>
public class AdjustPointsDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 调整类型：add-增加，subtract-扣减
    /// </summary>
    public string Type { get; set; } = "add";

    /// <summary>
    /// 积分数量
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// 调整原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}