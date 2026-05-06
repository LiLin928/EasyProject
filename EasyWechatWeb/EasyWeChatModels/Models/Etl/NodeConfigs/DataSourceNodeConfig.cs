namespace EasyWeChatModels.Models.Etl.NodeConfigs;

/// <summary>
/// 数据源节点配置
/// </summary>
public class DataSourceNodeConfig
{
    /// <summary>数据源ID</summary>
    public string DatasourceId { get; set; } = string.Empty;

    /// <summary>查询类型：table, sql</summary>
    public string QueryType { get; set; } = "table";

    /// <summary>表名（table 模式）</summary>
    public string? TableName { get; set; }

    /// <summary>SQL 语句（sql 模式）</summary>
    public string? Sql { get; set; }

    /// <summary>选择列</summary>
    public List<string>? Columns { get; set; }

    /// <summary>WHERE 条件</summary>
    public string? WhereClause { get; set; }

    /// <summary>数据行数限制</summary>
    public int? Limit { get; set; }

    /// <summary>输出变量名</summary>
    public string OutputVariable { get; set; } = "data";
}