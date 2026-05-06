using SqlSugar;

namespace EasyWeChatModels.Entitys;

[SugarTable("AntWorkflow", "Ant流程定义表")]
public class AntWorkflow
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(Length = 100, ColumnDescription = "流程名称")]
    public string Name { get; set; } = string.Empty;

    [SugarColumn(Length = 50, ColumnDescription = "流程编码")]
    public string Code { get; set; } = string.Empty;

    [SugarColumn(Length = 50, ColumnDescription = "分类编码")]
    public string CategoryCode { get; set; } = string.Empty;

    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "流程描述")]
    public string? Description { get; set; }

    [SugarColumn(ColumnDescription = "状态：0草稿/1待审核/2已发布/3拒绝/4停用")]
    public int Status { get; set; } = 0;

    [SugarColumn(Length = 20, ColumnDescription = "当前版本号")]
    public string CurrentVersion { get; set; } = "1.0";

    [SugarColumn(ColumnDataType = "longtext", IsNullable = true, ColumnDescription = "DAG配置JSON")]
    public string? FlowConfig { get; set; }

    [SugarColumn(ColumnDescription = "创建人ID")]
    public Guid CreatorId { get; set; }

    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}