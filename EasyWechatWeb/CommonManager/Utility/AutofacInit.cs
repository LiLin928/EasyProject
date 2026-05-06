using Autofac;

namespace CommonManager.Utility;

/// <summary>
/// Autofac 初始化帮助类，提供依赖注入容器的管理和服务解析方法
/// </summary>
/// <remarks>
/// Autofac 是一个功能强大的 IoC 容器，支持属性注入、命名服务等高级功能。
/// 该类提供静态方法，用于在无法通过构造函数注入的场景中获取服务实例。
///
/// 使用场景：
/// - 在静态方法或静态类中获取服务
/// - 在 BaseService 中获取数据库上下文（属性注入）
/// - 在无法使用依赖注入的特殊场景中解析服务
///
/// 注意：优先使用构造函数注入，仅在特殊场景使用此帮助类。
/// </remarks>
/// <example>
/// <code>
/// // 在应用启动时设置容器
/// var container = builder.Build();
/// AutofacInit.SetContainer(container);
///
/// // 在静态方法中获取服务
/// var userService = AutofacInit.GetServiceFromFac&lt;IUserService&gt;();
///
/// // 在 BaseService 中使用属性注入
/// public class UserService : BaseService&lt;User&gt;
/// {
///     // _db 通过 Autofac 属性注入自动赋值
///     public ISqlSugarClient _db { get; set; }
/// }
/// </code>
/// </example>
public static class AutofacInit
{
    /// <summary>
    /// Autofac 容器实例
    /// </summary>
    private static IContainer? _container;

    /// <summary>
    /// 设置 Autofac 容器实例
    /// </summary>
    /// <param name="container">Autofac 容器实例，在应用启动时创建</param>
    /// <remarks>
    /// 在应用启动时（如 Program.cs）设置容器，以便后续解析服务。
    /// 设置后容器在应用生命周期内保持不变。
    /// </remarks>
    /// <example>
    /// <code>
    /// // Program.cs 中设置容器
    /// var builder = new ContainerBuilder();
    /// builder.RegisterModule(new AutofacModuleRegister());
    /// var container = builder.Build();
    /// AutofacInit.SetContainer(container);
    /// </code>
    /// </example>
    public static void SetContainer(IContainer container)
    {
        _container = container;
    }

    /// <summary>
    /// 从容器中获取指定类型的服务实例
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns>服务实例，容器未初始化或服务不存在时返回 null</returns>
    /// <remarks>
    /// 使用泛型方法，返回强类型的服务实例。
    /// 容器未初始化时返回 null，不抛出异常。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 获取用户服务
    /// var userService = AutofacInit.GetServiceFromFac&lt;IUserService&gt;();
    /// if (userService != null)
    /// {
    ///     var user = await userService.GetByIdAsync(1);
    /// }
    ///
    /// // 获取缓存服务
    /// var cache = AutofacInit.GetServiceFromFac&lt;ICache&gt;();
    /// await cache.SetAsync("key", "value");
    /// </code>
    /// </example>
    public static T? GetServiceFromFac<T>() where T : class
    {
        if (_container == null) return null;
        return _container.Resolve<T>();
    }

    /// <summary>
    /// 从容器中获取指定类型的服务实例（非泛型版本）
    /// </summary>
    /// <param name="serviceType">服务类型</param>
    /// <returns>服务实例（object 类型），容器未初始化时返回 null</returns>
    /// <remarks>
    /// 适用于类型在运行时确定的场景，如反射创建服务。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 根据类型名称动态获取服务
    /// var serviceType = Type.GetType("Namespace.IUserService");
    /// var service = AutofacInit.GetServiceFromFac(serviceType);
    /// </code>
    /// </example>
    public static object? GetServiceFromFac(Type serviceType)
    {
        if (_container == null) return null;
        return _container.Resolve(serviceType);
    }

    /// <summary>
    /// 解析指定类型的服务实例（GetServiceFromFac 的快捷方法）
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns>服务实例，容器未初始化时返回 null</returns>
    /// <remarks>
    /// Resolve 是 GetServiceFromFac&lt;T&gt; 的别名，提供更简洁的调用方式。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 使用 Resolve 方法获取服务
    /// var userService = AutofacInit.Resolve&lt;IUserService&gt;();
    /// </code>
    /// </example>
    public static T? Resolve<T>() where T : class
    {
        return GetServiceFromFac<T>();
    }

    /// <summary>
    /// 解析具有名称的服务实例（命名服务）
    /// </summary>
    /// <param name="serviceType">服务类型</param>
    /// <param name="serviceName">服务名称，用于区分同一类型的多个实现</param>
    /// <returns>服务实例，容器未初始化或命名服务不存在时返回 null</returns>
    /// <remarks>
    /// 命名服务用于同一接口有多个实现时，通过名称区分：
    /// - 如 ICache 有 MemoryCacheService 和 RedisCacheService 两种实现
    /// - 通过 serviceName 指定要使用哪种实现
    ///
    /// 注册命名服务：
    /// builder.RegisterType&lt;MemoryCacheService&gt;().Named&lt;ICache&gt;("Memory");
    /// builder.RegisterType&lt;RedisCacheService&gt;().Named&lt;ICache&gt;("Redis");
    /// </remarks>
    /// <example>
    /// <code>
    /// // 获取命名服务
    /// var memoryCache = AutofacInit.Resolve(typeof(ICache), "Memory");
    /// var redisCache = AutofacInit.Resolve(typeof(ICache), "Redis");
    ///
    /// // 使用场景：根据配置选择缓存实现
    /// var cacheType = _config.GetValue("Cache:Type", "Memory");
    /// var cache = AutofacInit.Resolve(typeof(ICache), cacheType);
    /// </code>
    /// </example>
    public static object? Resolve(Type serviceType, string serviceName)
    {
        if (_container == null) return null;
        return _container.ResolveNamed(serviceName, serviceType);
    }
}