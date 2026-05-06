namespace EasyWeChatModels.Models;

/// <summary>
/// DAG 配置（对应前端 DagConfig）
/// </summary>
public class DagConfig
{
    /// <summary>配置版本号</summary>
    public string Version { get; set; } = "1.0";

    /// <summary>节点列表</summary>
    public List<DagNode> Nodes { get; set; } = new();

    /// <summary>连线列表</summary>
    public List<DagEdge> Edges { get; set; } = new();

    /// <summary>全局配置</summary>
    public DagGlobalConfig? GlobalConfig { get; set; }
}