namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 节点用户
/// </summary>
public class NodeUser
{
    /// <summary>目标ID（用户ID或角色ID）</summary>
    public Guid TargetId { get; set; }

    /// <summary>显示名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>类型：1用户/2角色</summary>
    public int Type { get; set; } = 1;
}