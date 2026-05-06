using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 列配置模板实体类
/// </summary>
[SugarTable("ColumnTemplate", "列配置模板表")]
public class ColumnTemplate
{
    /// <summary>
    /// 模板ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 模板名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "模板名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 模板类型 (single: 单列模板, table: 表格模板)
    /// </summary>
    [SugarColumn(Length = 20, ColumnDescription = "模板类型")]
    public string Type { get; set; } = "table";

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 关联数据源ID
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "数据源ID")]
    public Guid? DatasourceId { get; set; }

    /// <summary>
    /// SQL查询语句
    /// </summary>
    [SugarColumn(ColumnDataType = "longtext", IsNullable = true, ColumnDescription = "SQL查询")]
    public string? SqlQuery { get; set; }

    /// <summary>
    /// 列配置JSON
    /// </summary>
    [SugarColumn(ColumnDataType = "longtext", ColumnDescription = "列配置JSON")]
    public string ColumnConfigs { get; set; } = "[]";

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