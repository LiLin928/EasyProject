namespace BusinessManager.Buz.Etl.Engine;

/// <summary>
/// ETL 节点执行结果
/// </summary>
public class EtlNodeResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 输出变量（Key: 变量名, Value: 变量值）
    /// </summary>
    public Dictionary<string, object>? Outputs { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 处理的数据行数
    /// </summary>
    public int? ProcessedRows { get; set; }

    /// <summary>
    /// 执行耗时（毫秒）
    /// </summary>
    public long? Duration { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static EtlNodeResult SuccessResult(Dictionary<string, object> outputs, int? processedRows = null, long? duration = null)
    {
        return new EtlNodeResult
        {
            Success = true,
            Outputs = outputs,
            ProcessedRows = processedRows,
            Duration = duration
        };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static EtlNodeResult FailResult(string errorMessage, int retryCount = 0)
    {
        return new EtlNodeResult
        {
            Success = false,
            ErrorMessage = errorMessage,
            RetryCount = retryCount
        };
    }

    /// <summary>
    /// 创建空结果（无输出）
    /// </summary>
    public static EtlNodeResult EmptyResult(long? duration = null)
    {
        return new EtlNodeResult
        {
            Success = true,
            Outputs = new Dictionary<string, object>(),
            Duration = duration
        };
    }
}