namespace EasyWeChatModels.Dto;

/// <summary>
/// CAP 日志查询参数
/// </summary>
public class CapLogQueryDto
{
    /// <summary>
    /// 主题名称
    /// </summary>
    public string? Topic { get; set; }

    /// <summary>
    /// 消息状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public int? MessageType { get; set; }

    /// <summary>
    /// 创建时间范围-起始
    /// </summary>
    public DateTime? CreateTimeBegin { get; set; }

    /// <summary>
    /// 创建时间范围-结束
    /// </summary>
    public DateTime? CreateTimeEnd { get; set; }

    /// <summary>
    /// 页码
    /// </summary>
    /// <example>1</example>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    /// <example>20</example>
    public int PageSize { get; set; } = 20;
}