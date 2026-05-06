using SqlSugar;

namespace EasyWeChatModels.Options;

/// <summary>
/// 数据库连接配置选项，用于配置 SqlSugar 数据库连接
/// </summary>
public class DbConnectionOptions
{
    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 数据库类型（MySql=0, SqlServer=1, Sqlite=2, Oracle=3, PostgreSQL=4）
    /// </summary>
    public DbType DbType { get; set; } = DbType.MySql;

    /// <summary>
    /// 是否自动关闭连接
    /// </summary>
    public bool IsAutoCloseConnection { get; set; } = true;

    /// <summary>
    /// 主键类型（Attribute=1, AutoIncrement=2, Guid=3）
    /// </summary>
    public InitKeyType InitKeyType { get; set; } = InitKeyType.Attribute;

    /// <summary>
    /// 是否是从库（只读副本）
    /// </summary>
    public bool IsSlave { get; set; } = false;
}