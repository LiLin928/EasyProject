using BusinessManager.Buz.Etl.Engine;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models.Etl;
using EasyWeChatModels.Models.Etl.NodeConfigs;
using SqlSugar;

namespace BusinessManager.Buz.Etl.Executor;

/// <summary>
/// SQL节点执行器
/// 执行自定义SQL语句
/// </summary>
public class SqlExecutor : BaseExecutor
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public override string NodeType => "sql";

    /// <summary>
    /// 执行SQL
    /// </summary>
    public override async Task<EtlNodeResult> ExecuteAsync(EtlExecutionContext context, DagNode node)
    {
        var startTime = DateTime.Now;

        try
        {
            var config = ParseConfig<SqlNodeConfig>(node.Config);

            // 获取数据源
            var datasource = await GetDatasourceAsync(context.Db, config.DatasourceId);
            if (datasource == null)
            {
                return CreateFailResult($"数据源不存在: {config.DatasourceId}");
            }

            // 创建动态连接
            var dynamicDb = CreateDynamicDb(datasource);

            // 替换变量
            var upstreamVars = GetUpstreamVariables(context, node.Id);
            var sql = ReplaceVariables(config.Sql, upstreamVars);

            // 根据SQL类型执行
            switch (config.SqlType)
            {
                case "query":
                    return await ExecuteQueryAsync(dynamicDb, sql, config, context, node.Id);

                case "insert":
                case "update":
                case "delete":
                    return await ExecuteCommandAsync(dynamicDb, sql, config);

                case "ddl":
                    return await ExecuteDdlAsync(dynamicDb, sql);

                default:
                    return CreateFailResult($"不支持的SQL类型: {config.SqlType}");
            }
        }
        catch (Exception ex)
        {
            return CreateFailResult($"执行SQL失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 验证配置
    /// </summary>
    public override string? ValidateConfig(DagNode node)
    {
        var config = ParseConfig<SqlNodeConfig>(node.Config);

        if (string.IsNullOrEmpty(config.DatasourceId))
            return "数据源ID不能为空";

        if (string.IsNullOrEmpty(config.Sql))
            return "SQL语句不能为空";

        if (config.SqlType == "query" && string.IsNullOrEmpty(config.OutputVariable))
            return "查询SQL需要配置输出变量名";

        return null;
    }

    private async Task<Datasource?> GetDatasourceAsync(ISqlSugarClient db, string datasourceId)
    {
        if (!Guid.TryParse(datasourceId, out var id))
        {
            return await db.Queryable<Datasource>()
                .Where(x => x.Name == datasourceId)
                .FirstAsync();
        }
        return await db.Queryable<Datasource>()
            .Where(x => x.Id == id)
            .FirstAsync();
    }

    private SqlSugarScope CreateDynamicDb(Datasource datasource)
    {
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

    private string BuildConnectionString(Datasource datasource)
    {
        var dbType = (datasource.Type ?? "mysql").ToLower();
        return dbType switch
        {
            "mysql" => $"Server={datasource.Host};Port={datasource.Port};Database={datasource.Database};User ID={datasource.Username};Password={datasource.Password};Charset=utf8mb4;",
            "sqlserver" => $"Server={datasource.Host},{datasource.Port};Database={datasource.Database};User ID={datasource.Username};Password={datasource.Password};TrustServerCertificate=True;",
            "postgresql" => $"Host={datasource.Host};Port={datasource.Port};Database={datasource.Database};Username={datasource.Username};Password={datasource.Password};",
            _ => $"Server={datasource.Host};Port={datasource.Port};Database={datasource.Database};User ID={datasource.Username};Password={datasource.Password};"
        };
    }

    private DbType ParseDbType(string dbTypeStr)
    {
        return dbTypeStr.ToLower() switch
        {
            "mysql" => DbType.MySql,
            "sqlserver" => DbType.SqlServer,
            "postgresql" => DbType.PostgreSQL,
            "oracle" => DbType.Oracle,
            "sqlite" => DbType.Sqlite,
            _ => DbType.MySql
        };
    }

    private string ReplaceVariables(string template, Dictionary<string, object> variables)
    {
        var result = template;
        foreach (var (key, value) in variables)
        {
            var placeholder = $"${{{key}}}";
            if (result.Contains(placeholder))
            {
                var replacement = value is string ? $"'{value}'" : value?.ToString() ?? "null";
                result = result.Replace(placeholder, replacement);
            }
        }
        return result;
    }

    private async Task<EtlNodeResult> ExecuteQueryAsync(SqlSugarScope db, string sql,
        SqlNodeConfig config, EtlExecutionContext context, string nodeId)
    {
        var data = await db.SqlQueryable<object>(sql).ToDataTableAsync();
        var list = DataTableToDictionaryList(data);

        var outputs = new Dictionary<string, object>
        {
            [config.OutputVariable!] = list,
            ["_count"] = list.Count,
            ["_queryTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        return CreateSuccessResult(outputs, list.Count);
    }

    private async Task<EtlNodeResult> ExecuteCommandAsync(SqlSugarScope db, string sql, SqlNodeConfig config)
    {
        var affected = await db.Ado.ExecuteCommandAsync(sql);

        var outputs = new Dictionary<string, object>
        {
            ["_affectedRows"] = affected,
            ["_executeTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        return CreateSuccessResult(outputs, affected);
    }

    private async Task<EtlNodeResult> ExecuteDdlAsync(SqlSugarScope db, string sql)
    {
        await db.Ado.ExecuteCommandAsync(sql);

        var outputs = new Dictionary<string, object>
        {
            ["_ddlExecuted"] = true,
            ["_executeTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        return CreateSuccessResult(outputs, 0);
    }

    private List<Dictionary<string, object>> DataTableToDictionaryList(System.Data.DataTable dataTable)
    {
        var list = new List<Dictionary<string, object>>();
        foreach (System.Data.DataRow row in dataTable.Rows)
        {
            var dict = new Dictionary<string, object>();
            foreach (System.Data.DataColumn column in dataTable.Columns)
            {
                var value = row[column];
                dict[column.ColumnName] = value == System.DBNull.Value ? null! : value;
            }
            list.Add(dict);
        }
        return list;
    }
}