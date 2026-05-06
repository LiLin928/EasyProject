namespace EasyWeChatModels.Dto;

/// <summary>
/// 任务日志查询参数
/// </summary>
public class TaskLogQueryDto
{
    /// <summary>
    /// 任务名称
    /// </summary>
    public string? JobName { get; set; }

    /// <summary>
    /// 任务分组
    /// </summary>
    public string? JobGroup { get; set; }

    /// <summary>
    /// 执行状态
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 开始时间范围-起始
    /// </summary>
    public DateTime? StartTimeBegin { get; set; }

    /// <summary>
    /// 开始时间范围-结束
    /// </summary>
    public DateTime? StartTimeEnd { get; set; }

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