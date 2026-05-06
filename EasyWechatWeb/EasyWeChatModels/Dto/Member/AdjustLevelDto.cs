namespace EasyWeChatModels.Dto;

/// <summary>
/// 调整等级参数
/// </summary>
public class AdjustLevelDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 会员等级ID
    /// </summary>
    public Guid LevelId { get; set; }

    /// <summary>
    /// 调整原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}