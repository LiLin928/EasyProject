namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// Ant流程实例 DTO
/// </summary>
public class AntWorkflowInstanceDto
{
    /// <summary>实例ID</summary>
    public Guid Id { get; set; }

    /// <summary>流程定义ID</summary>
    public Guid WorkflowId { get; set; }

    /// <summary>流程名称</summary>
    public string? WorkflowName { get; set; }

    /// <summary>流程标题</summary>
    public string? Title { get; set; }

    /// <summary>业务单据ID</summary>
    public string? BusinessId { get; set; }

    /// <summary>业务类型编码</summary>
    public string? BusinessType { get; set; }

    /// <summary>状态</summary>
    public int Status { get; set; }

    /// <summary>发起人ID</summary>
    public Guid? InitiatorId { get; set; }

    /// <summary>发起人姓名</summary>
    public string? InitiatorName { get; set; }

    /// <summary>开始时间</summary>
    public DateTime? StartTime { get; set; }

    /// <summary>完成时间</summary>
    public DateTime? FinishTime { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreateTime { get; set; }
}