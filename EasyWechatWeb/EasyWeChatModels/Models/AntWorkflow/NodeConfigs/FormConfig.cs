namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 表单配置
/// </summary>
public class FormConfig
{
    /// <summary>表单字段列表</summary>
    public List<FormFieldConfig> Fields { get; set; } = new();

    /// <summary>表单验证规则</summary>
    public List<FormRule>? Rules { get; set; }
}