namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 并行分支
/// </summary>
public class ParallelBranch
{
    /// <summary>分支ID（字符串）</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>分支名称</summary>
    public string Name { get; set; } = string.Empty;
}