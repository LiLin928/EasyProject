namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 子流程节点配置
/// </summary>
public class SubflowNodeConfig
{
    /// <summary>子流程ID</summary>
    public Guid SubflowId { get; set; }

    /// <summary>是否传递表单数据</summary>
    public bool PassFormData { get; set; } = true;

    /// <summary>是否等待子流程完成</summary>
    public bool WaitForCompletion { get; set; } = true;

    /// <summary>输入参数映射（父流程 -> 子流程）</summary>
    public List<ParameterMapping>? InputMappings { get; set; }

    /// <summary>输出参数映射（子流程 -> 父流程）</summary>
    public List<ParameterMapping>? OutputMappings { get; set; }
}

/// <summary>
/// 参数映射配置
/// </summary>
public class ParameterMapping
{
    /// <summary>源字段名</summary>
    public string? SourceField { get; set; }

    /// <summary>目标字段名</summary>
    public string? TargetField { get; set; }
}