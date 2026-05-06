namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// 查询已办任务 DTO
/// </summary>
public class QueryDoneTaskDto
{
    /// <summary>页码</summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>每页数量</summary>
    public int PageSize { get; set; } = 10;

    /// <summary>流程名称（模糊查询）</summary>
    public string? WorkflowName { get; set; }

    /// <summary>开始时间</summary>
    public DateTime? StartTime { get; set; }

    /// <summary>结束时间</summary>
    public DateTime? EndTime { get; set; }
}