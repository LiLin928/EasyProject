using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 审批人节点配置
/// </summary>
public class ApproverNodeConfig
{
    /// <summary>节点ID</summary>
    public Guid Id { get; set; }

    /// <summary>节点名称</summary>
    public string Name { get; set; } = "审批人";

    /// <summary>节点类型</summary>
    public AntNodeType Type { get; set; } = AntNodeType.Approver;

    /// <summary>审批人设置类型</summary>
    public ApproverSetType SetType { get; set; } = ApproverSetType.FixedUser;

    /// <summary>审批人列表</summary>
    public List<NodeUser> NodeUserList { get; set; } = new();

    /// <summary>主管层级</summary>
    public int? DirectorLevel { get; set; }

    /// <summary>审批模式</summary>
    public ExamineMode ExamineMode { get; set; } = ExamineMode.Sequential;

    /// <summary>无处理人时的操作</summary>
    public NoHandlerAction NoHandlerAction { get; set; } = NoHandlerAction.AutoPass;

    /// <summary>超时设置（小时）</summary>
    public int? Timeout { get; set; }

    /// <summary>超时动作：autoPass/autoReject/transfer</summary>
    public string? TimeoutAction { get; set; }

    /// <summary>选择模式</summary>
    public int? SelectMode { get; set; }

    /// <summary>选择范围</summary>
    public int? SelectRange { get; set; }
}