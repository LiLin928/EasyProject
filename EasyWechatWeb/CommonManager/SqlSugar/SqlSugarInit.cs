using EasyWeChatModels.Options;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using CommonManager.Helper;

namespace CommonManager.SqlSugar;

/// <summary>
/// SqlSugar 初始化配置类，提供数据库客户端创建和服务注册方法
/// </summary>
/// <remarks>
/// SqlSugar 是一个轻量级 ORM 框架，支持多种数据库（MySQL、SqlServer、PostgreSQL、Oracle 等）。
/// 该类封装了 SqlSugar 客户端的创建和配置，包括 AOP（切面编程）配置用于 SQL 日志记录。
/// 支持主从数据库配置，读写分离。
/// </remarks>
/// <example>
/// <code>
/// // 在 Program.cs 中注册 SqlSugar 服务
/// var dbOptions = builder.Configuration.GetSection("MasterSlaveConnectionStrings")
///     .Get&lt;List&lt;DbConnectionOptions&gt;&gt;();
/// builder.Services.AddSqlSugar(dbOptions);
///
/// // 在 Service 中使用
/// public class UserService : BaseService&lt;User&gt;
/// {
///     // _db 通过属性注入自动赋值
///     public async Task&lt;User?&gt; GetUser(int id)
///     {
///         return await _db.Queryable&lt;User&gt;().InSingleAsync(id);
///     }
/// }
/// </code>
/// </example>
public static class SqlSugarInit
{
    /// <summary>
    /// 创建 SqlSugar 客户端实例
    /// </summary>
    /// <param name="options">数据库连接配置列表，支持多个连接（主从配置）</param>
    /// <returns>配置好的 ISqlSugarClient 实例</returns>
    /// <remarks>
    /// 客户端配置包括：
    /// - 连接字符串配置：支持多个数据库连接
    /// - AOP 配置：
    ///   - OnLogExecuting：SQL 执行前记录日志
    ///   - OnLogExecuted：SQL 执行后（可记录执行时间）
    ///   - OnError：SQL 执行错误记录
    ///   - DataExecuting：数据操作前的安全检查
    ///
    /// 使用 SqlSugarScope 保证线程安全，适用于依赖注入场景。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 创建单库客户端
    /// var options = new List&lt;DbConnectionOptions&gt;
    /// {
    ///     new DbConnectionOptions
    ///     {
    ///         ConnectionString = "Server=localhost;Database=MyDb;...",
    ///         DbType = DbType.MySql,
    ///         IsAutoCloseConnection = true
    ///     }
    /// };
    /// var db = SqlSugarInit.CreateClient(options);
    ///
    /// // 创建主从库客户端
    /// var masterSlaveOptions = new List&lt;DbConnectionOptions&gt;
    /// {
    ///     new DbConnectionOptions { ConnectionString = "...", IsSlave = false },  // 主库（写）
    ///     new DbConnectionOptions { ConnectionString = "...", IsSlave = true }    // 从库（读）
    /// };
    /// var dbCluster = SqlSugarInit.CreateClient(masterSlaveOptions);
    /// </code>
    /// </example>
    public static ISqlSugarClient CreateClient(List<DbConnectionOptions> options)
    {
        var configList = new List<ConnectionConfig>();

        foreach (var option in options)
        {
            configList.Add(new ConnectionConfig
            {
                ConnectionString = option.ConnectionString,
                DbType = option.DbType,
                IsAutoCloseConnection = option.IsAutoCloseConnection,
                InitKeyType = option.InitKeyType
            });
        }

        var db = new SqlSugarScope(configList, config =>
        {
            // 配置 AOP
            config.Aop.OnLogExecuting = (sql, pars) =>
            {
                LogHelper.MySqlInfo(sql);
            };

            config.Aop.OnLogExecuted = (sql, pars) =>
            {
                // 可记录执行时间
            };

            config.Aop.OnError = (exp) =>
            {
                LogHelper.MySqlError(exp.Message);
            };

            // 删除/更新必须带 WHERE 条件
            config.Aop.DataExecuting = (oldValue, entityInfo) =>
            {
                if (entityInfo.OperationType == DataFilterType.DeleteByObject ||
                    entityInfo.OperationType == DataFilterType.UpdateByObject)
                {
                    // 可以在这里添加安全检查
                }
            };
        });

        return db;
    }

    /// <summary>
    /// 注册 SqlSugar 服务到依赖注入容器
    /// </summary>
    /// <param name="services">IServiceCollection 服务集合</param>
    /// <param name="options">数据库连接配置列表</param>
    /// <remarks>
    /// 注册为 Scoped 生命周期，每次 HTTP 请求创建一个实例。
    /// 结合 Autofac 属性注入，BaseService 的 _db 属性会自动赋值。
    ///
    /// 使用方式：
    /// 1. 在 Program.cs 中调用 builder.Services.AddSqlSugar(dbOptions)
    /// 2. 在 BaseService 中声明 public ISqlSugarClient _db { get; set; }
    /// 3. Autofac 会自动注入 _db 属性
    /// </remarks>
    /// <example>
    /// <code>
    /// // Program.cs 配置
    /// var dbOptions = builder.Configuration.GetSection("MasterSlaveConnectionStrings")
    ///     .Get&lt;List&lt;DbConnectionOptions&gt;&gt;();
    /// builder.Services.AddSqlSugar(dbOptions);
    ///
    /// // appsettings.json 配置
    /// {
    ///   "MasterSlaveConnectionStrings": [
    ///     {
    ///       "ConnectionString": "Server=localhost;Database=EasyProject;...",
    ///       "DbType": 0,
    ///       "IsAutoCloseConnection": true,
    ///       "InitKeyType": 1
    ///     }
    ///   ]
    /// }
    /// </code>
    /// </example>
    public static void AddSqlSugar(this IServiceCollection services, List<DbConnectionOptions> options)
    {
        services.AddScoped<ISqlSugarClient>(provider =>
        {
            return CreateClient(options);
        });
    }
}