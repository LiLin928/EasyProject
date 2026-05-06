namespace CommonManager.Base;

/// <summary>
/// 统一 API 响应格式，用于封装所有 API 接口的返回结果
/// </summary>
/// <typeparam name="T">响应数据的类型</typeparam>
/// <remarks>
/// 该类提供统一的 API 响应格式，包含状态码、消息、数据和时间戳。
/// 所有 API 接口应使用此类封装返回结果，保证响应格式的一致性。
/// </remarks>
/// <example>
/// <code>
/// // 返回成功响应
/// var response = ApiResponse&lt;User&gt;.Success(user, "获取用户成功");
///
/// // 返回失败响应
/// var errorResponse = ApiResponse&lt;User&gt;.Error("用户不存在", 404);
/// </code>
/// </example>
public class ApiResponse<T>
{
    /// <summary>
    /// 状态码：200 表示成功，其他值表示失败
    /// </summary>
    /// <remarks>
    /// 常见状态码：
    /// - 200: 操作成功
    /// - 400: 参数错误
    /// - 401: 未授权
    /// - 403: 禁止访问
    /// - 404: 资源不存在
    /// - 500: 服务器内部错误
    /// </remarks>
    /// <example>
    /// <code>
    /// if (response.Code == 200)
    /// {
    ///     // 处理成功响应
    /// }
    /// </code>
    /// </example>
    public int Code { get; set; }

    /// <summary>
    /// 提示信息，用于向用户显示操作结果说明
    /// </summary>
    /// <example>"操作成功"、"用户不存在"、"参数验证失败"</example>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 业务数据，包含实际的响应内容
    /// </summary>
    /// <remarks>
    /// 成功时包含返回的业务数据，失败时通常为 null 或默认值。
    /// </remarks>
    public T? Data { get; set; }

    /// <summary>
    /// 时间戳（毫秒），记录响应生成的时间
    /// </summary>
    /// <remarks>
    /// 使用 UTC 时间的 Unix 时间戳（毫秒级别），可用于追踪请求和调试。
    /// </remarks>
    public long Timestamp { get; set; }

    /// <summary>
    /// 返回成功响应
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="data">响应数据，可为 null</param>
    /// <param name="message">响应消息，默认为"操作成功"</param>
    /// <returns>成功的 API 响应对象，状态码为 200</returns>
    /// <example>
    /// <code>
    /// // 返回单个对象
    /// var user = new User { Id = 1, Name = "张三" };
    /// var response = ApiResponse&lt;User&gt;.Success(user, "获取用户成功");
    ///
    /// // 返回列表
    /// var users = new List&lt;User&gt; { ... };
    /// var listResponse = ApiResponse&lt;List&lt;User&gt;&gt;.Success(users);
    /// </code>
    /// </example>
    public static ApiResponse<T> Success(T? data, string message = "操作成功")
    {
        return new ApiResponse<T>
        {
            Code = 200,
            Message = message,
            Data = data,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
    }

    /// <summary>
    /// 返回失败响应
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="message">错误消息，描述失败原因</param>
    /// <param name="code">错误码，默认为 500（服务器内部错误）</param>
    /// <returns>失败的 API 响应对象，Data 为默认值</returns>
    /// <example>
    /// <code>
    /// // 返回参数错误
    /// var response = ApiResponse&lt;User&gt;.Error("用户名不能为空", 400);
    ///
    /// // 返回资源不存在
    /// var notFoundResponse = ApiResponse&lt;User&gt;.Error("用户不存在", 404);
    ///
    /// // 返回服务器错误
    /// var errorResponse = ApiResponse&lt;User&gt;.Error("数据库连接失败");
    /// </code>
    /// </example>
    public static ApiResponse<T> Error(string message, int code = 500)
    {
        return new ApiResponse<T>
        {
            Code = code,
            Message = message,
            Data = default,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
    }

    /// <summary>
    /// 返回无数据的成功响应
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="message">响应消息，默认为"操作成功"</param>
    /// <returns>成功的 API 响应对象，Data 为默认值</returns>
    /// <remarks>
    /// 适用于不需要返回数据的操作，如删除、更新等。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 删除成功
    /// var response = ApiResponse&lt;object&gt;.Success("删除成功");
    ///
    /// // 更新成功
    /// var updateResponse = ApiResponse&lt;object&gt;.Success();
    /// </code>
    /// </example>
    public static ApiResponse<T> Success(string message = "操作成功")
    {
        return Success(default, message);
    }
}