namespace BusinessManager.Tasks;

using BusinessManager.Infrastructure.IService;
using CommonManager.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

/// <summary>
/// CAP 失败消息监控服务 - 定期检查失败消息并执行补偿
/// </summary>
/// <remarks>
/// BackgroundService 必须使用构造函数注入，因为 AddHostedService 不支持 Autofac 属性注入。
///
/// 使用 [AutofacInject(ConfigKey = "Cap.Enabled")] 特性：
/// - 当 Cap.Enabled = false 时，Autofac 不会自动注册此服务
/// - 当 Cap.Enabled = true 时，Autofac 会自动注册此服务
/// </remarks>
[AutofacInject(ConfigKey = "Cap.Enabled")]
public class CapFailedMessageMonitorService : BackgroundService
{
    private readonly ITaskCompensationService _compensationService;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// 检查间隔（秒）- 默认 5 分钟
    /// </summary>
    private const int CheckIntervalSeconds = 300;

    public CapFailedMessageMonitorService(
        ITaskCompensationService compensationService,
        IConfiguration configuration)
    {
        _compensationService = compensationService;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 检查 CAP 是否启用，未启用则直接退出
        var capEnabled = _configuration.GetValue<bool?>("Cap:Enabled") ?? false;

        if (!capEnabled)
        {
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _compensationService.CheckAndProcessFailedMessagesAsync();
            }
            catch
            {
                // 忽略异常，下次轮询继续执行
            }

            await Task.Delay(CheckIntervalSeconds * 1000, stoppingToken);
        }
    }
}