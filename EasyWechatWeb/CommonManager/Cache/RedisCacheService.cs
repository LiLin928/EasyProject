using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

namespace CommonManager.Cache;

/// <summary>
/// Redis 缓存实现，基于 StackExchange.Redis
/// </summary>
/// <remarks>
/// 使用 StackExchange.Redis 客户端实现 ICache 接口。
/// 适用于分布式场景，支持多应用共享缓存。
/// 优点：支持分布式、支持持久化、支持原子操作、数据类型丰富。
/// 缺点：需要 Redis 服务、网络延迟。
///
/// 使用 Autofac 属性注入模式，_configuration 属性会自动赋值。
/// Redis 连接采用延迟初始化，在首次使用时从配置读取 RedisConnection。
/// </remarks>
/// <example>
/// <code>
/// // 配置示例（appsettings.json）：
/// // {
/// //   "IsUseRedis": true,
/// //   "RedisConnection": "127.0.0.1:6379,password=your_redis_pwd"
/// // }
///
/// // 使用缓存（属性注入）
/// public class UserService
/// {
///     public ICache _cache { get; set; } = null!;
/// }
/// </code>
/// </example>
public class RedisCacheService : ICache
{
    /// <summary>
    /// 配置实例（Autofac 属性注入）
    /// </summary>
    public IConfiguration _configuration { get; set; } = null!;

    /// <summary>
    /// Redis 数据库实例（延迟初始化）
    /// </summary>
    private IDatabase? _database;

    /// <summary>
    /// Redis 连接实例（延迟初始化）
    /// </summary>
    private ConnectionMultiplexer? _connection;

    /// <summary>
    /// 获取 Redis 数据库实例（延迟初始化）
    /// </summary>
    private IDatabase GetDatabase()
    {
        if (_database == null)
        {
            var connectionString = _configuration["RedisConnection"] ?? "127.0.0.1:6379";
            _connection = ConnectionMultiplexer.Connect(connectionString);
            _database = _connection.GetDatabase();
        }
        return _database;
    }

    /// <summary>
    /// 设置缓存值
    /// </summary>
    /// <param name="key">缓存键名</param>
    /// <param name="value">缓存值（字符串格式）</param>
    /// <param name="expireSeconds">过期时间（秒），-1 表示永不过期</param>
    /// <returns>异步任务</returns>
    public async Task SetAsync(string key, string value, int expireSeconds = -1)
    {
        var db = GetDatabase();
        if (expireSeconds > 0)
            await db.StringSetAsync(key, value, TimeSpan.FromSeconds(expireSeconds));
        else
            await db.StringSetAsync(key, value);
    }

    /// <summary>
    /// 获取缓存值
    /// </summary>
    /// <param name="key">缓存键名</param>
    /// <returns>缓存值，不存在或已过期时返回 null</returns>
    public async Task<string?> GetAsync(string key)
    {
        var db = GetDatabase();
        var value = await db.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="key">缓存键名</param>
    /// <returns>异步任务</returns>
    public async Task RemoveAsync(string key)
    {
        var db = GetDatabase();
        await db.KeyDeleteAsync(key);
    }

    /// <summary>
    /// 判断缓存键是否存在
    /// </summary>
    /// <param name="key">缓存键名</param>
    /// <returns>存在返回 true，不存在返回 false</returns>
    public async Task<bool> ExistsAsync(string key)
    {
        var db = GetDatabase();
        return await db.KeyExistsAsync(key);
    }

    /// <summary>
    /// 设置哈希表字段值
    /// </summary>
    /// <param name="key">哈希表键名</param>
    /// <param name="field">字段名</param>
    /// <param name="value">字段值</param>
    /// <returns>异步任务</returns>
    public async Task SetHashAsync(string key, string field, string value)
    {
        var db = GetDatabase();
        await db.HashSetAsync(key, field, value);
    }

    /// <summary>
    /// 获取哈希表字段值
    /// </summary>
    /// <param name="key">哈希表键名</param>
    /// <param name="field">字段名</param>
    /// <returns>字段值，不存在时返回 null</returns>
    public async Task<string?> GetHashAsync(string key, string field)
    {
        var db = GetDatabase();
        var value = await db.HashGetAsync(key, field);
        return value.HasValue ? value.ToString() : null;
    }

    /// <summary>
    /// 数值递增操作（原子操作）
    /// </summary>
    /// <param name="key">缓存键名</param>
    /// <param name="value">递增值，默认为 1</param>
    /// <returns>递增后的新值</returns>
    /// <remarks>
    /// Redis 的 Increment 操作是原子性的，适用于高并发计数场景。
    /// 键不存在时，Redis 会自动初始化为 0 再执行递增。
    /// </remarks>
    public async Task<long> IncrementAsync(string key, long value = 1)
    {
        var db = GetDatabase();
        return await db.StringIncrementAsync(key, value);
    }
}