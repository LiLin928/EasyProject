using CommonManager.Logging;
using Microsoft.AspNetCore.Http.Extensions;
using Serilog;

namespace EasyWeChatWeb.Middleware;

/// <summary>
/// 请求日志中间件
/// 记录每个 HTTP 请求的详细信息和响应状态
/// 使用 Serilog 结构化日志，支持 Elasticsearch 查询
/// </summary>
/// <remarks>
/// 该中间件会通过 HttpRequestContext 记录以下字段到 Elasticsearch：
/// - RequestPath: 请求路径
/// - Method: HTTP 方法
/// - UserId: 用户ID
/// - UserName: 用户名
/// - IpAddress: 客户端IP地址
/// - Duration: 执行时长（毫秒）
///
/// 日志级别：
/// - 正常请求：Information 级别
/// - 错误响应：Warning 级别（状态码 >= 400）
/// - 异常：Error 级别
/// </remarks>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 开始请求，初始化上下文
        var requestContext = HttpRequestContext.BeginRequest(context);

        var requestMethod = context.Request.Method;
        var requestPath = context.Request.Path;
        var requestQuery = context.Request.QueryString.Value ?? "";

        Log.Information("HTTP {Method} {Path}{Query} - Request Started",
            requestMethod, requestPath, requestQuery);

        var originalBodyStream = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // 记录异常
            Log.Error(ex, "HTTP {Method} {Path} - Request Failed",
                requestMethod, requestPath);
            throw;
        }

        // 结束请求，计算执行时长
        HttpRequestContext.EndRequest();

        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream);
        var responseBody = await reader.ReadToEndAsync();
        memoryStream.Position = 0;
        await memoryStream.CopyToAsync(originalBodyStream);
        context.Response.Body = originalBodyStream;

        var statusCode = context.Response.StatusCode;
        var duration = requestContext.Duration ?? 0;

        // 正常响应
        Log.Information("HTTP {Method} {Path} - Response: {StatusCode}, Duration: {Duration}ms",
            requestMethod, requestPath, statusCode, duration);

        // 错误响应额外记录响应体
        if (statusCode >= 400)
        {
            Log.Warning("HTTP {Method} {Path} - Error Response: {StatusCode}, Duration: {Duration}ms, Body: {Body}",
                requestMethod, requestPath, statusCode, duration, responseBody);
        }

        // 清除请求上下文
        HttpRequestContext.Clear();
    }
}

/// <summary>
/// 请求日志中间件扩展类
/// 提供便捷的中间件注册方法
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// 注册请求日志中间件
    /// </summary>
    /// <param name="builder">IApplicationBuilder 应用构建器</param>
    /// <returns>IApplicationBuilder 应用构建器，支持链式调用</returns>
    /// <remarks>
    /// 该中间件通常注册在请求管道的前端，在异常处理中间件之后。
    /// 推荐顺序：
    /// app.UseExceptionHandling();
    /// app.UseRequestLogging();
    /// app.UseRouting();
    ///
    /// 这样可以确保所有请求都被记录，包括异常处理后的响应。
    /// </remarks>
    /// <example>
    /// 在 Program.cs 中使用：
    /// var app = builder.Build();
    /// app.UseExceptionHandling();
    /// app.UseRequestLogging();
    /// </example>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}