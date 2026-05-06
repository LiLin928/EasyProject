using BusinessManager.Buz.Etl.Engine;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models.Etl;
using EasyWeChatModels.Models.Etl.NodeConfigs;
using Newtonsoft.Json;
using SqlSugar;

namespace BusinessManager.Buz.Etl.Executor;

/// <summary>
/// 数据源节点执行器
/// 从数据库查询数据，输出到变量
/// </summary>
public class DataSourceExecutor : BaseExecutor
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public override string NodeType => "datasource";

    /// <summary>
    /// 执行数据源查询
    /// </summary>
    public override async Task<EtlNodeResult> ExecuteAsync(EtlExecutionContext context, DagNode node)
    {
        var startTime = DateTime.Now;

        try
        {
            // 检查 context.Db 是否为空
            if (context.Db == null)
            {
                return CreateFailResult("执行上下文的数据库连接(Db)为空，无法查询数据源");
            }

            // 1. 解析节点配置
            var config = ParseConfig<DataSourceNodeConfig>(node.Config);

            // 检查解析后的配置
            if (string.IsNullOrEmpty(config.DatasourceId))
            {
                return CreateFailResult($"数据源ID解析失败，原始配置: {node.Config}，解析后 DatasourceId 为空。请检查节点配置是否使用 camelCase 格式（如 datasourceId 而非 DatasourceId）");
            }

            // 3. 获取数据源连接信息
            var datasource = await GetDatasourceAsync(context.Db, config.DatasourceId);
            if (datasource == null)
            {
                return CreateFailResult($"数据源不存在: {config.DatasourceId}");
            }

            // 检查数据源的必要属性
            if (string.IsNullOrEmpty(datasource.Host))
            {
                return CreateFailResult($"数据源 {datasource.Name} 的 Host 为空，请检查数据源配置");
            }

            // 4. 创建动态数据库连接
            var dynamicDb = CreateDynamicDb(datasource);

            // 5. 执行查询
            var queryResult = await ExecuteQueryAsync(dynamicDb, config, context, node.Id);

            // 6. 构建输出变量
            var outputs = new Dictionary<string, object>
            {
                [config.OutputVariable] = queryResult.Data,
                ["_count"] = queryResult.Count,
                ["_sourceType"] = datasource.Type ?? "unknown",
                ["_queryTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return CreateSuccessResult(outputs, queryResult.Count);
        }
        catch (Exception ex)
        {
            return CreateFailResult($"执行数据源查询失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 验证配置
    /// </summary>
    public override string? ValidateConfig(DagNode node)
    {
        var config = ParseConfig<DataSourceNodeConfig>(node.Config);

        if (string.IsNullOrEmpty(config.DatasourceId))
        {
            return "数据源ID不能为空";
        }

        if (config.QueryType == "table" && string.IsNullOrEmpty(config.TableName))
        {
            return "查询类型为 table 时，表名不能为空";
        }

        if (config.QueryType == "sql" && string.IsNullOrEmpty(config.Sql))
        {
            return "查询类型为 sql 时，SQL 语句不能为空";
        }

        if (string.IsNullOrEmpty(config.OutputVariable))
        {
            return "输出变量名不能为空";
        }

        return null;
    }

    /// <summary>
    /// 获取数据源实体
    /// </summary>
    private async Task<Datasource?> GetDatasourceAsync(ISqlSugarClient db, string datasourceId)
    {
        // 尝试解析为 Guid
        if (!Guid.TryParse(datasourceId, out var id))
        {
            // 可能是数据源名称，按名称查询
            return await db.Queryable<Datasource>()
                .Where(x => x.Name == datasourceId)
                .FirstAsync();
        }

        // 按 ID 查询
        return await db.Queryable<Datasource>()
            .Where(x => x.Id == id)
            .FirstAsync();
    }

    /// <summary>
    /// 创建动态数据库连接
    /// </summary>
    private SqlSugarScope CreateDynamicDb(Datasource datasource)
    {
        // 构建连接字符串
        var connectionString = BuildConnectionString(datasource);
        var dbType = ParseDbType(datasource.Type ?? "mysql");

        return new SqlSugarScope(new ConnectionConfig
        {
            ConnectionString = connectionString,
            DbType = dbType,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });
    }

    /// <summary>
    /// 构建数据库连接字符串
    /// </summary>
    private string BuildConnectionString(Datasource datasource)
    {
        var dbType = (datasource.Type ?? "mysql").ToLower();

        return dbType switch
        {
            "mysql" => $"Server={datasource.Host};Port={datasource.Port};Database={datasource.Database};User ID={datasource.Username};Password={datasource.Password};Charset=utf8mb4;",
            "sqlserver" => $"Server={datasource.Host},{datasource.Port};Database={datasource.Database};User ID={datasource.Username};Password={datasource.Password};TrustServerCertificate=True;",
            "postgresql" => $"Host={datasource.Host};Port={datasource.Port};Database={datasource.Database};Username={datasource.Username};Password={datasource.Password};",
            "oracle" => $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={datasource.Host})(PORT={datasource.Port}))(CONNECT_DATA=(SERVICE_NAME={datasource.Database})));User ID={datasource.Username};Password={datasource.Password};",
            "sqlite" => $"Data Source={datasource.Database};",
            "clickhouse" => $"Host={datasource.Host};Port={datasource.Port};Database={datasource.Database};User={datasource.Username};Password={datasource.Password};",
            _ => $"Server={datasource.Host};Port={datasource.Port};Database={datasource.Database};User ID={datasource.Username};Password={datasource.Password};"
        };
    }

    /// <summary>
    /// 解析数据库类型
    /// </summary>
    private DbType ParseDbType(string dbTypeStr)
    {
        return dbTypeStr.ToLower() switch
        {
            "mysql" => DbType.MySql,
            "sqlserver" => DbType.SqlServer,
            "sqlite" => DbType.Sqlite,
            "oracle" => DbType.Oracle,
            "postgresql" => DbType.PostgreSQL,
            "clickhouse" => DbType.ClickHouse,
            "0" => DbType.MySql,
            "1" => DbType.SqlServer,
            "2" => DbType.Sqlite,
            "3" => DbType.Oracle,
            "4" => DbType.PostgreSQL,
            "5" => DbType.ClickHouse,
            _ => DbType.MySql
        };
    }

    /// <summary>
    /// 执行查询
    /// </summary>
    private async Task<QueryResult> ExecuteQueryAsync(
        SqlSugarScope db,
        DataSourceNodeConfig config,
        EtlExecutionContext context,
        string nodeId)
    {
        // 获取上游变量（用于变量替换）
        var upstreamVars = GetUpstreamVariables(context, nodeId);

        // 根据查询类型执行
        if (config.QueryType == "table")
        {
            return await ExecuteTableQueryAsync(db, config, upstreamVars);
        }
        else if (config.QueryType == "sql")
        {
            return await ExecuteSqlQueryAsync(db, config, upstreamVars);
        }
        else
        {
            throw new NotSupportedException($"不支持的查询类型: {config.QueryType}");
        }
    }

    /// <summary>
    /// 执行表查询
    /// </summary>
    private async Task<QueryResult> ExecuteTableQueryAsync(
        SqlSugarScope db,
        DataSourceNodeConfig config,
        Dictionary<string, object> upstreamVars)
    {
        // 替换变量
        var tableName = ReplaceVariables(config.TableName!, upstreamVars);
        var whereClause = ReplaceVariables(config.WhereClause, upstreamVars);

        // 构建查询
        var query = db.Queryable<dynamic>()
            .AS(tableName);

        // 添加列选择
        if (config.Columns != null && config.Columns.Count > 0)
        {
            var selectColumns = config.Columns.Select(c => ReplaceVariables(c, upstreamVars)).ToList();
            query = query.Select(string.Join(",", selectColumns));
        }

        // 添加 WHERE 条件
        if (!string.IsNullOrEmpty(whereClause))
        {
            query = query.Where(whereClause);
        }

        // 添加 LIMIT
        if (config.Limit.HasValue && config.Limit.Value > 0)
        {
            query = query.Take(config.Limit.Value);
        }

        // 执行查询
        var data = await query.ToDataTableAsync();

        // 转换为字典列表
        var list = DataTableToDictionaryList(data);

        return new QueryResult
        {
            Data = list,
            Count = list.Count
        };
    }

    /// <summary>
    /// 执行 SQL 查询
    /// </summary>
    private async Task<QueryResult> ExecuteSqlQueryAsync(
        SqlSugarScope db,
        DataSourceNodeConfig config,
        Dictionary<string, object> upstreamVars)
    {
        // 替换 SQL 中的变量
        var sql = ReplaceVariables(config.Sql!, upstreamVars);

        // 执行查询
        var data = await db.SqlQueryable<object>(sql).ToDataTableAsync();

        // 转换为字典列表
        var list = DataTableToDictionaryList(data);

        return new QueryResult
        {
            Data = list,
            Count = list.Count
        };
    }

    /// <summary>
    /// 替换变量（支持 ${varName} 和 :varName 格式）
    /// </summary>
    private string ReplaceVariables(string? template, Dictionary<string, object> variables)
    {
        if (string.IsNullOrEmpty(template))
        {
            return template ?? string.Empty;
        }

        var result = template;

        // 替换 ${varName} 格式
        foreach (var (key, value) in variables)
        {
            var placeholder = $"${{{key}}}";
            if (result.Contains(placeholder))
            {
                result = result.Replace(placeholder, value?.ToString() ?? string.Empty);
            }
        }

        // 替换 :varName 格式（SQL 参数格式）
        foreach (var (key, value) in variables)
        {
            var placeholder = $":{key}";
            if (result.Contains(placeholder))
            {
                // 如果值是字符串，需要加引号
                var replacement = value is string
                    ? $"'{value}'"
                    : value?.ToString() ?? "null";
                result = result.Replace(placeholder, replacement);
            }
        }

        return result;
    }

    /// <summary>
    /// DataTable 转换为字典列表
    /// </summary>
    private List<Dictionary<string, object>> DataTableToDictionaryList(System.Data.DataTable dataTable)
    {
        var list = new List<Dictionary<string, object>>();

        foreach (System.Data.DataRow row in dataTable.Rows)
        {
            var dict = new Dictionary<string, object>();
            foreach (System.Data.DataColumn column in dataTable.Columns)
            {
                var value = row[column];
                // 处理 DBNull
                dict[column.ColumnName] = value == System.DBNull.Value ? null! : value;
            }
            list.Add(dict);
        }

        return list;
    }

    /// <summary>
    /// 查询结果内部类
    /// </summary>
    private class QueryResult
    {
        /// <summary>
        /// 查询数据（字典列表）
        /// </summary>
        public List<Dictionary<string, object>> Data { get; set; } = new();

        /// <summary>
        /// 数据行数
        /// </summary>
        public int Count { get; set; }
    }
}