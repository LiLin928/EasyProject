namespace EasyWeChatModels.Dto;

/// <summary>
/// 优惠券查询参数
/// </summary>
public class QueryCouponDto
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
    /// 优惠券名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 类型：1-满减券，2-折扣券
    /// </summary>
    public int? Type { get; set; }

    /// <summary>
    /// 状态：1-启用，0-禁用
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