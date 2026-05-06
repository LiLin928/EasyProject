namespace EasyWeChatModels.Models;

/// <summary>
/// DAG 全局配置
/// </summary>
public class DagGlobalConfig
{
    /// <summary>最大并发数</summary>
    public int? MaxConcurrency { get; set; }

    /// <summary>超时时间（秒）</summary>
    public int? Timeout { get; set; }

    /// <summary>重试次数</summary>
    public int? RetryTimes { get; set; }

    /// <summary>重试间隔（秒）</summary>
    public int? RetryInterval { get; set; }

    /// <summary>失败策略：stop/continue</summary>
    public string? FailureStrategy { get; set; }
}