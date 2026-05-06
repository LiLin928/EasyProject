namespace CommonManager.Cache;

/// <summary>
/// 缓存接口，定义统一的缓存操作方法
/// </summary>
/// <remarks>
/// 该接口提供统一的缓存操作规范，支持内存缓存和 Redis 缓存实现。
/// 通过依赖注入可以灵活切换缓存实现方式。
/// </remarks>
/// <example>
/// <code>
/// // 在服务中使用缓存
/// public class UserService
/// {
///     private readonly ICache _cache;
///
///     public UserService(ICache cache)
///     {
///         _cache = cache;
///     }
///
///     public async Task&lt;User?&gt; GetUserFromCache(int userId)
///     {
///         var cached = await _cache.GetAsync($"user:{userId}");
///         if (cached != null)
///             return JsonHelper.ToObject&lt;User&gt;(cached);
///         return null;
///     }
/// }
/// </code>
/// </example>
public interface ICache
{
    /// <summary>
    /// 设置缓存值
    /// </summary>
    /// <param name="key">缓存键名</param>
    /// <param name="value">缓存值（字符串格式）</param>
    /// <param name="expireSeconds">过期时间（秒），-1 表示永不过期</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// 如果键已存在，将覆盖原有值。
    /// expireSeconds 为 -1 或不提供时，缓存不会自动过期。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 设置永久缓存
    /// await _cache.SetAsync("config:theme", "dark");
    ///
    /// // 设置 5 分钟过期的缓存
    /// await _cache.SetAsync("user:session:123", sessionData, 300);
    /// </code>
    /// </example>
    Task SetAsync(string key, string value, int expireSeconds = -1);

    /// <summary>
    /// 获取缓存值
    /// </summary>
    /// <param name="key">缓存键名</param>
    /// <returns>缓存值，不存在或已过期时返回 null</returns>
    /// <remarks>
    /// 返回原始字符串值，如需转换为对象请配合 JsonHelper 使用。
    /// </remarks>
    /// <example>
    /// <code>
    /// var cached = await _cache.GetAsync("user:1");
    /// if (cached != null)
    /// {
    ///     var user = JsonHelper.ToObject&lt;User&gt;(cached);
    /// }
    /// </code>
    /// </example>
    Task<string?> GetAsync(string key);

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="key">缓存键名</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// 删除指定键的缓存，键不存在时操作也会成功执行。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 用户信息更新后清除缓存
    /// await _userService.UpdateAsync(user);
    /// await _cache.RemoveAsync($"user:{user.Id}");
    /// </code>
    /// </example>
    Task RemoveAsync(string key);

    /// <summary>
    /// 判断缓存键是否存在
    /// </summary>
    /// <param name="key">缓存键名</param>
    /// <returns>存在返回 true，不存在返回 false</returns>
    /// <remarks>
    /// 可用于在设置缓存前检查是否已有缓存，避免重复查询数据库。
    /// </remarks>
    /// <example>
    /// <code>
    /// if (await _cache.ExistsAsync("user:1"))
    /// {
    ///     // 缓存存在，直接使用
    /// }
    /// else
    /// {
    ///     // 缓存不存在，查询数据库并设置缓存
    /// }
    /// </code>
    /// </example>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// 设置哈希表字段值
    /// </summary>
    /// <param name="key">哈希表键名</param>
    /// <param name="field">字段名</param>
    /// <param name="value">字段值</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// 哈希表适用于存储结构化数据，如用户的多属性信息。
    /// 相比普通键值对，哈希表可以更灵活地管理复杂数据。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 存储用户的不同属性
    /// await _cache.SetHashAsync("user:1", "name", "张三");
    /// await _cache.SetHashAsync("user:1", "email", "zhangsan@example.com");
    /// await _cache.SetHashAsync("user:1", "status", "active");
    /// </code>
    /// </example>
    Task SetHashAsync(string key, string field, string value);

    /// <summary>
    /// 获取哈希表字段值
    /// </summary>
    /// <param name="key">哈希表键名</param>
    /// <param name="field">字段名</param>
    /// <returns>字段值，不存在时返回 null</returns>
    /// <remarks>
    /// 从哈希表中获取指定字段的值。
    /// </remarks>
    /// <example>
    /// <code>
    /// var userName = await _cache.GetHashAsync("user:1", "name");
    /// Console.WriteLine($"用户名: {userName}");
    /// </code>
    /// </example>
    Task<string?> GetHashAsync(string key, string field);

    /// <summary>
    /// 数值递增操作
    /// </summary>
    /// <param name="key">缓存键名</param>
    /// <param name="value">递增值，默认为 1</param>
    /// <returns>递增后的新值</returns>
    /// <remarks>
    /// 常用于计数器场景，如访问计数、点赞数、库存等。
    /// 如果键不存在，将初始化为 0 后再执行递增。
    /// 支持传入负值实现递减操作。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 访问计数
    /// var viewCount = await _cache.IncrementAsync("page:home:views");
    ///
    /// // 库存扣减
    /// var newStock = await _cache.IncrementAsync("product:123:stock", -1);
    ///
    /// // 批量增加
    /// var totalLikes = await _cache.IncrementAsync("post:456:likes", 10);
    /// </code>
    /// </example>
    Task<long> IncrementAsync(string key, long value = 1);
}