namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 条件分支
/// </summary>
public class ConditionBranch
{
    /// <summary>分支ID（字符串）</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>分支名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>优先级</summary>
    public int Priority { get; set; }

    /// <summary>条件规则列表</summary>
    public List<ConditionRule> ConditionRules { get; set; } = new();

    /// <summary>是否默认分支</summary>
    public bool IsDefault { get; set; }
}