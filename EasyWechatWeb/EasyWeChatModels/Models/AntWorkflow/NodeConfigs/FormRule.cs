namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 表单验证规则
/// </summary>
public class FormRule
{
    /// <summary>字段ID</summary>
    public string FieldId { get; set; } = string.Empty;

    /// <summary>规则类型</summary>
    public string Rule { get; set; } = "required";

    /// <summary>规则值</summary>
    public object? Value { get; set; }

    /// <summary>错误消息</summary>
    public string Message { get; set; } = string.Empty;
}