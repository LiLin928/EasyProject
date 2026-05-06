using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace CommonManager.Logging;

/// <summary>
/// HTTP 请求上下文 Enricher
/// 将请求路径、方法、用户信息、IP地址、执行时长等信息添加到日志中
/// </summary>
/// <remarks>
/// 使用方式：
/// 1. 在 Serilog 配置中添加：.Enrich.With<HttpRequestEnricher>()
/// 2. 在请求处理开始时调用：HttpRequestContext.BeginRequest(httpContext)
/// 3. 在请求处理结束时调用：HttpRequestContext.EndRequest()
/// </remarks>
public class HttpRequestEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        // 从 HttpRequestContext 获取当前请求信息
        var context = HttpRequestContext.Current;

        if (context != null)
        {
            // 添加请求路径
            if (!string.IsNullOrEmpty(context.RequestPath))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestPath", context.RequestPath));
            }

            // 添加请求方法
            if (!string.IsNullOrEmpty(context.Method))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Method", context.Method));
            }

            // 添加用户ID
            if (!string.IsNullOrEmpty(context.UserId))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserId", context.UserId));
            }

            // 添加用户名
            if (!string.IsNullOrEmpty(context.UserName))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserName", context.UserName));
            }

            // 添加IP地址
            if (!string.IsNullOrEmpty(context.IpAddress))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("IpAddress", context.IpAddress));
            }

            // 添加执行时长（毫秒）
            if (context.Duration.HasValue)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Duration", context.Duration.Value));
            }
        }
    }
}

/// <summary>
/// HTTP 请求上下文
/// 用于在请求处理过程中存储和传递请求信息
/// </summary>
public static class HttpRequestContext
{
    private static readonly AsyncLocal<RequestContext> _currentContext = new AsyncLocal<RequestContext>();

    /// <summary>
    /// 当前请求上下文
    /// </summary>
    public static RequestContext? Current => _currentContext.Value;

    /// <summary>
    /// 开始请求处理，初始化上下文
    /// </summary>
    public static RequestContext BeginRequest(Microsoft.AspNetCore.Http.HttpContext httpContext)
    {
        var context = new RequestContext
        {
            RequestPath = httpContext.Request.Path.Value ?? "",
            Method = httpContext.Request.Method,
            StartTime = Stopwatch.StartNew()
        };

        // 尝试从 Claims 中获取用户信息
        if (httpContext.User?.Identity?.IsAuthenticated == true)
        {
            context.UserId = httpContext.User.FindFirst("UserId")?.Value ?? "";
            context.UserName = httpContext.User.FindFirst("UserName")?.Value ?? "";
        }

        // 获取IP地址
        context.IpAddress = GetClientIpAddress(httpContext);

        _currentContext.Value = context;
        return context;
    }

    /// <summary>
    /// 结束请求处理，计算执行时长
    /// </summary>
    public static void EndRequest()
    {
        var context = _currentContext.Value;
        if (context != null && context.StartTime != null)
        {
            context.StartTime.Stop();
            context.Duration = context.StartTime.ElapsedMilliseconds;
        }
    }

    /// <summary>
    /// 清除当前请求上下文
    /// </summary>
    public static void Clear()
    {
        _currentContext.Value = null!;
    }

    /// <summary>
    /// 获取客户端IP地址
    /// </summary>
    private static string GetClientIpAddress(Microsoft.AspNetCore.Http.HttpContext httpContext)
    {
        // 尝试从 X-Forwarded-For 头获取（代理场景）
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            // X-Forwarded-For 可能包含多个IP，取第一个
            var ip = forwardedFor.Split(',').FirstOrDefault()?.Trim();
            if (!string.IsNullOrEmpty(ip))
            {
                return ip;
            }
        }

        // 尝试从 X-Real-IP 头获取
        var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        // 使用连接的远程IP地址
        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "";
    }
}

/// <summary>
/// 请求上下文数据
/// </summary>
public class RequestContext
{
    public string RequestPath { get; set; } = "";
    public string Method { get; set; } = "";
    public string UserId { get; set; } = "";
    public string UserName { get; set; } = "";
    public string IpAddress { get; set; } = "";
    public Stopwatch? StartTime { get; set; }
    public long? Duration { get; set; }
}