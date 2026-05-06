namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// Ant流程版本 DTO
/// </summary>
public class AntWorkflowVersionDto
{
    /// <summary>版本ID</summary>
    public Guid Id { get; set; }

    /// <summary>流程ID</summary>
    public Guid WorkflowId { get; set; }

    /// <summary>版本号</summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>流程配置JSON</summary>
    public string? FlowConfig { get; set; }

    /// <summary>发布时间</summary>
    public DateTime? PublishTime { get; set; }

    /// <summary>发布人ID</summary>
    public Guid? PublisherId { get; set; }

    /// <summary>发布人姓名</summary>
    public string? PublisherName { get; set; }

    /// <summary>版本状态</summary>
    public int Status { get; set; }

    /// <summary>版本备注</summary>
    public string? Remark { get; set; }
}