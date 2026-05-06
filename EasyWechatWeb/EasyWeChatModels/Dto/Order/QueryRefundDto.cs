namespace EasyWeChatModels.Dto;

/// <summary>
/// 售后查询参数
/// </summary>
public class QueryRefundDto
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
    /// 售后编号
    /// </summary>
    public string? RefundNo { get; set; }

    /// <summary>
    /// 订单ID
    /// </summary>
    public Guid? OrderId { get; set; }

    /// <summary>
    /// 订单编号
    /// </summary>
    public string? OrderNo { get; set; }

    /// <summary>
    /// 售后类型
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public string? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public string? EndTime { get; set; }
}