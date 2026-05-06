using BusinessManager.Buz.Etl.Engine;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models.Etl;
using EasyWeChatModels.Models.Etl.NodeConfigs;
using SqlSugar;

namespace BusinessManager.Buz.Etl.Executor;

/// <summary>
/// 输出节点执行器
/// 将转换后的数据写入目标数据库表
/// </summary>
public class OutputExecutor : BaseExecutor
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public override string NodeType => "output";

    /// <summary>
    /// 执行数据输出
    /// </summary>
    public override async Task<EtlNodeResult> ExecuteAsync(EtlExecutionContext context, DagNode node)
    {
        var startTime = DateTime.Now;

        try
        {
            // 1. 解析节点配置
            var config = ParseConfig<OutputNodeConfig>(node.Config);

            // 2. 验证必要字段
            var validationError = ValidateConfig(node);
            if (validationError != null)
            {
                return CreateFailResult(validationError);
            }

            // 3. 获取输入数据
            var inputData = GetInputData(context, node.Id, config.InputVariable);
            if (inputData == null || inputData.Count == 0)
            {
                return CreateSuccessResult(new Dictionary<string, object>
                {
                    ["_count"] = 0,
                    ["_outputType"] = config.OutputType,
                    ["_tableName"] = config.TableName
                }, 0);
            }

            // 4. 获取数据源连接信息
            var datasource = await GetDatasourceAsync(context.Db, config.DatasourceId);
            if (datasource == null)
            {
                return CreateFailResult($"数据源不存在: {config.DatasourceId}");
            }

            // 5. 创建动态数据库连接
            var dynamicDb = CreateDynamicDb(datasource);

            // 6. 执行数据写入
            var writeResult = await ExecuteWriteAsync(dynamicDb, config, inputData, context, node.Id);

            // 7. 构建输出变量
            var outputs = new Dictionary<string, object>
            {
                ["_count"] = writeResult.ProcessedRows,
                ["_outputType"] = config.OutputType,
                ["_tableName"] = config.TableName,
                ["_writeTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ["_affectedRows"] = writeResult.AffectedRows
            };

            return CreateSuccessResult(outputs, writeResult.ProcessedRows);
        }
        catch (Exception ex)
        {
            return CreateFailResult($"执行数据输出失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 验证配置
    /// </summary>
    public override string? ValidateConfig(DagNode node)
    {
        var config = ParseConfig<OutputNodeConfig>(node.Config);

        if (string.IsNullOrEmpty(config.DatasourceId))
        {
            return "目标数据源ID不能为空";
        }

        if (string.IsNullOrEmpty(config.TableName))
        {
            return "目标表名不能为空";
        }

        if (string.IsNullOrEmpty(config.InputVariable))
        {
            return "输入变量名不能为空";
        }

        // 检查输出类型是否有效
        var validOutputTypes = new[] { "insert", "update", "merge", "truncate_insert" };
        if (!validOutputTypes.Contains(config.OutputType))
        {
            return $"不支持的输出类型: {config.OutputType}";
        }

        // update 和 merge 需要有主键字段
        if ((config.OutputType == "update" || config.OutputType == "merge") && config.FieldMapping != null)
        {
            var hasKeyField = config.FieldMapping.Any(m => m.IsKey);
            if (!hasKeyField)
            {
                return $"{config.OutputType} 输出类型需要在字段映射中标记主键字段（isKey=true）";
            }
        }

        return null;
    }

    /// <summary>
    /// 获取数据源实体
    /// </summary>
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

    /// <summary>
    /// 创建动态数据库连接
    /// </summary>
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
    /// 执行数据写入
    /// </summary>
    private async Task<WriteResult> ExecuteWriteAsync(
        SqlSugarScope db,
        OutputNodeConfig config,
        List<Dictionary<string, object>> inputData,
        EtlExecutionContext context,
        string nodeId)
    {
        // 应用字段映射
        var mappedData = ApplyFieldMapping(inputData, config.FieldMapping);

        // 根据输出类型执行写入
        switch (config.OutputType)
        {
            case "insert":
                return await ExecuteInsertAsync(db, config, mappedData);

            case "update":
                return await ExecuteUpdateAsync(db, config, mappedData);

            case "merge":
                return await ExecuteMergeAsync(db, config, mappedData);

            case "truncate_insert":
                return await ExecuteTruncateInsertAsync(db, config, mappedData);

            default:
                throw new NotSupportedException($"不支持的输出类型: {config.OutputType}");
        }
    }

    /// <summary>
    /// 应用字段映射
    /// </summary>
    private List<Dictionary<string, object>> ApplyFieldMapping(
        List<Dictionary<string, object>> inputData,
        List<FieldMappingItem>? fieldMapping)
    {
        if (fieldMapping == null || fieldMapping.Count == 0)
        {
            // 无映射，直接返回原数据
            return inputData;
        }

        var result = new List<Dictionary<string, object>>();

        foreach (var row in inputData)
        {
            var newRow = new Dictionary<string, object>();

            foreach (var mapping in fieldMapping)
            {
                // 获取源字段值
                var sourceValue = row.TryGetValue(mapping.SourceField, out var val) ? val : null;

                // 数据类型转换
                if (!string.IsNullOrEmpty(mapping.DataType))
                {
                    sourceValue = ConvertDataType(sourceValue, mapping.DataType);
                }

                // 设置到目标字段
                newRow[mapping.TargetField] = sourceValue ?? DBNull.Value;
            }

            result.Add(newRow);
        }

        return result;
    }

    /// <summary>
    /// 数据类型转换
    /// </summary>
    private object? ConvertDataType(object? value, string dataType)
    {
        if (value == null) return null;

        try
        {
            return dataType.ToLower() switch
            {
                "string" => value.ToString() ?? string.Empty,
                "int" => Convert.ToInt32(value),
                "long" => Convert.ToInt64(value),
                "double" => Convert.ToDouble(value),
                "decimal" => Convert.ToDecimal(value),
                "bool" => Convert.ToBoolean(value),
                "datetime" => Convert.ToDateTime(value),
                "guid" => Guid.Parse(value.ToString() ?? string.Empty),
                _ => value
            };
        }
        catch
        {
            return value;
        }
    }

    // ========== Insert 输出 ==========

    /// <summary>
    /// 执行插入操作（使用动态对象方式，避免 DataTable 兼容性问题）
    /// 自动为缺失的主键字段生成 GUID
    /// </summary>
    private async Task<WriteResult> ExecuteInsertAsync(
        SqlSugarScope db,
        OutputNodeConfig config,
        List<Dictionary<string, object>> mappedData)
    {
        var tableName = config.TableName;
        var batchSize = config.BatchSize ?? 100;

        // 为缺少主键的数据行自动生成 GUID
        foreach (var row in mappedData)
        {
            // 检查是否缺少 Id 字段（常见的主键命名）
            if (!row.ContainsKey("Id") || row["Id"] == null || row["Id"] == DBNull.Value)
            {
                row["Id"] = Guid.NewGuid().ToString();
            }
        }

        var totalAffected = 0;
        var batches = SplitIntoBatches(mappedData, batchSize);

        foreach (var batch in batches)
        {
            // 使用动态对象方式插入（SqlSugar 对 MySQL 的 DataTable 支持有限）
            // 逐条插入或使用 ExecuteCommandAsync 执行批量 SQL
            foreach (var row in batch)
            {
                // 构建插入 SQL
                var columns = row.Keys.ToList();
                var columnList = string.Join(", ", columns);
                var valueList = string.Join(", ", columns.Select(c =>
                {
                    var value = row[c];
                    if (value == null || value == DBNull.Value)
                        return "NULL";
                    if (value is string s)
                        return $"'{s.Replace("'", "''")}'";  // SQL 转义
                    if (value is DateTime dt)
                        return $"'{dt:yyyy-MM-dd HH:mm:ss}'";
                    if (value is bool b)
                        return b ? "1" : "0";
                    return value.ToString();
                }));

                var sql = $"INSERT INTO {tableName} ({columnList}) VALUES ({valueList})";
                var affected = await db.Ado.ExecuteCommandAsync(sql);
                totalAffected += affected;
            }
        }

        return new WriteResult
        {
            ProcessedRows = mappedData.Count,
            AffectedRows = totalAffected
        };
    }

    // ========== Update 输出 ==========

    /// <summary>
    /// 执行更新操作
    /// </summary>
    private async Task<WriteResult> ExecuteUpdateAsync(
        SqlSugarScope db,
        OutputNodeConfig config,
        List<Dictionary<string, object>> mappedData)
    {
        var tableName = config.TableName;
        var keyFields = config.FieldMapping?.Where(m => m.IsKey).Select(m => m.TargetField).ToList()
            ?? new List<string>();

        if (keyFields.Count == 0)
        {
            throw new InvalidOperationException("更新操作需要指定主键字段");
        }

        var totalAffected = 0;

        foreach (var row in mappedData)
        {
            // 构建 WHERE 条件
            var whereConditions = new List<string>();
            foreach (var keyField in keyFields)
            {
                if (row.TryGetValue(keyField, out var value))
                {
                    whereConditions.Add($"{keyField} = '{value}'");
                }
            }

            if (whereConditions.Count == 0) continue;

            // 构建更新字段（排除主键）
            var updateFields = new Dictionary<string, object>();
            foreach (var (field, value) in row)
            {
                if (!keyFields.Contains(field))
                {
                    updateFields[field] = value;
                }
            }

            if (updateFields.Count == 0) continue;

            // 执行更新
            var whereClause = string.Join(" AND ", whereConditions);
            var affected = await db.Updateable(updateFields)
                .AS(tableName)
                .Where(whereClause)
                .ExecuteCommandAsync();

            totalAffected += affected;
        }

        return new WriteResult
        {
            ProcessedRows = mappedData.Count,
            AffectedRows = totalAffected
        };
    }

    // ========== Merge 输出 ==========

    /// <summary>
    /// 执行合并操作（存在则更新，不存在则插入）
    /// </summary>
    private async Task<WriteResult> ExecuteMergeAsync(
        SqlSugarScope db,
        OutputNodeConfig config,
        List<Dictionary<string, object>> mappedData)
    {
        var tableName = config.TableName;
        var keyFields = config.FieldMapping?.Where(m => m.IsKey).Select(m => m.TargetField).ToList()
            ?? new List<string>();

        if (keyFields.Count == 0)
        {
            throw new InvalidOperationException("合并操作需要指定主键字段");
        }

        var totalInserted = 0;
        var totalUpdated = 0;

        foreach (var row in mappedData)
        {
            // 检查记录是否存在
            var exists = await CheckRecordExistsAsync(db, tableName, keyFields, row);

            if (exists)
            {
                // 更新
                var updateResult = await UpdateSingleRecordAsync(db, tableName, keyFields, row);
                totalUpdated += updateResult;
            }
            else
            {
                // 插入
                var insertResult = await InsertSingleRecordAsync(db, tableName, row);
                totalInserted += insertResult;
            }
        }

        return new WriteResult
        {
            ProcessedRows = mappedData.Count,
            AffectedRows = totalInserted + totalUpdated
        };
    }

    /// <summary>
    /// 检查记录是否存在
    /// </summary>
    private async Task<bool> CheckRecordExistsAsync(
        SqlSugarScope db,
        string tableName,
        List<string> keyFields,
        Dictionary<string, object> row)
    {
        var whereConditions = new List<string>();
        foreach (var keyField in keyFields)
        {
            if (row.TryGetValue(keyField, out var value))
            {
                whereConditions.Add($"{keyField} = '{value}'");
            }
        }

        if (whereConditions.Count == 0) return false;

        var whereClause = string.Join(" AND ", whereConditions);
        var count = await db.Queryable<dynamic>()
            .AS(tableName)
            .Where(whereClause)
            .CountAsync();

        return count > 0;
    }

    /// <summary>
    /// 更新单条记录
    /// </summary>
    private async Task<int> UpdateSingleRecordAsync(
        SqlSugarScope db,
        string tableName,
        List<string> keyFields,
        Dictionary<string, object> row)
    {
        var updateFields = new Dictionary<string, object>();
        foreach (var (field, value) in row)
        {
            if (!keyFields.Contains(field))
            {
                updateFields[field] = value;
            }
        }

        if (updateFields.Count == 0) return 0;

        var whereConditions = new List<string>();
        foreach (var keyField in keyFields)
        {
            if (row.TryGetValue(keyField, out var value))
            {
                whereConditions.Add($"{keyField} = '{value}'");
            }
        }

        var whereClause = string.Join(" AND ", whereConditions);
        return await db.Updateable(updateFields)
            .AS(tableName)
            .Where(whereClause)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 插入单条记录
    /// </summary>
    private async Task<int> InsertSingleRecordAsync(
        SqlSugarScope db,
        string tableName,
        Dictionary<string, object> row)
    {
        var dataTable = DictionaryListToDataTable(new List<Dictionary<string, object>> { row }, tableName);
        return await db.Insertable(dataTable).ExecuteCommandAsync();
    }

    // ========== Truncate + Insert 输出 ==========

    /// <summary>
    /// 执行清空后插入操作
    /// </summary>
    private async Task<WriteResult> ExecuteTruncateInsertAsync(
        SqlSugarScope db,
        OutputNodeConfig config,
        List<Dictionary<string, object>> mappedData)
    {
        var tableName = config.TableName;

        // 1. 清空表（DELETE，避免 TRUNCATE 权限问题）
        var deleteAffected = await db.Deleteable<object>()
            .AS(tableName)
            .ExecuteCommandAsync();

        // 2. 执行插入
        var insertResult = await ExecuteInsertAsync(db, config, mappedData);

        return new WriteResult
        {
            ProcessedRows = mappedData.Count,
            AffectedRows = deleteAffected + insertResult.AffectedRows
        };
    }

    // ========== 辅助方法 ==========

    /// <summary>
    /// 将数据分批
    /// </summary>
    private List<List<Dictionary<string, object>>> SplitIntoBatches(
        List<Dictionary<string, object>> data,
        int batchSize)
    {
        var batches = new List<List<Dictionary<string, object>>>();

        for (var i = 0; i < data.Count; i += batchSize)
        {
            var batch = data.Skip(i).Take(batchSize).ToList();
            batches.Add(batch);
        }

        return batches;
    }

    /// <summary>
    /// 字典列表转换为 DataTable
    /// </summary>
    private System.Data.DataTable DictionaryListToDataTable(
        List<Dictionary<string, object>> data,
        string tableName)
    {
        var dataTable = new System.Data.DataTable(tableName);

        if (data.Count == 0)
        {
            return dataTable;
        }

        // 创建列
        foreach (var key in data[0].Keys)
        {
            dataTable.Columns.Add(key);
        }

        // 填充数据
        foreach (var row in data)
        {
            var dataRow = dataTable.NewRow();
            foreach (var (key, value) in row)
            {
                dataRow[key] = value ?? DBNull.Value;
            }
            dataTable.Rows.Add(dataRow);
        }

        return dataTable;
    }

    /// <summary>
    /// 写入结果内部类
    /// </summary>
    private class WriteResult
    {
        /// <summary>
        /// 处理的数据行数
        /// </summary>
        public int ProcessedRows { get; set; }

        /// <summary>
        /// 影响的数据库行数
        /// </summary>
        public int AffectedRows { get; set; }
    }
}