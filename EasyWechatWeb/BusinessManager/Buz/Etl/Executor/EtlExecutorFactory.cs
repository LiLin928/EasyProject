using BusinessManager.Buz.Etl.Engine;

namespace BusinessManager.Buz.Etl.Executor;

/// <summary>
/// ETL 执行器工厂
/// 根据节点类型获取对应的执行器实例
/// </summary>
public class EtlExecutorFactory
{
    /// <summary>
    /// 执行器注册表（NodeType → Executor）
    /// </summary>
    private readonly Dictionary<string, IEtlNodeExecutor> _executors = new();

    /// <summary>
    /// 占位执行器（用于未实现的节点类型）
    /// </summary>
    private readonly PlaceholderExecutor _placeholderExecutor = new();

    /// <summary>
    /// 注册执行器
    /// </summary>
    /// <param name="executor">执行器实例</param>
    public void Register(IEtlNodeExecutor executor)
    {
        if (executor == null)
        {
            throw new ArgumentNullException(nameof(executor));
        }

        var nodeType = executor.NodeType;
        if (string.IsNullOrEmpty(nodeType))
        {
            throw new ArgumentException("执行器必须定义 NodeType", nameof(executor));
        }

        _executors[nodeType] = executor;
    }

    /// <summary>
    /// 注册多个执行器
    /// </summary>
    /// <param name="executors">执行器列表</param>
    public void RegisterAll(IEnumerable<IEtlNodeExecutor> executors)
    {
        foreach (var executor in executors)
        {
            Register(executor);
        }
    }

    /// <summary>
    /// 获取执行器
    /// </summary>
    /// <param name="nodeType">节点类型</param>
    /// <returns>执行器实例，如果未注册返回占位执行器</returns>
    public IEtlNodeExecutor GetExecutor(string nodeType)
    {
        if (_executors.TryGetValue(nodeType, out var executor))
        {
            return executor;
        }

        // 返回占位执行器（避免 null）
        return _placeholderExecutor;
    }

    /// <summary>
    /// 检查执行器是否已注册
    /// </summary>
    /// <param name="nodeType">节点类型</param>
    /// <returns>是否已注册</returns>
    public bool HasExecutor(string nodeType)
    {
        return _executors.ContainsKey(nodeType);
    }

    /// <summary>
    /// 获取所有已注册的节点类型
    /// </summary>
    public List<string> GetRegisteredTypes()
    {
        return _executors.Keys.ToList();
    }

    /// <summary>
    /// 清空所有注册
    /// </summary>
    public void Clear()
    {
        _executors.Clear();
    }

    /// <summary>
    /// 创建默认工厂（注册所有核心执行器）
    /// </summary>
    public static EtlExecutorFactory CreateDefault()
    {
        var factory = new EtlExecutorFactory();

        // Part 5: 数据源执行器
        factory.Register(new DataSourceExecutor());

        // Part 6: 转换执行器
        factory.Register(new TransformExecutor());

        // Part 7: 输出执行器
        factory.Register(new OutputExecutor());

        // Part 11: 其他执行器
        factory.Register(new SqlExecutor());
        factory.Register(new ApiExecutor());
        factory.Register(new ScriptExecutor());
        factory.Register(new ConditionExecutor());
        factory.Register(new ParallelExecutor());
        factory.Register(new NotificationExecutor());
        factory.Register(new SubflowExecutor());

        return factory;
    }
}