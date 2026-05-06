namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// Ant执行日志 DTO
/// </summary>
public class AntExecutionLogDto
{
    /// <summary>记录ID</summary>
    public Guid Id { get; set; }

    /// <summary>节点名称</summary>
    public string? NodeName { get; set; }

    /// <summary>节点类型</summary>
    public int? NodeType { get; set; }

    /// <summary>审批人姓名</summary>
    public string? HandlerName { get; set; }

    /// <summary>审批状态</summary>
    public int? ApproveStatus { get; set; }

    /// <summary>审批意见</summary>
    public string? ApproveDesc { get; set; }

    /// <summary>审批时间</summary>
    public DateTime? ApproveTime { get; set; }

    /// <summary>处理时长（秒）</summary>
    public int? Duration { get; set; }

    /// <summary>转交给用户姓名</summary>
    public string? TransferToName { get; set; }
}