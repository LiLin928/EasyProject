using EasyWeChatModels.Dto;
using Microsoft.Extensions.Logging;
using global::Quartz;

namespace InfrastructureManager.Jobs;

/// <summary>
/// HelloWorld 测试任务
/// </summary>
/// <remarks>
/// 每分钟输出一次 HelloWorld，用于测试定时任务功能
/// Quartz Job 必须使用构造函数注入，因为 Quartz DI JobFactory 不支持 Autofac 属性注入
/// </remarks>
public class HelloWorldJob : IJob
{
    private readonly ILogger<HelloWorldJob> _logger;

    /// <summary>
    /// 构造函数注入（Quartz JobFactory 要求）
    /// </summary>
    public HelloWorldJob(ILogger<HelloWorldJob> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 执行任务
    /// </summary>
    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("HelloWorld Job 执行开始 - {Time}", DateTime.Now);

        // 输出 HelloWorld
        _logger.LogInformation("HelloWorld! 当前时间: {Time}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        // 记录任务完成
        _logger.LogInformation("HelloWorld Job 执行完成 - {Time}", DateTime.Now);

        await Task.CompletedTask;
    }

    /// <summary>
    /// 异步执行方法（供反射调用）
    /// </summary>
    public async Task<TaskExecutionResult> ExecuteAsync()
    {
        _logger.LogInformation("HelloWorld 反射执行 - {Time}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        return TaskExecutionResult.Success("HelloWorld 输出成功");
    }
}