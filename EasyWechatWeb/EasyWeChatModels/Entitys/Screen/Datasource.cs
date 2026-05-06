using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 数据源实体类
/// </summary>
/// <remarks>
/// 用于存储外部数据库连接信息，支持多种数据库类型
/// </remarks>
[SugarTable("Datasource", "数据源表")]
public class Datasource
{
    /// <summary>
    /// 数据源ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 数据源名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "数据源名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 数据源类型
    /// </summary>
    /// <remarks>
    /// 支持: mysql, postgresql, oracle, sqlserver, clickhouse
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "数据源类型")]
    public string Type { get; set; } = "mysql";

    /// <summary>
    /// 主机地址
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "主机地址")]
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// 端口
    /// </summary>
    [SugarColumn(ColumnDescription = "端口")]
    public int Port { get; set; } = 3306;

    /// <summary>
    /// 数据库名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "数据库")]
    public string Database { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "用户名")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "密码")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 连接状态
    /// </summary>
    /// <remarks>
    /// 状态值: connected, disconnected, error
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "连接状态")]
    public string Status { get; set; } = "disconnected";

    /// <summary>
    /// 最后连接时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "最后连接时间")]
    public DateTime? LastConnectionTime { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    [SugarColumn(ColumnDescription = "创建人ID")]
    public Guid CreatorId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}