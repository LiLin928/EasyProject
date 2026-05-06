namespace EasyWeChatModels.Dto;

/// <summary>
/// API 回调载荷
/// </summary>
public class ApiCallbackPayload
{
    /// <summary>
    /// 任务 ID
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// 回调地址
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// HTTP 方法
    /// </summary>
    public string Method { get; set; } = "POST";

    /// <summary>
    /// 请求体（JSON）
    /// </summary>
    public string? Payload { get; set; }

    /// <summary>
    /// 执行日志 ID
    /// </summary>
    public Guid? LogId { get; set; }
}