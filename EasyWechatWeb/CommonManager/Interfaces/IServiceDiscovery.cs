namespace CommonManager.Interfaces;

/// <summary>
/// 服务发现接口，用于微服务架构中的服务注册与发现（Consul 预留）
/// </summary>
/// <remarks>
/// 该接口定义了服务注册与发现的基本方法，为后续集成 Consul 服务发现框架预留。
/// Consul 是一个分布式服务发现和配置管理系统，提供服务注册、健康检查、KV 存储等功能。
///
/// 应用场景：
/// - 微服务动态注册和发现
/// - 服务健康状态监控
/// - 服务负载均衡
/// - 服务配置集中管理
///
/// 实现后可支持：
/// - 服务启动时自动注册到 Consul
/// - 服务停止时自动注销
/// - 定期健康检查，自动剔除故障服务
/// - 客户端通过 Consul 发现可用服务实例
/// - 多数据中心支持
///
/// 预留此接口便于后续集成 Consul 框架，实现完整的微服务治理能力。
/// </remarks>
/// <example>
/// <code>
/// // 服务注册与发现（后续实现）
/// public class OrderService : IOrderService
/// {
///     private readonly IServiceDiscovery _discovery;
///
///     public async Task CallPaymentService(PaymentRequest request)
///     {
///         // 从 Consul 发现支付服务实例
///         var paymentService = await _discovery.DiscoverServiceAsync("payment-service");
///
///         // 调用支付服务
///         var response = await CallService(paymentService, request);
///     }
/// }
///
/// // 应用启动时注册服务
/// public void ConfigureServices(IServiceCollection services)
/// {
///     var discovery = new ConsulServiceDiscovery(consulConfig);
///     discovery.RegisterServiceAsync(
///         "order-service",
///         "order-service-instance-1",
///         "192.168.1.100",
///         8080
///     );
/// }
/// </code>
/// </example>
public interface IServiceDiscovery
{
    /// <summary>
    /// 注册服务到服务发现中心
    /// </summary>
    /// <param name="serviceName">服务名称，用于服务发现时的查询标识，如 "user-service"、"order-service"</param>
    /// <param name="serviceId">服务实例 ID，唯一标识一个服务实例，如 "user-service-192.168.1.100-8080"</param>
    /// <param name="address">服务实例的网络地址，如 IP 地址 "192.168.1.100"</param>
    /// <param name="port">服务实例的端口，如 8080</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// 服务注册流程：
    /// 1. 服务启动时调用 RegisterServiceAsync 注册自身
    /// 2. Consul 记录服务信息并开始健康检查
    /// 3. 其他服务可通过 DiscoverServiceAsync 发现此服务
    /// 4. 服务停止时调用 DeregisterServiceAsync 注销
    ///
    /// 命名规范：
    /// - serviceName：统一命名，如模块名-service
    /// - serviceId：唯一标识，建议包含地址和端口
    /// </remarks>
    /// <example>
    /// <code>
    /// // 注册用户服务
    /// await _discovery.RegisterServiceAsync(
    ///     "user-service",
    ///     "user-service-192.168.1.100-8080",
    ///     "192.168.1.100",
    ///     8080
    /// );
    ///
    /// // 注册订单服务（带标签）
    /// await _discovery.RegisterServiceAsync(
    ///     "order-service",
    ///     "order-service-instance-1",
    ///     "10.0.0.1",
    ///     5000
    /// );
    /// </code>
    /// </example>
    Task RegisterServiceAsync(string serviceName, string serviceId, string address, int port);

    /// <summary>
    /// 从服务发现中心注销服务
    /// </summary>
    /// <param name="serviceId">服务实例 ID，注册时使用的唯一标识</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// 服务注销流程：
    /// 1. 服务停止前调用 DeregisterServiceAsync 注销
    /// 2. Consul 移除服务记录，停止健康检查
    /// 3. 其他服务无法再发现此实例
    ///
    /// 建议在应用退出事件中调用，确保服务正常注销：
    /// - 使用 IHostApplicationLifetime.ApplicationStopping 事件
    /// - 避免服务突然停止导致的注册残留
    /// </remarks>
    /// <example>
    /// <code>
    /// // 应用停止时注销服务
    /// public void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime)
    /// {
    ///     lifetime.ApplicationStopping.Register(() =>
    ///     {
    ///         _discovery.DeregisterServiceAsync("user-service-192.168.1.100-8080");
    ///     });
    /// }
    /// </code>
    /// </example>
    Task DeregisterServiceAsync(string serviceId);

    /// <summary>
    /// 发现指定名称的服务实例
    /// </summary>
    /// <param name="serviceName">服务名称，注册时使用的服务标识</param>
    /// <returns>发现的服务信息，包含地址和端口；未发现时返回 null</returns>
    /// <remarks>
    /// 服务发现流程：
    /// 1. 从 Consul 查询指定名称的服务
    /// 2. 获取健康的服务实例列表
    /// 3. 返回一个实例信息（通常使用负载均衡策略选择）
    ///
    /// 负载均衡策略（实现时可选择）：
    /// - 随机选择：Random
    /// - 轮询：RoundRobin
    /// - 最少连接：LeastConnection
    /// - 权重：Weighted
    /// </remarks>
    /// <example>
    /// <code>
    /// // 发现支付服务
    /// var paymentService = await _discovery.DiscoverServiceAsync("payment-service");
    /// if (paymentService != null)
    /// {
    ///     var url = $"http://{paymentService.Address}:{paymentService.Port}/api/pay";
    ///     await CallPaymentApi(url);
    /// }
    /// </code>
    /// </example>
    Task<ServiceInfo?> DiscoverServiceAsync(string serviceName);

    /// <summary>
    /// 发现指定名称的所有服务实例
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <returns>所有健康的服务实例列表</returns>
    /// <remarks>
    /// 获取所有实例的场景：
    /// - 客户端自行实现负载均衡
    /// - 批量操作所有实例
    /// - 监控和统计服务状态
    /// </remarks>
    /// <example>
    /// <code>
    /// // 获取所有支付服务实例
    /// var instances = await _discovery.DiscoverAllServicesAsync("payment-service");
    /// Console.WriteLine($"支付服务实例数: {instances.Count}");
    ///
    /// // 客户端负载均衡
    /// var selectedInstance = SelectByRoundRobin(instances);
    /// await CallService(selectedInstance);
    /// </code>
    /// </example>
    Task<List<ServiceInfo>> DiscoverAllServicesAsync(string serviceName);
}

/// <summary>
/// 服务信息，包含服务实例的基本信息
/// </summary>
/// <remarks>
/// ServiceInfo 描述一个服务实例的基本属性，包括：
/// - 唯一标识（ServiceId）
/// - 服务名称（ServiceName）
/// - 网络地址（Address）
/// - 端口号（Port）
/// - 标签信息（Tags）
///
/// 客户端根据这些信息连接到服务实例。
/// </remarks>
/// <example>
/// <code>
/// // 使用服务信息连接服务
/// var service = await _discovery.DiscoverServiceAsync("user-service");
/// var httpClient = new HttpClient();
/// var url = $"http://{service.Address}:{service.Port}/api/users";
/// var response = await httpClient.GetAsync(url);
///
/// // 检查服务标签
/// if (service.Tags.TryGetValue("version", out var version))
/// {
///     Console.WriteLine($"服务版本: {version}");
/// }
/// </code>
/// </example>
public class ServiceInfo
{
    /// <summary>
    /// 服务实例 ID，唯一标识一个服务实例
    /// </summary>
    /// <remarks>
    /// 格式建议：服务名-地址-端口，如 "user-service-192.168.1.100-8080"
    /// </remarks>
    public string ServiceId { get; set; } = string.Empty;

    /// <summary>
    /// 服务名称，用于服务发现时的查询标识
    /// </summary>
    /// <remarks>
    /// 同一类服务的所有实例共享相同的服务名称。
    /// </remarks>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// 服务实例的网络地址（IP 或域名）
    /// </summary>
    /// <remarks>
    /// 通常是 IP 地址，如 "192.168.1.100" 或 "10.0.0.1"。
    /// 客户端使用此地址连接服务。
    /// </remarks>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 服务实例的端口号
    /// </summary>
    /// <remarks>
    /// 服务监听的端口，如 8080、5000 等。
    /// 完整的服务地址为 Address:Port。
    /// </remarks>
    public int Port { get; set; }

    /// <summary>
    /// 服务标签字典，包含自定义的元数据信息
    /// </summary>
    /// <remarks>
    /// 标签可用于：
    /// - 服务版本标识
    /// - 服务分组
    /// - 环境标识（dev/test/prod）
    /// - 其他自定义元数据
    ///
    /// 示例：
    /// - "version": "v1.2.0"
    /// - "environment": "production"
    /// - "group": "backend"
    /// </remarks>
    public Dictionary<string, string> Tags { get; set; } = new();
}