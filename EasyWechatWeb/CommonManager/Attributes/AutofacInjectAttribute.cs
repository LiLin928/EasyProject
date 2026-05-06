namespace CommonManager.Attributes;

using Microsoft.Extensions.Configuration;

/// <summary>
/// Autofac 注入控制特性 - 用于控制类是否被 Autofac 自动扫描注册
/// </summary>
/// <remarks>
/// 使用场景：
/// 1. 默认情况：所有以 Service/Controller/Filter/Cache/Factory 结尾的类都会被自动注册
/// 2. 使用此特性可以控制特定类是否被注册
/// 3. 通过配置键动态控制注册行为（如根据 Cap.Enabled 决定是否注册相关服务）
///
/// 配置读取规则：
/// - 未指定 ConfigKey：默认 Enabled = true
/// - 指定 ConfigKey：读取对应配置值，支持 bool 类型（true/false）
/// - 配置不存在：默认 Enabled = true
///
/// 示例：
/// [AutofacInject] // 默认启用
/// public class UserService : IUserService { }
///
/// [AutofacInject(ConfigKey = "Cap.Enabled")] // 根据 Cap.Enabled 配置决定
/// public class CapFailedMessageMonitorService : BackgroundService { }
///
/// [AutofacInject(Enabled = false)] // 强制禁用
/// public class TempService : ITempService { }
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class AutofacInjectAttribute : Attribute
{
    /// <summary>
    /// 是否启用注入（硬编码，优先级最高）
    /// </summary>
    /// <remarks>
    /// 设为 false 时强制禁用，不读取配置
    /// </remarks>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 配置键 - 从 appsettings.json 读取配置决定是否启用
    /// </summary>
    /// <remarks>
    /// 支持的配置键示例：
    /// - "Cap.Enabled" → 读取 Cap.Enabled 值
    /// - "Quartz.Enabled" → 读取 Quartz.Enabled 值
    /// - "Redis.Enabled" → 读取 Redis.Enabled 值
    ///
    /// 配置值必须是 bool 类型（true/false）
    /// </remarks>
    public string? ConfigKey { get; set; }

    /// <summary>
    /// 检查是否应该注册此类
    /// </summary>
    /// <param name="configuration">配置对象（可为 null，此时使用默认值）</param>
    /// <returns>true 表示应该注册，false 表示跳过</returns>
    public bool ShouldRegister(IConfiguration? configuration = null)
    {
        // 如果硬编码禁用，直接返回 false
        if (!Enabled)
        {
            return false;
        }

        // 如果没有配置键，使用默认值 true
        if (string.IsNullOrEmpty(ConfigKey))
        {
            return true;
        }

        // 读取配置值
        if (configuration == null)
        {
            return true; // 无配置对象时默认启用
        }

        var configValue = configuration.GetValue<bool?>(ConfigKey);

        // 配置不存在时默认启用，配置存在时使用配置值
        return configValue ?? true;
    }
}