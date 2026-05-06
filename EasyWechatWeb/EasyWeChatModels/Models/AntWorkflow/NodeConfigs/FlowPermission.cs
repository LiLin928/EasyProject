namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 流程权限（发起人权限配置）
/// </summary>
public class FlowPermission
{
    /// <summary>目标ID</summary>
    public Guid TargetId { get; set; }

    /// <summary>名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>类型</summary>
    public int Type { get; set; }
}