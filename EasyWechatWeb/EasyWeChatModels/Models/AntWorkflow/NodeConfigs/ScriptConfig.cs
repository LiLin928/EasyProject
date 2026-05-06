namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 脚本配置
/// </summary>
public class ScriptConfig
{
    /// <summary>脚本内容</summary>
    public string Script { get; set; } = string.Empty;

    /// <summary>脚本格式：javascript/groovy/python</summary>
    public string Format { get; set; } = "javascript";

    /// <summary>结果变量名，用于存储脚本执行结果到 FormData</summary>
    public string? ResultVariable { get; set; }
}