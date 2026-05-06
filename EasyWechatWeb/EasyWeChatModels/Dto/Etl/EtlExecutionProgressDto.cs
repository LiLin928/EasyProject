namespace EasyWeChatModels.Dto.Etl;

/// <summary>
/// 执行进度 DTO（用于实时进度查询）
/// </summary>
public class EtlExecutionProgressDto
{
    /// <summary>执行记录ID</summary>
    public Guid ExecutionId { get; set; }

    /// <summary>执行状态</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>进度百分比 (0-100)</summary>
    public int Progress { get; set; }

    /// <summary>已完成节点数</summary>
    public int CompletedNodes { get; set; }

    /// <summary>总节点数</summary>
    public int TotalNodes { get; set; }

    /// <summary>当前执行的节点ID</summary>
    public string? CurrentNodeId { get; set; }

    /// <summary>当前执行的节点名称</summary>
    public string? CurrentNodeName { get; set; }

    /// <summary>开始时间</summary>
    public string? StartTime { get; set; }

    /// <summary>已执行时长（毫秒）</summary>
    public long? ElapsedTime { get; set; }

    /// <summary>预估剩余时间（毫秒）</summary>
    public long? EstimatedRemainingTime { get; set; }
}