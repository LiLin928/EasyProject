namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 错误处理配置
/// </summary>
public class ErrorHandlingConfig
{
    /// <summary>错误处理策略：stop/continue/retry</summary>
    public string Strategy { get; set; } = "stop";

    /// <summary>重试次数</summary>
    public int RetryCount { get; set; }

    /// <summary>重试间隔（秒）</summary>
    public int RetryInterval { get; set; }
}