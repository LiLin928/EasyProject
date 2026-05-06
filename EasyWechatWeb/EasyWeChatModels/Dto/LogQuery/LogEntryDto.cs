using System;

namespace EasyWeChatModels.Dto.LogQuery;

/// <summary>
/// 日志条目（列表展示）
/// </summary>
public class LogEntryDto
{
    /// <summary>
    /// ES 文档 ID
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// 日志时间戳
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 日志级别
    /// </summary>
    public string Level { get; set; } = "";

    /// <summary>
    /// 日志消息
    /// </summary>
    public string Message { get; set; } = "";

    /// <summary>
    /// 请求路径
    /// </summary>
    public string? RequestPath { get; set; }

    /// <summary>
    /// HTTP 方法
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// IP 地址
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// 执行时长（毫秒）
    /// </summary>
    public int? Duration { get; set; }

    /// <summary>
    /// 来源机器名
    /// </summary>
    public string? MachineName { get; set; }

    /// <summary>
    /// 环境名称
    /// </summary>
    public string? Environment { get; set; }

    /// <summary>
    /// 异常信息摘要（前 200 字符）
    /// </summary>
    public string? Exception { get; set; }
}