namespace EasyWeChatModels.Dto.Etl;

/// <summary>
/// 节点执行结果 DTO
/// </summary>
public class EtlNodeResultDto
{
    /// <summary>是否成功</summary>
    public bool Success { get; set; }

    /// <summary>节点ID</summary>
    public string NodeId { get; set; } = string.Empty;

    /// <summary>节点名称</summary>
    public string? NodeName { get; set; }

    /// <summary>输出变量（JSON格式）</summary>
    public string? OutputsJson { get; set; }

    /// <summary>错误信息</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>处理的数据行数</summary>
    public int? ProcessedRows { get; set; }

    /// <summary>执行耗时（毫秒）</summary>
    public long? Duration { get; set; }

    /// <summary>重试次数</summary>
    public int RetryCount { get; set; }
}