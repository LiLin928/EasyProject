namespace CommonManager.Interfaces;

/// <summary>
/// 网关路由接口，用于 API 网关的路由管理（Ocelot 预留）
/// </summary>
/// <remarks>
/// 该接口定义了 API 网关的路由管理方法，为后续集成 Ocelot 网关框架预留。
/// Ocelot 是一个基于 .NET Core 的 API 网关，支持路由、负载均衡、限流、认证等功能。
///
/// 应用场景：
/// - 统一 API 入口，隐藏后端服务结构
/// - 动态路由配置，无需重启网关
/// - 请求负载均衡到多个后端实例
/// - 统一认证和授权处理
/// - API 限流和保护
///
/// 实现后可支持：
/// - 动态添加/删除/修改路由
/// - 路由规则持久化
/// - 多种负载均衡策略
/// - 服务发现集成
/// - 请求聚合和转换
///
/// 预留此接口便于后续集成 Ocelot 框架，实现完整的 API 网关能力。
/// </remarks>
/// <example>
/// <code>
/// // 网关路由管理（后续实现）
/// public class GatewayService : IGatewayService
/// {
///     private readonly IGatewayRoute _routeManager;
///
///     public async Task ConfigureRoutes()
///     {
///         // 添加用户服务路由
///         await _routeManager.AddRouteAsync(new RouteConfig
///         {
///             Name = "user-service",
///             UpstreamPathTemplate = "/api/users/{everything}",
///             DownstreamPathTemplate = "/api/{everything}",
///             DownstreamScheme = "http",
///             DownstreamHost = "192.168.1.100",
///             DownstreamPort = 8080,
///             HttpMethods = new List&lt;string&gt; { "GET", "POST", "PUT", "DELETE" }
///         });
///     }
/// }
/// </code>
/// </example>
public interface IGatewayRoute
{
    /// <summary>
    /// 获取当前配置的所有路由规则
    /// </summary>
    /// <returns>路由配置列表</returns>
    /// <remarks>
    /// 获取所有已配置的路由规则，可用于：
    /// - 路由配置查看和导出
    /// - 路由状态监控
    /// - 路由管理界面展示
    /// </remarks>
    /// <example>
    /// <code>
    /// // 获取所有路由配置
    /// var routes = await _routeManager.GetRoutesAsync();
    /// foreach (var route in routes)
    /// {
    ///     Console.WriteLine($"路由 {route.Name}: {route.UpstreamPathTemplate} -> {route.DownstreamPathTemplate}");
    /// }
    /// </code>
    /// </example>
    Task<List<RouteConfig>> GetRoutesAsync();

    /// <summary>
    /// 添加新的路由规则
    /// </summary>
    /// <param name="route">路由配置对象，包含路由的所有必要信息</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// 路由配置说明：
    /// - Name：路由名称，唯一标识
    /// - UpstreamPathTemplate：上游（客户端）请求路径模板
    /// - DownstreamPathTemplate：下游（后端服务）路径模板
    /// - DownstreamScheme：下游协议（http/https）
    /// - DownstreamHost：下游服务主机地址
    /// - DownstreamPort：下游服务端口
    /// - HttpMethods：支持的 HTTP 方法列表
    ///
    /// 路径模板支持占位符：
    /// - {everything}：匹配所有路径
    /// - {id}：匹配单个参数
    /// </remarks>
    /// <example>
    /// <code>
    /// // 添加用户服务路由
    /// await _routeManager.AddRouteAsync(new RouteConfig
    /// {
    ///     Name = "user-api",
    ///     UpstreamPathTemplate = "/gateway/users/{everything}",
    ///     DownstreamPathTemplate = "/api/users/{everything}",
    ///     DownstreamScheme = "http",
    ///     DownstreamHost = "user-service-host",
    ///     DownstreamPort = 8080,
    ///     HttpMethods = new List&lt;string&gt; { "GET", "POST" }
    /// });
    ///
    /// // 添加订单服务路由
    /// await _routeManager.AddRouteAsync(new RouteConfig
    /// {
    ///     Name = "order-api",
    ///     UpstreamPathTemplate = "/gateway/orders/{id}",
    ///     DownstreamPathTemplate = "/api/orders/{id}",
    ///     DownstreamScheme = "http",
    ///     DownstreamHost = "order-service-host",
    ///     DownstreamPort = 8081
    /// });
    /// </code>
    /// </example>
    Task AddRouteAsync(RouteConfig route);

    /// <summary>
    /// 移除指定的路由规则
    /// </summary>
    /// <param name="routeName">路由名称，添加时指定的唯一标识</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// 移除路由后，对应的请求将不再转发。
    /// 移除不存在的路由不会报错。
    /// 建议在服务下线时移除对应路由。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 移除服务路由
    /// await _routeManager.RemoveRouteAsync("user-api");
    /// await _routeManager.RemoveRouteAsync("deprecated-service");
    /// </code>
    /// </example>
    Task RemoveRouteAsync(string routeName);
}

/// <summary>
/// 路由配置，定义 API 网关的路由规则
/// </summary>
/// <remarks>
/// RouteConfig 描述一个路由规则，包含：
/// - 路由名称
/// - 上游路径模板（客户端请求路径）
/// - 下游路径模板（转发到后端服务的路径）
/// - 下游服务地址和端口
/// - 支持的 HTTP 方法
///
/// 网关根据路由配置将客户端请求转发到对应的后端服务。
/// </remarks>
/// <example>
/// <code>
/// // 创建路由配置
/// var route = new RouteConfig
/// {
///     Name = "product-api",
///     UpstreamPathTemplate = "/api/products/{everything}",
///     DownstreamPathTemplate = "/products/{everything}",
///     DownstreamScheme = "http",
///     DownstreamHost = "product-service",
///     DownstreamPort = 5000,
///     HttpMethods = new List&lt;string&gt; { "GET", "POST", "PUT", "DELETE" }
/// };
///
/// // 路由示例：GET /api/products/123 -> http://product-service:5000/products/123
/// </code>
/// </example>
public class RouteConfig
{
    /// <summary>
    /// 路由名称，唯一标识一个路由规则
    /// </summary>
    /// <remarks>
    /// 用于管理和引用路由，建议使用有意义的命名。
    /// 如 "user-api"、"product-service" 等。
    /// </remarks>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 上游路径模板，客户端请求的 URL 路径匹配规则
    /// </summary>
    /// <remarks>
    /// 上游路径是网关接收的请求路径。
    /// 支持占位符：
    /// - {everything}：匹配所有后续路径
    /// - {id}、{name} 等：匹配单个路径段
    ///
    /// 示例：
    /// - "/api/users/{everything}"：匹配 /api/users 及其所有子路径
    /// - "/api/orders/{orderId}"：匹配 /api/orders/123 等格式
    /// </remarks>
    public string UpstreamPathTemplate { get; set; } = string.Empty;

    /// <summary>
    /// 下游路径模板，转发到后端服务的 URL 路径
    /// </summary>
    /// <remarks>
    /// 下游路径是转发到后端服务的实际路径。
    /// 可引用上游路径中的占位符。
    ///
    /// 示例：
    /// - "/api/{everything}"：保留 {everything} 匹配的内容
    /// - "/users/{id}"：使用上游的 {id} 占位符
    ///
    /// 路径转换示例：
    /// - 上游 /gateway/users/123 -> 下游 /users/123
    /// </remarks>
    public string DownstreamPathTemplate { get; set; } = string.Empty;

    /// <summary>
    /// 下游协议，转发请求使用的网络协议
    /// </summary>
    /// <remarks>
    /// 通常为 "http" 或 "https"。
    /// 根据后端服务的配置选择。
    /// </remarks>
    public string DownstreamScheme { get; set; } = string.Empty;

    /// <summary>
    /// 下游服务主机地址
    /// </summary>
    /// <remarks>
    /// 可以是：
    /// - IP 地址：如 "192.168.1.100"
    /// - 域名：如 "user-service.internal"
    /// - 服务名（配合服务发现）：如 "user-service"
    /// </remarks>
    public string DownstreamHost { get; set; } = string.Empty;

    /// <summary>
    /// 下游服务端口
    /// </summary>
    /// <remarks>
    /// 后端服务监听的端口，如 8080、5000 等。
    /// </remarks>
    public int DownstreamPort { get; set; }

    /// <summary>
    /// 支持的 HTTP 方法列表
    /// </summary>
    /// <remarks>
    /// 限制哪些 HTTP 方法可以通过此路由。
    /// 常见值："GET"、"POST"、"PUT"、"DELETE"、"PATCH"。
    /// 空列表表示允许所有方法。
    /// </remarks>
    public List<string> HttpMethods { get; set; } = new();
}