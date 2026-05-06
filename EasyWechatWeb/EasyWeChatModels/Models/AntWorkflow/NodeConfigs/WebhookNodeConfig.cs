using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// Webhook节点配置
/// </summary>
public class WebhookNodeConfig
{
    /// <summary>节点ID</summary>
    public Guid Id { get; set; }

    /// <summary>节点名称</summary>
    public string Name { get; set; } = "Webhook";

    /// <summary>节点类型</summary>
    public AntNodeType Type { get; set; } = AntNodeType.Webhook;

    /// <summary>Webhook URL地址</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>请求方法</summary>
    public string Method { get; set; } = "POST";

    /// <summary>请求头</summary>
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>请求体</summary>
    public string? Body { get; set; }

    /// <summary>触发时机：before/after/manual</summary>
    public string Trigger { get; set; } = "after";

    /// <summary>超时时间（毫秒）</summary>
    public int Timeout { get; set; } = 30000;

    /// <summary>认证配置</summary>
    public WebhookAuthConfig? AuthConfig { get; set; }

    /// <summary>重试配置</summary>
    public WebhookRetryConfig? RetryConfig { get; set; }

    /// <summary>字段映射</summary>
    public List<FieldMapping>? FieldMappings { get; set; }
}

/// <summary>
/// Webhook认证配置
/// </summary>
public class WebhookAuthConfig
{
    /// <summary>认证类型：none/basic/bearer/api_key</summary>
    public string Type { get; set; } = "none";

    /// <summary>用户名（Basic认证）</summary>
    public string? Username { get; set; }

    /// <summary>密码（Basic认证）</summary>
    public string? Password { get; set; }

    /// <summary>Token（Bearer认证）</summary>
    public string? Token { get; set; }

    /// <summary>Key名称（API Key认证）</summary>
    public string? KeyName { get; set; }

    /// <summary>Key值（API Key认证）</summary>
    public string? KeyValue { get; set; }

    /// <summary>添加位置：header/query</summary>
    public string? AddTo { get; set; } = "header";
}

/// <summary>
/// Webhook重试配置
/// </summary>
public class WebhookRetryConfig
{
    /// <summary>重试次数</summary>
    public int Count { get; set; } = 0;

    /// <summary>重试间隔（毫秒）</summary>
    public int Interval { get; set; } = 1000;
}