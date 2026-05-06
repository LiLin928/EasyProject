namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// API调用配置
/// </summary>
public class ApiConfig
{
    /// <summary>接口地址</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>请求方法</summary>
    public string Method { get; set; } = "POST";

    /// <summary>请求头</summary>
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>请求体</summary>
    public string? Body { get; set; }

    /// <summary>字段映射</summary>
    public List<FieldMapping>? FieldMappings { get; set; }
}