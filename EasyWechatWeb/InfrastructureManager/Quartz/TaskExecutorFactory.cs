namespace InfrastructureManager.Quartz;

using EasyWeChatModels.Dto;
using Microsoft.Extensions.Logging;

/// <summary>
/// 任务执行器工厂 - 动态加载并执行处理器
/// </summary>
public class TaskExecutorFactory
{
    /// <summary>
    /// 服务提供者（属性注入）
    /// </summary>
    public IServiceProvider _serviceProvider { get; set; } = null!;

    /// <summary>
    /// 日志器（属性注入）
    /// </summary>
    public ILogger<TaskExecutorFactory> _logger { get; set; } = null!;

    /// <summary>
    /// 执行处理器
    /// </summary>
    public async Task<TaskExecutionResult> ExecuteAsync(string handlerType, string? handlerMethod = null, string? businessData = null)
    {
        try
        {
            // 清理 HandlerType（移除换行符、多余空格）
            var cleanedHandlerType = handlerType
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\t", "")
                .Trim();

            // 移除多余空格（如 "HelloWorldJob,  InfrastructureManager" -> "HelloWorldJob, InfrastructureManager"）
            cleanedHandlerType = System.Text.RegularExpressions.Regex.Replace(cleanedHandlerType, @"\s+", " ");

            _logger.LogInformation("解析处理器类型: 原始='{Original}', 清理后='{Cleaned}'", handlerType, cleanedHandlerType);

            // 解析处理器类型
            var type = Type.GetType(cleanedHandlerType);
            if (type == null)
            {
                _logger.LogError("处理器类型不存在: {HandlerType}", cleanedHandlerType);
                return TaskExecutionResult.Failed($"处理器类型不存在: {cleanedHandlerType}");
            }

            // 从 DI 容器获取处理器实例
            var handler = _serviceProvider.GetService(type);
            if (handler == null)
            {
                _logger.LogError("处理器实例不存在: {HandlerType}", cleanedHandlerType);
                return TaskExecutionResult.Failed($"处理器实例不存在: {cleanedHandlerType}");
            }

            // 获取执行方法
            var methodName = handlerMethod ?? "ExecuteAsync";
            var methodInfo = type.GetMethod(methodName);
            if (methodInfo == null)
            {
                _logger.LogError("方法不存在: {MethodName}", methodName);
                return TaskExecutionResult.Failed($"方法不存在: {methodName}");
            }

            // 执行方法（根据参数类型调用）
            var parameters = methodInfo.GetParameters();
            object? result;

            if (parameters.Length == 0)
            {
                result = methodInfo.Invoke(handler, null);
            }
            else if (parameters.Length == 1 && !string.IsNullOrEmpty(businessData))
            {
                // 尝试解析 JSON 参数
                var paramType = parameters[0].ParameterType;
                var paramValue = Newtonsoft.Json.JsonConvert.DeserializeObject(businessData, paramType);
                result = methodInfo.Invoke(handler, new[] { paramValue });
            }
            else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
            {
                result = methodInfo.Invoke(handler, new[] { businessData });
            }
            else
            {
                result = methodInfo.Invoke(handler, null);
            }

            // 处理异步结果
            if (result is Task task)
            {
                await task;
                if (task is Task<TaskExecutionResult> resultTask)
                {
                    return await resultTask;
                }
            }
            else if (result is TaskExecutionResult executionResult)
            {
                return executionResult;
            }

            return TaskExecutionResult.Success("执行完成");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行处理器失败: {HandlerType}", handlerType);
            return TaskExecutionResult.Failed(ex.Message);
        }
    }
}