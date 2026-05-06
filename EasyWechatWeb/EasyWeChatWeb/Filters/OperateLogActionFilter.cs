using BusinessManager.Buz.IService;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace EasyWeChatWeb.Filters;

/// <summary>
/// 操作日志自动记录过滤器（使用 Autofac 属性注入）
/// </summary>
public class OperateLogActionFilter : IAsyncActionFilter
{
    /// <summary>
    /// 操作日志服务接口（Autofac 属性注入）
    /// </summary>
    public IOperateLogService _operateLogService { get; set; } = null!;

    /// <summary>
    /// 日志记录器（Autofac 属性注入）
    /// </summary>
    public ILogger<OperateLogActionFilter> _logger { get; set; } = null!;

    /// <summary>
    /// 配置（Autofac 属性注入）
    /// </summary>
    public IConfiguration _configuration { get; set; } = null!;

    /// <summary>
    /// 默认排除路径（不记录日志的接口）
    /// </summary>
    private static readonly string[] DefaultExcludePaths = new[]
    {
        "/api/auth/login",
        "/api/auth/refresh",
        "/api/auth/logout",
        "/api/operatelog",
        "/swagger",
        "/health"
    };

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        var path = request.Path.Value ?? "";

        // 检查是否排除路径
        if (ShouldExclude(path, request.Method))
        {
            await next();
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var userId = GetUserId(context);
        var userName = GetUserName(context);
        var module = GetModuleName(context);
        var action = GetActionName(context);
        var ip = GetClientIp(context);
        var @params = GetRequestParams(context);

        AddOperateLogDto? log = null;

        try
        {
            // 执行控制器方法
            var result = await next();
            stopwatch.Stop();

            // 记录成功日志
            log = new AddOperateLogDto
            {
                UserId = userId,
                UserName = userName,
                Module = module,
                Action = action,
                Method = request.Method,
                Url = path,
                Ip = ip,
                Params = @params,
                Result = GetResultJson(result),
                Status = 1,
                Duration = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // 记录失败日志
            log = new AddOperateLogDto
            {
                UserId = userId,
                UserName = userName,
                Module = module,
                Action = action,
                Method = request.Method,
                Url = path,
                Ip = ip,
                Params = @params,
                Status = 0,
                ErrorMsg = ex.Message,
                Duration = stopwatch.ElapsedMilliseconds
            };

            throw;
        }
        finally
        {
            // 异步保存日志（不阻塞请求）
            if (log != null)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _operateLogService.AddAsync(log);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "保存操作日志失败");
                    }
                });
            }
        }
    }

    private bool ShouldExclude(string path, string method)
    {
        var enabled = _configuration.GetValue<bool>("OperateLog:Enabled");
        if (!enabled) return true;

        foreach (var excludePath in DefaultExcludePaths)
        {
            if (path.StartsWith(excludePath, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        var configExcludePaths = _configuration.GetSection("OperateLog:ExcludePaths").Get<string[]>();
        if (configExcludePaths != null)
        {
            foreach (var excludePath in configExcludePaths)
            {
                if (path.StartsWith(excludePath, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }

        var excludeGet = _configuration.GetValue<bool>("OperateLog:ExcludeGet");
        if (excludeGet && method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    private Guid? GetUserId(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ??
                          user.FindFirst("sub") ??
                          user.FindFirst("userId") ??
                          user.FindFirst("id");
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            return userId;
        return null;
    }

    private string? GetUserName(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;
        return user.FindFirst(ClaimTypes.Name)?.Value ??
               user.FindFirst("userName")?.Value ??
               user.FindFirst("name")?.Value ??
               user.Identity?.Name;
    }

    private string GetModuleName(ActionExecutingContext context) =>
        context.ActionDescriptor.RouteValues["controller"] ?? "Unknown";

    private string GetActionName(ActionExecutingContext context) =>
        context.ActionDescriptor.RouteValues["action"] ?? "Unknown";

    private string GetClientIp(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;
        var forwardedFor = request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
            return forwardedFor.Split(',').First().Trim();

        var realIp = request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
            return realIp;

        return context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private string? GetRequestParams(ActionExecutingContext context)
    {
        try
        {
            var parameters = new Dictionary<string, object?>();
            foreach (var kvp in context.ActionArguments)
            {
                if (kvp.Key.ToLower().Contains("password") ||
                    kvp.Key.ToLower().Contains("secret") ||
                    kvp.Key.ToLower().Contains("token"))
                {
                    parameters[kvp.Key] = "******";
                }
                else
                {
                    parameters[kvp.Key] = kvp.Value;
                }
            }

            var json = JsonSerializer.Serialize(parameters, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return json.Length > 4000 ? json.Substring(0, 4000) + "..." : json;
        }
        catch { return null; }
    }

    private string? GetResultJson(ActionExecutedContext result)
    {
        try
        {
            if (result.Result is Microsoft.AspNetCore.Mvc.ObjectResult objectResult)
            {
                var json = JsonSerializer.Serialize(objectResult.Value, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                return json.Length > 2000 ? json.Substring(0, 2000) + "..." : json;
            }
            return null;
        }
        catch { return null; }
    }
}