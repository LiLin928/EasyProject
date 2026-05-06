namespace EasyWeChatModels.Dto;

/// <summary>
/// 积分记录查询参数
/// </summary>
public class QueryPointsRecordDto
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 用户关键字（昵称/手机号）
    /// </summary>
    public string? UserKeyword { get; set; }

    /// <summary>
    /// 变动类型
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public string? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public string? EndTime { get; set; }
}