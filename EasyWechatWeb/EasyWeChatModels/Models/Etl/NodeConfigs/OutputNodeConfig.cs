namespace EasyWeChatModels.Models.Etl.NodeConfigs;

/// <summary>
/// 输出节点配置
/// </summary>
public class OutputNodeConfig
{
    /// <summary>目标数据源ID</summary>
    public string DatasourceId { get; set; } = string.Empty;

    /// <summary>输出类型：insert, update, merge, truncate_insert</summary>
    public string OutputType { get; set; } = "insert";

    /// <summary>目标表名</summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>输入变量名</summary>
    public string InputVariable { get; set; } = string.Empty;

    /// <summary>字段映射</summary>
    public List<FieldMappingItem>? FieldMapping { get; set; }

    /// <summary>批次大小</summary>
    public int? BatchSize { get; set; }

    /// <summary>冲突处理策略：skip, update, error</summary>
    public string? OnConflict { get; set; }
}