using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 抄送人节点配置
/// </summary>
public class CopyerNodeConfig
{
    /// <summary>节点ID</summary>
    public Guid Id { get; set; }

    /// <summary>节点名称</summary>
    public string Name { get; set; } = "抄送人";

    /// <summary>节点类型</summary>
    public AntNodeType Type { get; set; } = AntNodeType.Copyer;

    /// <summary>抄送人列表</summary>
    public List<NodeUser> NodeUserList { get; set; } = new();

    /// <summary>是否允许自选抄送人</summary>
    public bool AllowSelfSelect { get; set; }
}