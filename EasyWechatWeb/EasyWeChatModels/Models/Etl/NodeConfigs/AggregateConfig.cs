namespace EasyWeChatModels.Models.Etl.NodeConfigs;

/// <summary>
/// 聚合配置
/// </summary>
public class AggregateConfig
{
    /// <summary>分组字段列表</summary>
    public List<string>? GroupBy { get; set; }

    /// <summary>聚合函数列表</summary>
    public List<AggregateItem> Aggregations { get; set; } = new();
}

/// <summary>
/// 聚合项
/// </summary>
public class AggregateItem
{
    /// <summary>聚合函数：sum, avg, count, max, min</summary>
    public string Function { get; set; } = "sum";

    /// <summary>聚合字段</summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>输出别名</summary>
    public string? Alias { get; set; }
}