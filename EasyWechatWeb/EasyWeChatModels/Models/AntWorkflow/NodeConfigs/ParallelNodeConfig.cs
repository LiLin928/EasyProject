using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 并行分支节点配置
/// </summary>
public class ParallelNodeConfig
{
    /// <summary>节点ID（字符串）</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>节点名称</summary>
    public string Name { get; set; } = "并行分支";

    /// <summary>节点类型</summary>
    public AntNodeType Type { get; set; } = AntNodeType.Parallel;

    /// <summary>并行分支列表</summary>
    public List<ParallelBranch> ParallelNodes { get; set; } = new();

    /// <summary>完成条件：all/any/count</summary>
    public string CompleteCondition { get; set; } = "all";

    /// <summary>完成数量（用于 count 条件）</summary>
    public int? CompleteCount { get; set; }
}