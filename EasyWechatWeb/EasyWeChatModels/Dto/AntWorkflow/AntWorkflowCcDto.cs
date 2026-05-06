namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// Ant抄送 DTO
/// </summary>
public class AntWorkflowCcDto
{
    /// <summary>抄送记录ID</summary>
    public Guid Id { get; set; }

    /// <summary>实例ID</summary>
    public Guid InstanceId { get; set; }

    /// <summary>流程标题</summary>
    public string? InstanceTitle { get; set; }

    /// <summary>节点名称</summary>
    public string? NodeName { get; set; }

    /// <summary>发送人姓名</summary>
    public string? FromUserName { get; set; }

    /// <summary>发送时间</summary>
    public DateTime? SendTime { get; set; }

    /// <summary>是否已读</summary>
    public int IsRead { get; set; }
}