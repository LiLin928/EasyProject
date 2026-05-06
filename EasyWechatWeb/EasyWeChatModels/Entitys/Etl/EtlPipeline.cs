using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// ETL任务流实体
/// </summary>
[SugarTable("EtlPipeline", "ETL任务流表")]
public class EtlPipeline
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 任务流名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "任务流名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "分类")]
    public string? Category { get; set; }

    /// <summary>
    /// 标签（JSON数组）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "标签")]
    public string? Tags { get; set; }

    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "超时时间(秒)")]
    public int? Timeout { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "重试次数")]
    public int? RetryCount { get; set; }

    /// <summary>
    /// 并发数
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "并发数")]
    public int? Concurrency { get; set; }

    /// <summary>
    /// 失败策略：stop, continue, skip
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true, ColumnDescription = "失败策略")]
    public string? FailureStrategy { get; set; }

    /// <summary>
    /// DAG定义（JSON格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "DAG定义")]
    public string? DagConfig { get; set; }

    /// <summary>
    /// 状态：draft, published, archived
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "状态")]
    public string Status { get; set; } = "draft";

    /// <summary>
    /// 版本号
    /// </summary>
    [SugarColumn(ColumnDescription = "版本号")]
    public int Version { get; set; } = 1;

    /// <summary>
    /// 创建人ID
    /// </summary>
    [SugarColumn(ColumnDescription = "创建人ID")]
    public Guid CreatorId { get; set; }

    /// <summary>
    /// 创建人名称
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "创建人名称")]
    public string? CreatorName { get; set; }

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

    /// <summary>
    /// 发布时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "发布时间")]
    public DateTime? PublishTime { get; set; }
}