using EasyWeChatModels.Enums;
using EasyWeChatModels.Models.AntWorkflow;

namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 会签节点配置
/// </summary>
public class CounterSignNodeConfig
{
    /// <summary>节点ID</summary>
    public Guid Id { get; set; }

    /// <summary>节点名称</summary>
    public string Name { get; set; } = "会签";

    /// <summary>节点类型</summary>
    public AntNodeType Type { get; set; } = AntNodeType.CounterSign;

    /// <summary>审批人设置类型</summary>
    public ApproverSetType SetType { get; set; } = ApproverSetType.FixedUser;

    /// <summary>审批人列表</summary>
    public List<NodeUser> NodeUserList { get; set; } = new();

    /// <summary>通过条件配置</summary>
    public PassConditionConfig PassCondition { get; set; } = new();

    /// <summary>无处理人时的操作</summary>
    public NoHandlerAction NoHandlerAction { get; set; } = NoHandlerAction.AutoPass;

    /// <summary>主管层级（当 SetType = MultiSupervisor 时）</summary>
    public int? DirectorLevel { get; set; }
}

/// <summary>
/// 通过条件配置
/// </summary>
public class PassConditionConfig
{
    /// <summary>条件类型：all/percent/count</summary>
    public string Type { get; set; } = "all";

    /// <summary>通过比例（如60表示60%）</summary>
    public int? Percent { get; set; }

    /// <summary>通过数量</summary>
    public int? Count { get; set; }
}