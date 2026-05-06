using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 报表实体类
/// </summary>
[SugarTable("Report", "报表表")]
public class Report
{
    /// <summary>
    /// 报表ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 报表名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "报表名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 分类
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "分类")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 数据源ID
    /// </summary>
    [SugarColumn(ColumnDescription = "数据源ID")]
    public Guid DatasourceId { get; set; }

    /// <summary>
    /// SQL查询语句
    /// </summary>
    [SugarColumn(ColumnDataType = "longtext", ColumnDescription = "SQL查询")]
    public string SqlQuery { get; set; } = string.Empty;

    /// <summary>
    /// 是否显示图表
    /// </summary>
    [SugarColumn(ColumnDescription = "显示图表")]
    public bool ShowChart { get; set; } = true;

    /// <summary>
    /// 是否显示数据表格
    /// </summary>
    [SugarColumn(ColumnDescription = "显示表格")]
    public bool ShowTable { get; set; } = true;

    /// <summary>
    /// 图表类型 (bar/line/pie)
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "图表类型")]
    public string ChartType { get; set; } = "bar";

    /// <summary>
    /// X轴字段名
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "X轴字段")]
    public string? XAxisField { get; set; }

    /// <summary>
    /// Y轴字段名
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "Y轴字段")]
    public string? YAxisField { get; set; }

    /// <summary>
    /// 聚合类型 (sum/count/avg)
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "聚合类型")]
    public string Aggregation { get; set; } = "sum";

    /// <summary>
    /// 是否自动生成列配置
    /// </summary>
    [SugarColumn(ColumnDescription = "自动列配置")]
    public bool AutoColumns { get; set; } = true;

    /// <summary>
    /// 列模板ID
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "列模板ID")]
    public Guid? ColumnTemplateId { get; set; }

    /// <summary>
    /// 列配置JSON
    /// </summary>
    [SugarColumn(ColumnDataType = "longtext", IsNullable = true, ColumnDescription = "列配置JSON")]
    public string? ColumnConfigs { get; set; }

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