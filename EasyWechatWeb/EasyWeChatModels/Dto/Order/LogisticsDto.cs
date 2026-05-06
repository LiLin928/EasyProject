namespace EasyWeChatModels.Dto;

/// <summary>
/// 物流信息 DTO（旧版兼容）
/// </summary>
public class LogisticsDto
{
    /// <summary>
    /// 物流公司
    /// </summary>
    public string Company { get; set; } = string.Empty;

    /// <summary>
    /// 物流单号
    /// </summary>
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// 物流状态
    /// </summary>
    public string Status { get; set; } = string.Empty;
}