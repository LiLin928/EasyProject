using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// ETL执行日志实体
/// </summary>
[SugarTable("EtlExecutionLog", "ETL执行日志表")]
public class EtlExecutionLog
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 执行记录ID
    /// </summary>
    [SugarColumn(ColumnDescription = "执行记录ID")]
    public Guid ExecutionId { get; set; }

    /// <summary>
    /// 节点ID
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "节点ID")]
    public string? NodeId { get; set; }

    /// <summary>
    /// 节点名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "节点名称")]
    public string? NodeName { get; set; }

    /// <summary>
    /// 日志级别：info, warn, error, debug
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "日志级别")]
    public string Level { get; set; } = "info";

    /// <summary>
    /// 日志消息
    /// </summary>
    [SugarColumn(ColumnDataType = "text", ColumnDescription = "日志消息")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 日志时间
    /// </summary>
    [SugarColumn(ColumnDescription = "日志时间")]
    public DateTime LogTime { get; set; } = DateTime.Now;
}