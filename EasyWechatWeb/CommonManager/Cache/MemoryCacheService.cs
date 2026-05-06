using Microsoft.Extensions.Caching.Memory;

namespace CommonManager.Cache;

/// <summary>
/// 内存缓存实现，基于 IMemoryCache
/// </summary>
/// <remarks>
/// 使用 ASP.NET Core 内置的内存缓存组件实现 ICache 接口。
/// 适用于单机应用，不支持分布式场景。
/// 优点：速度快、无需外部依赖。
/// 缺点：应用重启后缓存丢失、不支持跨进程共享。
///
/// 使用 Autofac 属性注入模式，_cache 属性会自动赋值。
/// </remarks>
/// <example>
/// <code>
/// // 注册服务（由 Autofac 自动注册）
/// builder.Services.AddMemoryCache();
///
/// // 使用缓存（属性注入）
/// public class UserService
/// {
///     public ICache _cache { get; set; } = null!;
/// }
/// </code>
/// </example>
public class MemoryCacheService : ICache
{
    /// <summary>
    /// 内存缓存实例（Autofac 属性注入）
    /// </summary>
    public IMemoryCache _cache { get; set; } = null!;

    /// <summary>
    /// 设置缓存值
    /// </summary>
    /// <param name="key">缓存键名</param>
    /// <param name="value">缓存值（字符串格式）</param>
    /// <param name="expireSeconds">过期时间（秒），-1 表示永不过期</param>
    /// <returns>异步任务（内存缓存为同步操作，返回已完成任务）</returns>
    /// <remarks>
    /// 内存缓存使用滑动过期策略，每次访问都会重置过期时间。
    /// expireSeconds > 0 时设置过期时间，否则永久缓存。
    /// </remarks>
    public Task SetAsync(string key, string value, int expireSeconds = -1)
    {
        if (expireSeconds > 0)
            _cache.Set(key, value, TimeSpan.FromSeconds(expireSeconds));
        else
            _cache.Set(key, value);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取缓存值
    /// </summary>
    /// <param name="key">缓存键名</param>
    /// <returns>缓存值，不存在时返回 null</returns>
    public Task<string?> GetAsync(string key)
    {
        var value = _cache.Get<string>(key);
        return Task.FromResult(value);
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="key">缓存键名</param>
    /// <returns>异步任务（同步操作）</returns>
    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 判断缓存键是否存在
    /// </summary>
    /// <param name="key">缓存键名</param>
    /// <returns>存在返回 true，不存在返回 false</returns>
    public Task<bool> ExistsAsync(string key)
    {
        var exists = _cache.TryGetValue(key, out _);
        return Task.FromResult(exists);
    }

    /// <summary>
    /// 设置哈希表字段值
    /// </summary>
    /// <param name="key">哈希表键名</param>
    /// <param name="field">字段名</param>
    /// <param name="value">字段值</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// 内存缓存不支持真正的哈希表结构，使用 key:field 格式模拟。
    /// </remarks>
    public Task SetHashAsync(string key, string field, string value)
    {
        var hashKey = $"{key}:{field}";
        _cache.Set(hashKey, value);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取哈希表字段值
    /// </summary>
    /// <param name="key">哈希表键名</param>
    /// <param name="field">字段名</param>
    /// <returns>字段值，不存在时返回 null</returns>
    public Task<string?> GetHashAsync(string key, string field)
    {
        var hashKey = $"{key}:{field}";
        var value = _cache.Get<string>(hashKey);
        return Task.FromResult(value);
    }

    /// <summary>
    /// 数值递增操作
    /// </summary>
    /// <param name="key">缓存键名</param>
    /// <param name="value">递增值，默认为 1</param>
    /// <returns>递增后的新值</returns>
    /// <remarks>
    /// 内存缓存不支持原子操作，先获取后设置的并发场景可能出现数据不一致。
    /// 如需原子操作请使用 Redis 缓存实现。
    /// </remarks>
    public Task<long> IncrementAsync(string key, long value = 1)
    {
        var currentValue = _cache.Get<long>(key);
        var newValue = currentValue + value;
        _cache.Set(key, newValue);
        return Task.FromResult(newValue);
    }
}