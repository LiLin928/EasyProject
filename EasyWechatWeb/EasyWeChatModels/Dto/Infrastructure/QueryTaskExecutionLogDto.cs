namespace EasyWeChatModels.Dto;

/// <summary>
/// 任务执行日志查询参数 DTO
/// </summary>
public class QueryTaskExecutionLogDto
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 任务名称
    /// </summary>
    public string? JobName { get; set; }

    /// <summary>
    /// 执行状态（0:执行中, 1:成功, 2:失败, 3:取消）
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 触发类型（0:Cron触发, 1:手动触发）
    /// </summary>
    public int? TriggerType { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public string? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public string? EndTime { get; set; }
}