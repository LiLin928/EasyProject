using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 发起人节点配置
/// </summary>
public class StartNodeConfig
{
    /// <summary>节点ID</summary>
    public Guid Id { get; set; }

    /// <summary>节点名称</summary>
    public string Name { get; set; } = "发起人";

    /// <summary>节点类型</summary>
    public AntNodeType Type { get; set; } = AntNodeType.Start;

    /// <summary>发起人权限配置</summary>
    public List<FlowPermission>? Permissions { get; set; }

    /// <summary>表单配置</summary>
    public FormConfig? FormConfig { get; set; }
}