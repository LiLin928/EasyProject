using System;

namespace EasyWeChatModels.Dto.LogQuery;

/// <summary>
/// 日志查询请求参数
/// </summary>
public class LogQueryRequestDto
{
    /// <summary>
    /// 查询环境标识（Development/Production）
    /// </summary>
    public string Environment { get; set; } = "Development";

    /// <summary>
    /// 开始时间，不指定则默认近 7 天
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间，不指定则默认今天
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 日志级别：Debug/Information/Warning/Error/Fatal
    /// </summary>
    public string? Level { get; set; }

    /// <summary>
    /// 请求路径（模糊匹配）
    /// </summary>
    public string? RequestPath { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// HTTP 方法：GET/POST/PUT/DELETE 等
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// IP 地址
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// 消息关键字（全文搜索）
    /// </summary>
    public string? MessageKeyword { get; set; }

    /// <summary>
    /// 异常堆栈关键字（全文搜索）
    /// </summary>
    public string? ExceptionKeyword { get; set; }

    /// <summary>
    /// 来源机器名
    /// </summary>
    public string? MachineName { get; set; }

    /// <summary>
    /// 最小执行时长（毫秒）
    /// </summary>
    public int? MinDuration { get; set; }

    /// <summary>
    /// 最大执行时长（毫秒）
    /// </summary>
    public int? MaxDuration { get; set; }

    /// <summary>
    /// 页码，从 1 开始
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量，默认 20
    /// </summary>
    public int PageSize { get; set; } = 20;
}