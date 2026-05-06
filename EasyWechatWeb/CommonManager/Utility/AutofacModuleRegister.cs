using Autofac;
using Autofac.Core;
using CommonManager.Attributes;
using CommonManager.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using SqlSugar;
using System.Reflection;

namespace CommonManager.Utility;

/// <summary>
/// Autofac 模块注册类，用于批量注册程序集中的服务
/// </summary>
/// <remarks>
/// Autofac 模块（Module）是一种组织注册逻辑的方式，可以批量注册多个程序集中的服务。
/// 该类继承自 Autofac.Module，重写 Load 方法实现自动扫描注册。
///
/// 注册规则：
/// - 以 "Service" 结尾的类：注册为接口实现，支持属性注入
/// - 以 "Controller" 结尾的类：仅支持属性注入
/// - 以 "Filter" 结尾的类：注册为接口实现和自身
/// - 以 "Factory" 结尾的类：注册为自身
/// - 以 "Cache" 结尾的类：注册为自身
///
/// 控制注册：
/// - 使用 [AutofacInject] 特性可控制是否注册
/// - 通过 ConfigKey 读取配置动态决定是否注册
///
/// 使用 PropertyAutowired() 实现属性注入，BaseService 的 _db 属性会自动赋值。
/// </remarks>
/// <example>
/// <code>
/// // 在 Program.cs 中使用
/// var autofacModule = new AutofacModuleRegister(configuration);
/// autofacModule.AddAssembly(typeof(UserService).Assembly);
/// autofacModule.AddAssembly("BusinessManager");
///
/// builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
/// builder.Host.ConfigureContainer&lt;ContainerBuilder&gt;(cb =>
/// {
///     cb.RegisterModule(autofacModule);
/// });
///
/// // 控制特定类是否注册
/// [AutofacInject(ConfigKey = "Cap.Enabled")]
/// public class CapFailedMessageMonitorService : BackgroundService { }
/// </code>
/// </example>
public class AutofacModuleRegister : Autofac.Module
{
    /// <summary>
    /// 待注册的程序集列表
    /// </summary>
    private readonly List<Assembly> _assemblies = new();

    /// <summary>
    /// 配置对象（用于读取 AutofacInject 特性的 ConfigKey）
    /// </summary>
    private readonly IConfiguration? _configuration;

    /// <summary>
    /// 创建 Autofac 模块注册器（无配置）
    /// </summary>
    public AutofacModuleRegister()
    {
        _configuration = null;
    }

    /// <summary>
    /// 创建 Autofac 模块注册器（带配置）
    /// </summary>
    /// <param name="configuration">配置对象，用于读取 AutofacInject 特性的 ConfigKey</param>
    public AutofacModuleRegister(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// 添加程序集到注册列表
    /// </summary>
    /// <param name="assembly">要添加的程序集实例</param>
    public void AddAssembly(Assembly assembly)
    {
        _assemblies.Add(assembly);
    }

    /// <summary>
    /// 根据名称添加程序集到注册列表
    /// </summary>
    /// <param name="assemblyName">程序集名称，如 "BusinessManager"、"EasyWeChatModels"</param>
    public void AddAssembly(string assemblyName)
    {
        var assembly = Assembly.Load(assemblyName);
        _assemblies.Add(assembly);
    }

    /// <summary>
    /// 重写 Load 方法，实现批量注册逻辑
    /// </summary>
    /// <param name="builder">Autofac 容器构建器</param>
    protected override void Load(ContainerBuilder builder)
    {
        foreach (var assembly in _assemblies)
        {
            // 注册 Service（类名以 Service 结尾）
            // 同时注册为接口实现和自身类型，以便 NodeHandlerFactory 可以直接 Resolve 具体类型
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Service") && ShouldRegisterType(t))
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope()
                .OnActivated(e =>
                {
                    InjectProperties(e.Instance, e.Context);
                });

            // 注册 Controller（类名以 Controller 结尾）
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Controller") && ShouldRegisterType(t))
                .OnActivated(e =>
                {
                    InjectProperties(e.Instance, e.Context);
                });

            // 注册 Filter（类名以 Filter 结尾）
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Filter") && ShouldRegisterType(t))
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope()
                .OnActivated(e =>
                {
                    InjectProperties(e.Instance, e.Context);
                });

            // 注册 Factory（类名以 Factory 结尾）
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Factory") && ShouldRegisterType(t))
                .AsSelf()
                .InstancePerLifetimeScope()
                .OnActivated(e =>
                {
                    InjectProperties(e.Instance, e.Context);
                });

            // 注册 Cache（类名以 Cache 结尾）
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Cache") && !t.Name.StartsWith("ICache") && !t.Name.StartsWith("Memory") && !t.Name.StartsWith("Redis") && ShouldRegisterType(t))
                .AsSelf()
                .InstancePerLifetimeScope()
                .OnActivated(e =>
                {
                    InjectProperties(e.Instance, e.Context);
                });

            // 注册 Helper（类名以 Helper 结尾）
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Helper") && ShouldRegisterType(t))
                .AsSelf()
                .InstancePerLifetimeScope()
                .OnActivated(e =>
                {
                    InjectProperties(e.Instance, e.Context);
                });
        }

        // 注册 ICache 接口实现（根据配置选择 Redis 或 Memory）
        if (_configuration != null)
        {
            var useRedis = _configuration.GetValue<bool>("IsUseRedis");
            if (useRedis)
            {
                // Redis 缓存
                builder.RegisterType<RedisCacheService>()
                    .As<ICache>()
                    .SingleInstance()
                    .OnActivated(e =>
                    {
                        InjectProperties(e.Instance, e.Context);
                    });
            }
            else
            {
                // 内存缓存（需要注册 IMemoryCache）
                builder.RegisterType<MemoryCacheService>()
                    .As<ICache>()
                    .SingleInstance()
                    .OnActivated(e =>
                    {
                        InjectProperties(e.Instance, e.Context);
                    });

                // 注册 IMemoryCache（如果尚未注册）
                builder.Register(c => new MemoryCache(new MemoryCacheOptions()))
                    .As<IMemoryCache>()
                    .SingleInstance();
            }
        }
    }

    /// <summary>
    /// 检查类型是否应该被注册
    /// </summary>
    /// <param name="type">要检查的类型</param>
    /// <returns>true 表示应该注册，false 表示跳过</returns>
    /// <remarks>
    /// 检查规则：
    /// 1. 无 [AutofacInject] 特性：默认注册
    /// 2. 有 [AutofacInject] 特性：
    ///    - Enabled = false：跳过注册
    ///    - Enabled = true + 无 ConfigKey：注册
    ///    - Enabled = true + 有 ConfigKey：根据配置值决定
    /// </remarks>
    private bool ShouldRegisterType(Type type)
    {
        var attribute = type.GetCustomAttribute<AutofacInjectAttribute>();

        // 无特性时默认注册
        if (attribute == null)
        {
            return true;
        }

        // 有特性时根据特性配置决定
        return attribute.ShouldRegister(_configuration);
    }

    /// <summary>
    /// 手动注入属性
    /// </summary>
    private void InjectProperties(object instance, IComponentContext context)
    {
        var type = instance.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.Name.StartsWith("_") && p.CanWrite);

        foreach (var property in properties)
        {
            if (property.GetValue(instance) != null)
                continue;

            try
            {
                var serviceType = property.PropertyType;
                if (context.TryResolve(serviceType, out var service))
                {
                    property.SetValue(instance, service);
                }
            }
            catch
            {
                // 忽略解析失败
            }
        }
    }
}