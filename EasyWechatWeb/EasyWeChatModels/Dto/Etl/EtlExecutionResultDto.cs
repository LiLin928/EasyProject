namespace EasyWeChatModels.Dto.Etl;

/// <summary>
/// 任务流执行结果 DTO
/// </summary>
public class EtlExecutionResultDto
{
    /// <summary>执行记录ID</summary>
    public Guid ExecutionId { get; set; }

    /// <summary>执行状态：pending, running, success, failure, cancelled</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>进度百分比</summary>
    public int Progress { get; set; }

    /// <summary>已完成节点数</summary>
    public int CompletedNodes { get; set; }

    /// <summary>总节点数</summary>
    public int TotalNodes { get; set; }

    /// <summary>执行耗时（毫秒）</summary>
    public long? Duration { get; set; }

    /// <summary>错误信息</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>节点执行结果列表</summary>
    public List<EtlNodeResultDto>? NodeResults { get; set; }

    /// <summary>执行开始时间</summary>
    public string? StartTime { get; set; }

    /// <summary>执行结束时间</summary>
    public string? EndTime { get; set; }
}