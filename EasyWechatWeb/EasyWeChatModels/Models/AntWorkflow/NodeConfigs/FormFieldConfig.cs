namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 表单字段配置
/// </summary>
public class FormFieldConfig
{
    /// <summary>字段ID</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>字段名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>字段标签</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>字段类型</summary>
    public string Type { get; set; } = "text";

    /// <summary>是否必填</summary>
    public bool Required { get; set; }

    /// <summary>默认值</summary>
    public object? DefaultValue { get; set; }

    /// <summary>选项列表</summary>
    public List<FieldOption>? Options { get; set; }

    /// <summary>占位符</summary>
    public string? Placeholder { get; set; }
}

/// <summary>
/// 字段选项
/// </summary>
public class FieldOption
{
    /// <summary>选项标签</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>选项值</summary>
    public string Value { get; set; } = string.Empty;
}