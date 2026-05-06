using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// ETL节点执行记录实体
/// </summary>
[SugarTable("EtlNodeExecution", "ETL节点执行记录表")]
public class EtlNodeExecution
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
    [SugarColumn(Length = 50, ColumnDescription = "节点ID")]
    public string NodeId { get; set; } = string.Empty;

    /// <summary>
    /// 节点名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "节点名称")]
    public string? NodeName { get; set; }

    /// <summary>
    /// 节点类型
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "节点类型")]
    public string? NodeType { get; set; }

    /// <summary>
    /// 节点配置（JSON格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "节点配置")]
    public string? NodeConfig { get; set; }

    /// <summary>
    /// 执行状态：pending, running, success, failure, skipped
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "执行状态")]
    public string Status { get; set; } = "pending";

    /// <summary>
    /// 开始时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "开始时间")]
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "结束时间")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 执行耗时（毫秒）
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "执行耗时(ms)")]
    public long? Duration { get; set; }

    /// <summary>
    /// 输入数据（JSON格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "输入数据")]
    public string? InputData { get; set; }

    /// <summary>
    /// 输出数据（JSON格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "输出数据")]
    public string? OutputData { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "错误信息")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    [SugarColumn(ColumnDescription = "重试次数")]
    public int RetryCount { get; set; } = 0;

    /// <summary>
    /// 处理的数据行数
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "处理数据行数")]
    public int? ProcessedRows { get; set; }

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