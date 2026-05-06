namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 字段映射（用于服务任务和Webhook）
/// </summary>
public class FieldMapping
{
    /// <summary>源字段</summary>
    public string SourceField { get; set; } = string.Empty;

    /// <summary>目标字段</summary>
    public string TargetField { get; set; } = string.Empty;

    /// <summary>转换规则</summary>
    public string? Transform { get; set; }
}