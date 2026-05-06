namespace EasyWeChatModels.Dto;

/// <summary>
/// 任务定义查询参数 DTO
/// </summary>
public class QueryTaskDefinitionDto
{
    /// <summary>
    /// 页码
    /// </summary>
    /// <example>1</example>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    /// <example>10</example>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 任务名称（模糊搜索）
    /// </summary>
    public string? TaskName { get; set; }

    /// <summary>
    /// 任务类型（0:Cron, 1:即时, 2:周期）
    /// </summary>
    public int? TaskType { get; set; }

    /// <summary>
    /// 状态（0:待调度, 1:已调度, 2:暂停, 3:完成, 4:失败）
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 任务分组
    /// </summary>
    public string? TaskGroup { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public string? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public string? EndTime { get; set; }
}