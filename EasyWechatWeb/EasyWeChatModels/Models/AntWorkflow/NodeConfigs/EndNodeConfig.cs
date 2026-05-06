using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 结束节点配置
/// </summary>
public class EndNodeConfig
{
    /// <summary>节点ID</summary>
    public Guid Id { get; set; }

    /// <summary>节点名称</summary>
    public string Name { get; set; } = "结束";

    /// <summary>节点类型</summary>
    public AntNodeType Type { get; set; } = AntNodeType.End;

    /// <summary>结束类型：success/reject/cancel</summary>
    public string EndType { get; set; } = "success";

    /// <summary>结束通知配置</summary>
    public NotificationConfig? Notification { get; set; }
}