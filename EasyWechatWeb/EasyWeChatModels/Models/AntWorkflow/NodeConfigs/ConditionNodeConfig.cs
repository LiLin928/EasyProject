using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 条件分支节点配置
/// </summary>
public class ConditionNodeConfig
{
    /// <summary>节点ID（字符串）</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>节点名称</summary>
    public string Name { get; set; } = "条件分支";

    /// <summary>节点类型</summary>
    public AntNodeType Type { get; set; } = AntNodeType.Condition;

    /// <summary>条件分支列表</summary>
    public List<ConditionBranch> ConditionNodes { get; set; } = new();
}