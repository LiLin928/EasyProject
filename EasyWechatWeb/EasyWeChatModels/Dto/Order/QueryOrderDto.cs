namespace EasyWeChatModels.Dto;

/// <summary>
/// 订单查询 DTO
/// </summary>
public class QueryOrderDto
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
    /// 订单编号
    /// </summary>
    public string? OrderNo { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 用户关键字（昵称/手机号）
    /// </summary>
    public string? UserKeyword { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public string? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public string? EndTime { get; set; }
}