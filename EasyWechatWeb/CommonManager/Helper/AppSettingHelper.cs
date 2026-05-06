using Microsoft.Extensions.Configuration;

namespace CommonManager.Helper;

/// <summary>
/// 配置读取帮助类，用于从 IConfiguration 中获取配置值
/// </summary>
/// <remarks>
/// 该类封装了常用的配置读取操作，支持获取单个配置值、绑定配置节和获取连接字符串。
/// 通过依赖注入 IConfiguration 实现配置读取。
/// </remarks>
/// <example>
/// <code>
/// // 注册服务（通常由框架自动注册）
/// builder.Services.AddScoped&lt;AppSettingHelper&gt;();
///
/// // 使用配置帮助类
/// public class UserService
/// {
///     private readonly AppSettingHelper _config;
///     public UserService(AppSettingHelper config) { _config = config; }
///
///     public string GetDefaultTheme()
///     {
///         return _config.GetValue("Theme:Default", "light");
///     }
/// }
/// </code>
/// </example>
public class AppSettingHelper
{
    /// <summary>
    /// 配置实例
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// 构造函数，注入 IConfiguration 实例
    /// </summary>
    /// <param name="configuration">IConfiguration 实例，由依赖注入提供</param>
    /// <remarks>
    /// IConfiguration 通常在 Program.cs 中通过 builder.Configuration 创建，
    /// ASP.NET Core 会自动将其注册到依赖注入容器。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 通过依赖注入创建
    /// builder.Services.AddScoped&lt;AppSettingHelper&gt;();
    /// </code>
    /// </example>
    public AppSettingHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// 获取指定键的配置值
    /// </summary>
    /// <param name="key">配置键名，支持分层结构（如 "App:Name"）</param>
    /// <returns>配置值，不存在时返回空字符串</returns>
    /// <remarks>
    /// 配置键名使用冒号分隔层级，对应 appsettings.json 的结构。
    /// 例如："JWT:SecurityKey" 对应 { "JWT": { "SecurityKey": "xxx" } }
    /// </remarks>
    /// <example>
    /// <code>
    /// // appsettings.json:
    /// // { "App": { "Name": "MyApp", "Version": "1.0" } }
    ///
    /// var appName = _config.GetValue("App:Name");  // 返回 "MyApp"
    /// var version = _config.GetValue("App:Version");  // 返回 "1.0"
    /// var missing = _config.GetValue("App:Missing");  // 返回 ""
    /// </code>
    /// </example>
    public string GetValue(string key)
    {
        return _configuration[key] ?? string.Empty;
    }

    /// <summary>
    /// 获取指定键的配置值，支持类型转换和默认值
    /// </summary>
    /// <typeparam name="T">目标类型（如 int、bool、double 等）</typeparam>
    /// <param name="key">配置键名</param>
    /// <param name="defaultValue">配置不存在或转换失败时的默认值</param>
    /// <returns>转换后的配置值，不存在时返回默认值</returns>
    /// <remarks>
    /// 支持基础类型的自动转换，如 int、bool、double、DateTime 等。
    /// 配置不存在或转换失败时返回默认值，不抛出异常。
    /// </remarks>
    /// <example>
    /// <code>
    /// // appsettings.json:
    /// // { "Cache": { "ExpireSeconds": 300, "Enabled": true } }
    ///
    /// var expireSeconds = _config.GetValue&lt;int&gt;("Cache:ExpireSeconds", 60);  // 返回 300
    /// var enabled = _config.GetValue&lt;bool&gt;("Cache:Enabled", false);  // 返回 true
    /// var missingInt = _config.GetValue&lt;int&gt;("Cache:Missing", 100);  // 返回 100（默认值）
    /// </code>
    /// </example>
    public T GetValue<T>(string key, T defaultValue)
    {
        var value = _configuration[key];
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }
        return (T)Convert.ChangeType(value, typeof(T));
    }

    /// <summary>
    /// 获取配置节并绑定到指定类型的对象
    /// </summary>
    /// <typeparam name="T">目标类型，需有无参构造函数</typeparam>
    /// <param name="key">配置节键名</param>
    /// <returns>绑定后的配置对象，配置节不存在时返回新实例</returns>
    /// <remarks>
    /// 适用于获取复杂的配置结构，如数据库连接选项、JWT 配置等。
    /// 配置节的属性名需与目标类型的属性名匹配。
    /// </remarks>
    /// <example>
    /// <code>
    /// // appsettings.json:
    /// // {
    /// //   "JWTTokenOptions": {
    /// //     "Issuer": "http://localhost",
    /// //     "Audience": "http://localhost",
    /// //     "SecurityKey": "your-secret-key",
    /// //     "AccessTokenExpiration": 60
    /// //   }
    /// // }
    ///
    /// var jwtOptions = _config.GetSection&lt;JWTTokenOptions&gt;("JWTTokenOptions");
    /// Console.WriteLine($"Issuer: {jwtOptions.Issuer}");
    /// </code>
    /// </example>
    public T GetSection<T>(string key) where T : class, new()
    {
        var section = _configuration.GetSection(key);
        if (!section.Exists())
        {
            return new T();
        }
        return section.Get<T>() ?? new T();
    }

    /// <summary>
    /// 获取指定名称的数据库连接字符串
    /// </summary>
    /// <param name="name">连接字符串名称（对应 ConnectionStrings 配置节中的键名）</param>
    /// <returns>连接字符串，不存在时返回空字符串</returns>
    /// <remarks>
    /// 连接字符串通常在 appsettings.json 的 ConnectionStrings 节中配置。
    /// </remarks>
    /// <example>
    /// <code>
    /// // appsettings.json:
    /// // {
    /// //   "ConnectionStrings": {
    /// //     "DefaultConnection": "Server=localhost;Database=MyDb;...",
    /// //     "Redis": "localhost:6379,password=xxx"
    /// //   }
    /// // }
    ///
    /// var dbConn = _config.GetConnectionString("DefaultConnection");
    /// var redisConn = _config.GetConnectionString("Redis");
    /// </code>
    /// </example>
    public string GetConnectionString(string name)
    {
        return _configuration.GetConnectionString(name) ?? string.Empty;
    }
}