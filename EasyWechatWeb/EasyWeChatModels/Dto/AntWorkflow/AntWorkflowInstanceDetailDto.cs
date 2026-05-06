namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// Ant流程实例详情 DTO
/// </summary>
public class AntWorkflowInstanceDetailDto
{
    /// <summary>实例基本信息</summary>
    public AntWorkflowInstanceDto Instance { get; set; } = new();

    /// <summary>业务数据JSON</summary>
    public string? BusinessData { get; set; }

    /// <summary>表单数据JSON</summary>
    public string? FormData { get; set; }

    /// <summary>DAG配置JSON</summary>
    public string? FlowConfig { get; set; }

    /// <summary>节点状态列表</summary>
    public List<AntNodeStatusDto> NodeStatusList { get; set; } = new();

    /// <summary>审批记录列表</summary>
    public List<AntExecutionLogDto> ApproveRecords { get; set; } = new();
}

/// <summary>
/// 节点状态 DTO
/// </summary>
public class AntNodeStatusDto
{
    /// <summary>节点ID（字符串）</summary>
    public string NodeId { get; set; } = string.Empty;

    /// <summary>节点名称</summary>
    public string? NodeName { get; set; }

    /// <summary>节点类型</summary>
    public int NodeType { get; set; }

    /// <summary>审批状态</summary>
    public int ApproveStatus { get; set; }

    /// <summary>当前处理人列表</summary>
    public List<AntHandlerDto>? Handlers { get; set; }
}

/// <summary>
/// 处理人 DTO
/// </summary>
public class AntHandlerDto
{
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
}