using CommonManager.Base;
using CommonManager.Error;
using System.Net;
using System.Text.Json;

namespace EasyWeChatWeb.Middleware;

/// <summary>
/// 全局异常处理中间件
/// 捕获并统一处理应用程序中未处理的异常
/// </summary>
/// <remarks>
/// 该中间件位于请求管道的最前端，捕获所有未处理的异常并返回统一的错误响应。
///
/// 异常处理策略：
/// - BusinessException: 业务异常，返回业务错误消息和错误码
/// - ArgumentException: 参数异常，返回参数错误消息和400状态码
/// - UnauthorizedAccessException: 授权异常，返回"未授权访问"和401状态码
/// - Exception: 其他异常，返回"服务器内部错误"和500状态码
///
/// 所有异常都会被记录到日志中。
/// </remarks>
/// <example>
/// 在 Program.cs 中使用：
/// app.UseExceptionHandling();
/// </example>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// 初始化异常处理中间件
    /// </summary>
    /// <param name="next">下一个请求处理委托</param>
    /// <param name="logger">日志记录器</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 处理 HTTP 请求，捕获并处理异常
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "业务异常: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex.Message, ex.ErrorCode);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "参数异常: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex.Message, 400);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "授权异常: {Message}", ex.Message);
            await HandleExceptionAsync(context, "未授权访问", 401);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "未处理的异常: {Message}", ex.Message);
            await HandleExceptionAsync(context, "服务器内部错误", 500);
        }
    }

    /// <summary>
    /// 处理异常并返回统一格式的错误响应
    /// </summary>
    /// <param name="context">HTTP 上下文对象</param>
    /// <param name="message">错误消息</param>
    /// <param name="statusCode">HTTP 状态码</param>
    /// <remarks>
    /// 返回的响应格式：
    /// {
    ///     "code": 状态码,
    ///     "message": 错误消息,
    ///     "data": null
    /// }
    ///
    /// 注意：对于 401 状态码，返回 HTTP 401；其他状态码返回 HTTP 200，
    /// 这是为了与 JWT 中间件的处理方式保持一致。
    /// </remarks>
    private static async Task HandleExceptionAsync(HttpContext context, string message, int statusCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode == 401 ? 401 : 200; // JWT middleware handles 401 differently

        var response = ApiResponse<object>.Error(message, statusCode);
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

/// <summary>
/// 异常处理中间件扩展类
/// 提供便捷的中间件注册方法
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// 注册异常处理中间件
    /// </summary>
    /// <param name="builder">IApplicationBuilder 应用构建器</param>
    /// <returns>IApplicationBuilder 应用构建器，支持链式调用</returns>
    /// <remarks>
    /// 该中间件应注册在请求管道的最前端，以确保捕获所有异常。
    /// 通常在其他中间件之前注册，如：
    /// app.UseExceptionHandling();
    /// app.UseRouting();
    /// app.UseAuthentication();
    /// </remarks>
    /// <example>
    /// 在 Program.cs 中使用：
    /// var app = builder.Build();
    /// app.UseExceptionHandling();
    /// </example>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}