namespace CommonManager.Error;

/// <summary>
/// 业务异常类，用于在业务逻辑中抛出可预期的异常
/// </summary>
/// <remarks>
/// 该类继承自 Exception，添加了 ErrorCode 属性用于区分不同类型的业务错误。
/// 通过静态方法可以快速创建常见类型的业务异常。
/// 在全局异常处理中可以根据 ErrorCode 返回不同的 HTTP 状态码。
/// </remarks>
/// <example>
/// <code>
/// // 抛出资源不存在异常
/// if (user == null)
///     throw BusinessException.NotFound("用户不存在");
///
/// // 抛出参数错误异常
/// if (string.IsNullOrEmpty(userName))
///     throw BusinessException.BadRequest("用户名不能为空");
///
/// // 抛出自定义业务异常
/// throw new BusinessException("余额不足", 400);
/// </code>
/// </example>
public class BusinessException : Exception
{
    /// <summary>
    /// 错误码，用于区分不同类型的业务错误
    /// </summary>
    /// <remarks>
    /// 常见错误码：
    /// - 400：参数错误（BadRequest）
    /// - 401：未授权访问（Unauthorized）
    /// - 403：禁止访问（Forbidden）
    /// - 404：资源不存在（NotFound）
    /// - 500：服务器内部错误
    /// </remarks>
    public int ErrorCode { get; set; }

    /// <summary>
    /// 默认构造函数，创建错误码为 400 的业务异常
    /// </summary>
    /// <remarks>
    /// 默认消息为空，错误码为 400（参数错误）。
    /// </remarks>
    public BusinessException() : base()
    {
        ErrorCode = 400;
    }

    /// <summary>
    /// 创建指定消息的业务异常，错误码默认为 400
    /// </summary>
    /// <param name="message">异常消息，描述错误原因</param>
    /// <remarks>
    /// 适用于一般的业务错误，错误码默认为 400。
    /// </remarks>
    /// <example>
    /// <code>
    /// throw new BusinessException("用户名已存在");
    /// </code>
    /// </example>
    public BusinessException(string message) : base(message)
    {
        ErrorCode = 400;
    }

    /// <summary>
    /// 创建指定消息和错误码的业务异常
    /// </summary>
    /// <param name="message">异常消息，描述错误原因</param>
    /// <param name="errorCode">错误码，用于区分错误类型</param>
    /// <remarks>
    /// 通过自定义错误码可以灵活处理不同的业务场景。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 抛出余额不足异常（自定义错误码）
    /// throw new BusinessException("余额不足", 1001);
    ///
    /// // 抛出权限不足异常
    /// throw new BusinessException("无权访问该资源", 403);
    /// </code>
    /// </example>
    public BusinessException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// 创建包含内部异常的业务异常
    /// </summary>
    /// <param name="message">异常消息，描述错误原因</param>
    /// <param name="innerException">内部异常对象</param>
    /// <remarks>
    /// 适用于包装其他异常，保留原始异常信息用于调试。
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     // 可能抛出异常的操作
    ///     await ProcessPaymentAsync(order);
    /// }
    /// catch (PaymentGatewayException ex)
    /// {
    ///     throw new BusinessException("支付处理失败，请稍后重试", ex);
    /// }
    /// </code>
    /// </example>
    public BusinessException(string message, Exception innerException) : base(message, innerException)
    {
        ErrorCode = 400;
    }

    /// <summary>
    /// 创建资源不存在异常（404）
    /// </summary>
    /// <param name="message">异常消息，默认为"资源不存在"</param>
    /// <returns>错误码为 404 的业务异常对象</returns>
    /// <remarks>
    /// 用于表示请求的资源不存在，如用户、订单、商品等。
    /// 全局异常处理应将此异常转换为 HTTP 404 响应。
    /// </remarks>
    /// <example>
    /// <code>
    /// var user = await _userService.GetByIdAsync(userId);
    /// if (user == null)
    ///     throw BusinessException.NotFound("用户不存在");
    ///
    /// var order = await _orderService.GetByIdAsync(orderId);
    /// if (order == null)
    ///     throw BusinessException.NotFound($"订单 {orderId} 不存在");
    /// </code>
    /// </example>
    public static BusinessException NotFound(string message = "资源不存在")
    {
        return new BusinessException(message, 404);
    }

    /// <summary>
    /// 创建参数错误异常（400）
    /// </summary>
    /// <param name="message">异常消息，默认为"参数错误"</param>
    /// <returns>错误码为 400 的业务异常对象</returns>
    /// <remarks>
    /// 用于表示请求参数验证失败或格式错误。
    /// 全局异常处理应将此异常转换为 HTTP 400 响应。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 参数验证
    /// if (string.IsNullOrEmpty(userName))
    ///     throw BusinessException.BadRequest("用户名不能为空");
    ///
    /// if (age &lt; 0 || age &gt; 150)
    ///     throw BusinessException.BadRequest("年龄必须在 0-150 之间");
    /// </code>
    /// </example>
    public static BusinessException BadRequest(string message = "参数错误")
    {
        return new BusinessException(message, 400);
    }

    /// <summary>
    /// 创建未授权访问异常（401）
    /// </summary>
    /// <param name="message">异常消息，默认为"未授权访问"</param>
    /// <returns>错误码为 401 的业务异常对象</returns>
    /// <remarks>
    /// 用于表示用户未登录或 Token 无效。
    /// 全局异常处理应将此异常转换为 HTTP 401 响应。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 检查用户是否登录
    /// var userId = GetCurrentUserId();
    /// if (userId == 0)
    ///     throw BusinessException.Unauthorized("请先登录");
    ///
    /// // 检查 Token 是否有效
    /// if (!ValidateToken(token))
    ///     throw BusinessException.Unauthorized("Token 已过期，请重新登录");
    /// </code>
    /// </example>
    public static BusinessException Unauthorized(string message = "未授权访问")
    {
        return new BusinessException(message, 401);
    }

    /// <summary>
    /// 创建禁止访问异常（403）
    /// </summary>
    /// <param name="message">异常消息，默认为"禁止访问"</param>
    /// <returns>错误码为 403 的业务异常对象</returns>
    /// <remarks>
    /// 用于表示用户已登录但无权限执行该操作。
    /// 全局异常处理应将此异常转换为 HTTP 403 响应。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 检查用户权限
    /// if (!HasPermission(userId, "admin"))
    ///     throw BusinessException.Forbidden("需要管理员权限");
    ///
    /// // 检查资源访问权限
    /// if (order.UserId != currentUserId)
    ///     throw BusinessException.Forbidden("无权访问此订单");
    /// </code>
    /// </example>
    public static BusinessException Forbidden(string message = "禁止访问")
    {
        return new BusinessException(message, 403);
    }
}