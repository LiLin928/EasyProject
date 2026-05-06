using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 服务任务节点配置
/// </summary>
public class ServiceNodeConfig
{
    /// <summary>节点ID</summary>
    public Guid Id { get; set; }

    /// <summary>节点名称</summary>
    public string Name { get; set; } = "服务任务";

    /// <summary>节点类型</summary>
    public AntNodeType Type { get; set; } = AntNodeType.Service;

    /// <summary>任务类型：api/script/webhook</summary>
    public string TaskType { get; set; } = "api";

    /// <summary>API调用配置</summary>
    public ApiConfig? ApiConfig { get; set; }

    /// <summary>脚本配置</summary>
    public ScriptConfig? ScriptConfig { get; set; }

    /// <summary>错误处理配置</summary>
    public ErrorHandlingConfig? ErrorHandling { get; set; }
}