using EasyWeChatModels.Enums;
using Newtonsoft.Json.Linq;

namespace EasyWeChatModels.Models;

/// <summary>
/// DAG 节点（对应前端 DagNode）
/// </summary>
public class DagNode
{
    /// <summary>节点ID（字符串，如 node_start）</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>节点名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>节点类型</summary>
    public AntNodeType Type { get; set; }

    /// <summary>位置坐标</summary>
    public DagPosition Position { get; set; } = new();

    /// <summary>节点配置（JSON对象，使用JToken接受任意类型）</summary>
    public JToken? Config { get; set; }
}