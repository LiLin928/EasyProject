using BusinessManager.Buz.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Npgsql;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using SqlSugar;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 数据源服务实现
/// </summary>
public class DatasourceService : BaseService<Datasource>, IDatasourceService
{
    public ILogger<DatasourceService> _logger { get; set; } = null!;

    /// <summary>
    /// 获取数据源列表（分页）
    /// </summary>
    public async Task<PageResponse<DatasourceDto>> GetPageListAsync(QueryDatasourceDto query)
    {
        // 处理别名字段：优先使用 DbType，如果没有则使用 Type
        var type = query.DbType ?? query.Type;

        var total = new RefAsync<int>();
        var list = await _db.Queryable<Datasource>()
            .WhereIF(!string.IsNullOrEmpty(query.Name), d => d.Name.Contains(query.Name!))
            .WhereIF(!string.IsNullOrEmpty(type), d => d.Type == type)
            .WhereIF(!string.IsNullOrEmpty(query.Status), d => d.Status == query.Status)
            .OrderByDescending(d => d.CreateTime)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        var dtos = list.Adapt<List<DatasourceDto>>();
        return PageResponse<DatasourceDto>.Create(dtos, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取数据源详情
    /// </summary>
    public new async Task<DatasourceDto?> GetByIdAsync(Guid id)
    {
        var entity = await base.GetByIdAsync(id);
        return entity?.Adapt<DatasourceDto>();
    }

    /// <summary>
    /// 获取所有数据源（下拉选择用）
    /// </summary>
    public async Task<List<DatasourceDto>> GetAllAsync()
    {
        var list = await _db.Queryable<Datasource>()
            .Where(d => d.Status == "connected")
            .OrderBy(d => d.Name)
            .ToListAsync();
        return list.Adapt<List<DatasourceDto>>();
    }

    /// <summary>
    /// 添加数据源
    /// </summary>
    public async Task<Guid> AddAsync(CreateDatasourceDto dto, Guid creatorId)
    {
        // 检查名称唯一性
        var exists = await GetFirstAsync(d => d.Name == dto.Name);
        if (exists != null)
        {
            throw BusinessException.BadRequest("数据源名称已存在");
        }

        var entity = dto.Adapt<Datasource>();
        entity.Id = Guid.NewGuid();
        entity.CreatorId = creatorId;
        entity.CreateTime = DateTime.Now;
        entity.Status = "disconnected";

        return await InsertAsync(entity);
    }

    /// <summary>
    /// 更新数据源
    /// </summary>
    public async Task<int> UpdateAsync(UpdateDatasourceDto dto)
    {
        var entity = await base.GetByIdAsync(dto.Id);
        if (entity == null)
        {
            throw BusinessException.NotFound("数据源不存在");
        }

        if (dto.Name != null) entity.Name = dto.Name;
        if (dto.Host != null) entity.Host = dto.Host;
        if (dto.Port.HasValue) entity.Port = dto.Port.Value;
        if (dto.Database != null) entity.Database = dto.Database;
        if (dto.Username != null) entity.Username = dto.Username;
        if (dto.Password != null) entity.Password = dto.Password;
        if (dto.Description != null) entity.Description = dto.Description;
        entity.UpdateTime = DateTime.Now;

        return await base.UpdateAsync(entity);
    }

    /// <summary>
    /// 删除数据源
    /// </summary>
    public new async Task<int> DeleteAsync(Guid id)
    {
        var entity = await base.GetByIdAsync(id);
        if (entity == null)
        {
            throw BusinessException.NotFound("数据源不存在");
        }

        // 检查是否被报表引用
        var reportCount = await _db.Queryable<Report>()
            .Where(r => r.DatasourceId == id)
            .CountAsync();
        if (reportCount > 0)
        {
            throw BusinessException.BadRequest("数据源被报表引用，无法删除");
        }

        return await base.DeleteAsync(id);
    }

    /// <summary>
    /// 测试连接
    /// </summary>
    public async Task<TestConnectionResultDto> TestConnectionAsync(Guid id)
    {
        var entity = await base.GetByIdAsync(id);
        if (entity == null)
        {
            throw BusinessException.NotFound("数据源不存在");
        }

        return await TestConnectionInternalAsync(entity, updateStatus: true);
    }

    /// <summary>
    /// 测试连接（按配置，用于未保存的数据源）
    /// </summary>
    public async Task<TestConnectionResultDto> TestConnectionByConfigAsync(CreateDatasourceDto dto)
    {
        var entity = dto.Adapt<Datasource>();
        return await TestConnectionInternalAsync(entity, updateStatus: false);
    }

    /// <summary>
    /// 内部测试连接方法
    /// </summary>
    private async Task<TestConnectionResultDto> TestConnectionInternalAsync(Datasource entity, bool updateStatus)
    {
        var result = new TestConnectionResultDto();
        var startTime = DateTime.Now;

        try
        {
            switch (entity.Type.ToLower())
            {
                case "mysql":
                    await TestMySQLConnectionAsync(entity, result);
                    break;

                case "postgresql":
                    await TestPostgreSQLConnectionAsync(entity, result);
                    break;

                case "sqlserver":
                    await TestSqlServerConnectionAsync(entity, result);
                    break;

                case "oracle":
                    await TestOracleConnectionAsync(entity, result);
                    break;

                case "clickhouse":
                    await TestClickHouseConnectionAsync(entity, result);
                    break;

                default:
                    result.Success = false;
                    result.Message = $"暂不支持 {entity.Type} 类型数据源";
                    return result;
            }

            result.ConnectionTime = (int)(DateTime.Now - startTime).TotalMilliseconds;

            if (updateStatus)
            {
                entity.Status = "connected";
                entity.LastConnectionTime = DateTime.Now;
                await base.UpdateAsync(entity);
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = $"连接失败: {ex.Message}";
            result.ConnectionTime = (int)(DateTime.Now - startTime).TotalMilliseconds;

            if (updateStatus)
            {
                entity.Status = "error";
                entity.UpdateTime = DateTime.Now;
                await base.UpdateAsync(entity);
            }

            _logger.LogError(ex, "数据源连接测试失败: {Type}, {Host}:{Port}", entity.Type, entity.Host, entity.Port);
        }

        return result;
    }

    /// <summary>
    /// 测试 MySQL 连接
    /// </summary>
    private async Task TestMySQLConnectionAsync(Datasource entity, TestConnectionResultDto result)
    {
        var connectionString = $"Server={entity.Host};Port={entity.Port};Database={entity.Database};User ID={entity.Username};Password={entity.Password};Charset=utf8mb4;";
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        result.Success = true;
        result.Message = "连接成功";
        result.ServerVersion = connection.ServerVersion;
    }

    /// <summary>
    /// 测试 PostgreSQL 连接
    /// </summary>
    private async Task TestPostgreSQLConnectionAsync(Datasource entity, TestConnectionResultDto result)
    {
        // 使用 NpgsqlConnectionStringBuilder 构建连接字符串，避免密码特殊字符问题
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = entity.Host,
            Port = entity.Port,
            Database = entity.Database,
            Username = entity.Username,
            Password = entity.Password,
            SslMode = SslMode.Disable,
            Timeout = 30,
            CommandTimeout = 60
        };

        _logger.LogInformation("PostgreSQL 连接测试: Host={Host}, Port={Port}, Database={Database}, Username={Username}",
            entity.Host, entity.Port, entity.Database, entity.Username);

        using var connection = new NpgsqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        result.Success = true;
        result.Message = "连接成功";
        result.ServerVersion = connection.PostgreSqlVersion.ToString();
    }

    /// <summary>
    /// 测试 SQL Server 连接
    /// </summary>
    private async Task TestSqlServerConnectionAsync(Datasource entity, TestConnectionResultDto result)
    {
        var connectionString = $"Server={entity.Host},{entity.Port};Database={entity.Database};User ID={entity.Username};Password={entity.Password};TrustServerCertificate=True";
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        result.Success = true;
        result.Message = "连接成功";
        result.ServerVersion = connection.ServerVersion;
    }

    /// <summary>
    /// 测试 Oracle 连接
    /// </summary>
    private async Task TestOracleConnectionAsync(Datasource entity, TestConnectionResultDto result)
    {
        // Oracle 连接格式：Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=xxx)(PORT=xxx))(CONNECT_DATA=(SERVICE_NAME=xxx)));User Id=xxx;Password=xxx
        var connectionString = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={entity.Host})(PORT={entity.Port}))(CONNECT_DATA=(SERVICE_NAME={entity.Database})));User Id={entity.Username};Password={entity.Password}";
        using var connection = new OracleConnection(connectionString);
        await connection.OpenAsync();

        result.Success = true;
        result.Message = "连接成功";
        result.ServerVersion = connection.ServerVersion;
    }

    /// <summary>
    /// 测试 ClickHouse 连接
    /// </summary>
    private async Task TestClickHouseConnectionAsync(Datasource entity, TestConnectionResultDto result)
    {
        // ClickHouse HTTP 接口端口默认 8123
        var port = entity.Port > 0 ? entity.Port : 8123;
        var url = $"http://{entity.Host}:{port}/?user={entity.Username}&password={entity.Password}&database={entity.Database}";

        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            result.Success = true;
            result.Message = "连接成功";
            result.ServerVersion = "ClickHouse";
        }
        else
        {
            result.Success = false;
            result.Message = $"连接失败: {response.StatusCode}";
        }
    }

    /// <summary>
    /// 获取支持的数据库类型
    /// </summary>
    public Task<List<DbTypeInfoDto>> GetDbTypesAsync()
    {
        var types = new List<DbTypeInfoDto>
        {
            new DbTypeInfoDto { Code = "mysql", Name = "MySQL", DefaultPort = 3306 },
            new DbTypeInfoDto { Code = "postgresql", Name = "PostgreSQL", DefaultPort = 5432 },
            new DbTypeInfoDto { Code = "oracle", Name = "Oracle", DefaultPort = 1521 },
            new DbTypeInfoDto { Code = "sqlserver", Name = "SQL Server", DefaultPort = 1433 },
            new DbTypeInfoDto { Code = "clickhouse", Name = "ClickHouse", DefaultPort = 8123 }
        };
        return Task.FromResult(types);
    }

    /// <summary>
    /// 测试查询
    /// </summary>
    public async Task<TestQueryResultDto> TestQueryAsync(Guid id, string sql)
    {
        var entity = await base.GetByIdAsync(id);
        if (entity == null)
        {
            throw BusinessException.NotFound("数据源不存在");
        }

        var result = new TestQueryResultDto();

        try
        {
            switch (entity.Type.ToLower())
            {
                case "mysql":
                    await ExecuteMySQLQueryAsync(entity, sql, result);
                    break;

                case "postgresql":
                    await ExecutePostgreSQLQueryAsync(entity, sql, result);
                    break;

                case "sqlserver":
                    await ExecuteSqlServerQueryAsync(entity, sql, result);
                    break;

                case "oracle":
                    await ExecuteOracleQueryAsync(entity, sql, result);
                    break;

                case "clickhouse":
                    await ExecuteClickHouseQueryAsync(entity, sql, result);
                    break;

                default:
                    result.Success = false;
                    result.Error = $"暂不支持 {entity.Type} 类型数据源查询";
                    return result;
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Error = $"查询失败: {ex.Message}";
            _logger.LogError(ex, "数据源查询测试失败: {Id}, SQL: {Sql}", id, sql);
        }

        return result;
    }

    /// <summary>
    /// 执行 MySQL 查询
    /// </summary>
    private async Task ExecuteMySQLQueryAsync(Datasource entity, string sql, TestQueryResultDto result)
    {
        var connectionString = $"Server={entity.Host};Port={entity.Port};Database={entity.Database};User ID={entity.Username};Password={entity.Password};Charset=utf8mb4;";
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        using var reader = await command.ExecuteReaderAsync();
        await ReadQueryResult(reader, result);
    }

    /// <summary>
    /// 执行 PostgreSQL 查询
    /// </summary>
    private async Task ExecutePostgreSQLQueryAsync(Datasource entity, string sql, TestQueryResultDto result)
    {
        var connectionString = $"Host={entity.Host};Port={entity.Port};Database={entity.Database};Username={entity.Username};Password={entity.Password}";
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        using var command = new NpgsqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();
        await ReadQueryResult(reader, result);
    }

    /// <summary>
    /// 执行 SQL Server 查询
    /// </summary>
    private async Task ExecuteSqlServerQueryAsync(Datasource entity, string sql, TestQueryResultDto result)
    {
        var connectionString = $"Server={entity.Host},{entity.Port};Database={entity.Database};User ID={entity.Username};Password={entity.Password};TrustServerCertificate=True";
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();
        await ReadQueryResult(reader, result);
    }

    /// <summary>
    /// 执行 Oracle 查询
    /// </summary>
    private async Task ExecuteOracleQueryAsync(Datasource entity, string sql, TestQueryResultDto result)
    {
        var connectionString = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={entity.Host})(PORT={entity.Port}))(CONNECT_DATA=(SERVICE_NAME={entity.Database})));User Id={entity.Username};Password={entity.Password}";
        using var connection = new OracleConnection(connectionString);
        await connection.OpenAsync();

        using var command = new OracleCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();
        await ReadQueryResult(reader, result);
    }

    /// <summary>
    /// 执行 ClickHouse 查询
    /// </summary>
    private async Task ExecuteClickHouseQueryAsync(Datasource entity, string sql, TestQueryResultDto result)
    {
        var port = entity.Port > 0 ? entity.Port : 8123;
        var url = $"http://{entity.Host}:{port}/?user={entity.Username}&password={entity.Password}&database={entity.Database}&default_format=JSON";

        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(url, new StringContent(sql));

        if (!response.IsSuccessStatusCode)
        {
            result.Success = false;
            result.Error = $"查询失败: {response.StatusCode}";
            return;
        }

        var jsonContent = await response.Content.ReadAsStringAsync();
        var clickHouseResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ClickHouseJsonResult>(jsonContent);

        result.Success = true;
        result.RowCount = clickHouseResult?.Data?.Length ?? 0;
        result.Columns = clickHouseResult?.Meta?.Select(m => m.Name).ToList() ?? new List<string>();

        // 转换数据为字典列表
        if (clickHouseResult?.Data != null && clickHouseResult.Meta != null)
        {
            var dataList = new List<object>();
            foreach (var row in clickHouseResult.Data)
            {
                var dict = new Dictionary<string, object>();
                for (int i = 0; i < clickHouseResult.Meta.Length; i++)
                {
                    dict[clickHouseResult.Meta[i].Name] = row[i];
                }
                dataList.Add(dict);
            }
            result.Data = dataList;
        }
    }

    /// <summary>
    /// ClickHouse JSON 结果结构
    /// </summary>
    private class ClickHouseJsonResult
    {
        public ClickHouseMeta[]? Meta { get; set; }
        public object[][]? Data { get; set; }
        public int Rows { get; set; }
    }

    private class ClickHouseMeta
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    /// <summary>
    /// 读取查询结果到 DTO
    /// </summary>
    private async Task ReadQueryResult(System.Data.Common.DbDataReader reader, TestQueryResultDto result)
    {
        // 获取列名
        for (int i = 0; i < reader.FieldCount; i++)
        {
            result.Columns.Add(reader.GetName(i));
        }

        // 获取数据
        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.GetValue(i) ?? "";
            }
            result.Data.Add(row);
        }

        result.Success = true;
        result.RowCount = result.Data.Count;
    }
}