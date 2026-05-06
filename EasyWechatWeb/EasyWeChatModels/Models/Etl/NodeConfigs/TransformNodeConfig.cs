namespace EasyWeChatModels.Models.Etl.NodeConfigs;

/// <summary>
/// 转换节点配置
/// </summary>
public class TransformNodeConfig
{
    /// <summary>转换类型：mapping, filter, aggregate, script</summary>
    public string TransformType { get; set; } = "mapping";

    /// <summary>输入变量名</summary>
    public string InputVariable { get; set; } = string.Empty;

    /// <summary>输出变量名</summary>
    public string OutputVariable { get; set; } = "result";

    /// <summary>字段映射（mapping 模式）</summary>
    public List<FieldMappingItem>? FieldMapping { get; set; }

    /// <summary>过滤表达式（filter 模式）</summary>
    public string? FilterExpression { get; set; }

    /// <summary>聚合配置（aggregate 模式）</summary>
    public AggregateConfig? AggregateConfig { get; set; }

    /// <summary>脚本内容（script 模式）</summary>
    public string? Script { get; set; }

    /// <summary>脚本语言：javascript, python, sql</summary>
    public string? ScriptLanguage { get; set; }
}