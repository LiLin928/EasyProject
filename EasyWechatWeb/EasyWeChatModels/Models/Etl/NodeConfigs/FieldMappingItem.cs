namespace EasyWeChatModels.Models.Etl.NodeConfigs;

/// <summary>
/// 字段映射项（用于字段映射转换）
/// </summary>
public class FieldMappingItem
{
    /// <summary>源字段名</summary>
    public string SourceField { get; set; } = string.Empty;

    /// <summary>目标字段名</summary>
    public string TargetField { get; set; } = string.Empty;

    /// <summary>转换表达式（可选）</summary>
    public string? Transform { get; set; }

    /// <summary>数据类型（可选）</summary>
    public string? DataType { get; set; }

    /// <summary>是否为主键字段（用于 UPDATE/MERGE）</summary>
    public bool IsKey { get; set; } = false;
}