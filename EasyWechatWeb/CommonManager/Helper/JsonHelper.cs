using Newtonsoft.Json;

namespace CommonManager.Helper;

/// <summary>
/// JSON 序列化帮助类，提供对象与 JSON 字符串之间的转换方法
/// </summary>
/// <remarks>
/// 该类基于 Newtonsoft.Json（Json.NET）实现，提供常用的序列化和反序列化方法。
/// 默认配置：忽略 null 值、日期格式 yyyy-MM-dd HH:mm:ss、忽略循环引用。
/// </remarks>
/// <example>
/// <code>
/// // 序列化对象
/// var user = new User { Id = 1, Name = "张三" };
/// var json = JsonHelper.ToJson(user);
///
/// // 反序列化对象
/// var userObj = JsonHelper.ToObject&lt;User&gt;(json);
///
/// // 反序列化列表
/// var userList = JsonHelper.ToList&lt;User&gt;(jsonArray);
/// </code>
/// </example>
public static class JsonHelper
{
    /// <summary>
    /// 默认的 JSON 序列化配置
    /// </summary>
    /// <remarks>
    /// 配置项说明：
    /// - NullValueHandling.Ignore：忽略 null 值，减少输出体积
    /// - DateFormatString：日期格式化为 yyyy-MM-dd HH:mm:ss
    /// - ReferenceLoopHandling.Ignore：忽略循环引用，防止序列化死循环
    /// </remarks>
    private static readonly JsonSerializerSettings DefaultSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        DateFormatString = "yyyy-MM-dd HH:mm:ss",
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    /// <summary>
    /// 将对象序列化为 JSON 字符串
    /// </summary>
    /// <param name="obj">要序列化的对象，可为 null</param>
    /// <returns>JSON 字符串，obj 为 null 时返回空字符串</returns>
    /// <remarks>
    /// 使用默认配置序列化：忽略 null 值、格式化日期、忽略循环引用。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 序列化单个对象
    /// var user = new User { Id = 1, Name = "张三", Email = null };
    /// var json = JsonHelper.ToJson(user);
    /// // 输出: {"Id":1,"Name":"张三"} (Email 为 null，被忽略)
    ///
    /// // 序列化列表
    /// var users = new List&lt;User&gt; { user1, user2 };
    /// var jsonArray = JsonHelper.ToJson(users);
    ///
    /// // 序列化复杂对象
    /// var order = new Order { OrderNo = "123", Items = items, CreateTime = DateTime.Now };
    /// var orderJson = JsonHelper.ToJson(order);
    /// // 日期输出格式: 2024-01-15 10:30:00
    /// </code>
    /// </example>
    public static string ToJson(object? obj)
    {
        if (obj == null) return string.Empty;
        return JsonConvert.SerializeObject(obj, DefaultSettings);
    }

    /// <summary>
    /// 将 JSON 字符串反序列化为指定类型的对象
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="json">JSON 字符串</param>
    /// <returns>反序列化后的对象，json 为空或 null 时返回默认值</returns>
    /// <remarks>
    /// 使用默认配置反序列化，支持日期格式 yyyy-MM-dd HH:mm:ss。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 反序列化单个对象
    /// var json = "{\"Id\":1,\"Name\":\"张三\"}";
    /// var user = JsonHelper.ToObject&lt;User&gt;(json);
    /// Console.WriteLine($"用户: {user.Name}");
    ///
    /// // 反序列化包含日期的对象
    /// var orderJson = "{\"CreateTime\":\"2024-01-15 10:30:00\"}";
    /// var order = JsonHelper.ToObject&lt;Order&gt;(orderJson);
    /// </code>
    /// </example>
    public static T? ToObject<T>(string json)
    {
        if (string.IsNullOrEmpty(json)) return default;
        return JsonConvert.DeserializeObject<T>(json, DefaultSettings);
    }

    /// <summary>
    /// 将 JSON 字符串反序列化为指定类型的列表
    /// </summary>
    /// <typeparam name="T">列表元素类型</typeparam>
    /// <param name="json">JSON 数组字符串</param>
    /// <returns>反序列化后的列表，json 为空时返回空列表</returns>
    /// <remarks>
    /// 适用于反序列化 JSON 数组字符串。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 反序列化用户列表
    /// var jsonArray = "[{\"Id\":1,\"Name\":\"张三\"},{\"Id\":2,\"Name\":\"李四\"}]";
    /// var users = JsonHelper.ToList&lt;User&gt;(jsonArray);
    /// foreach (var user in users)
    /// {
    ///     Console.WriteLine($"用户: {user.Name}");
    /// }
    ///
    /// // 空字符串返回空列表
    /// var emptyList = JsonHelper.ToList&lt;User&gt;("");
    /// // emptyList.Count == 0
    /// </code>
    /// </example>
    public static List<T>? ToList<T>(string json)
    {
        if (string.IsNullOrEmpty(json)) return new List<T>();
        return JsonConvert.DeserializeObject<List<T>>(json, DefaultSettings);
    }
}