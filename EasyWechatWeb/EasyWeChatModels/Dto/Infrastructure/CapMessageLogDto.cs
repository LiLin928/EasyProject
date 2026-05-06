namespace EasyWeChatModels.Dto;

/// <summary>
/// CAP 消息日志 DTO
/// </summary>
public class CapMessageLogDto
{
    /// <summary>
    /// ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public int MessageType { get; set; }

    /// <summary>
    /// 主题名称
    /// </summary>
    public string Topic { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 消息状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态描述
    /// </summary>
    public string StatusText { get; set; } = string.Empty;

    /// <summary>
    /// 重试次数
    /// </summary>
    public int Retries { get; set; }

    /// <summary>
    /// 消费者组 ID
    /// </summary>
    public string? GroupId { get; set; }

    /// <summary>
    /// 异常信息
    /// </summary>
    public string? ExceptionMessage { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 处理时间
    /// </summary>
    public DateTime? ProcessTime { get; set; }
}