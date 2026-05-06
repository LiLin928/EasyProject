namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// Ant任务 DTO
/// </summary>
public class AntWorkflowTaskDto
{
    /// <summary>任务ID</summary>
    public Guid Id { get; set; }

    /// <summary>实例ID</summary>
    public Guid InstanceId { get; set; }

    /// <summary>流程标题</summary>
    public string? InstanceTitle { get; set; }

    /// <summary>节点ID（字符串）</summary>
    public string NodeId { get; set; } = string.Empty;

    /// <summary>节点名称</summary>
    public string? NodeName { get; set; }

    /// <summary>节点类型</summary>
    public int NodeType { get; set; }

    /// <summary>进入时间</summary>
    public DateTime? EntryTime { get; set; }

    /// <summary>截止时间</summary>
    public DateTime? DueTime { get; set; }

    /// <summary>发起人ID</summary>
    public Guid? InitiatorId { get; set; }

    /// <summary>发起人姓名</summary>
    public string? InitiatorName { get; set; }

    /// <summary>任务类型</summary>
    public int TaskType { get; set; }
}