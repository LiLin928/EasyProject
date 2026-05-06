namespace EasyWeChatModels.Models;

/// <summary>
/// 边条件
/// </summary>
public class DagEdgeCondition
{
    /// <summary>分支ID（字符串）</summary>
    public string BranchId { get; set; } = string.Empty;

    /// <summary>分支名称</summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>优先级</summary>
    public int Priority { get; set; }

    /// <summary>是否默认分支</summary>
    public bool IsDefault { get; set; }
}