using SqlSugar;

namespace EasyWeChatModels.Entitys;

[SugarTable("AntWorkflowVersion", "Ant流程版本表")]
public class AntWorkflowVersion
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(ColumnDescription = "流程ID")]
    public Guid WorkflowId { get; set; }

    [SugarColumn(Length = 20, ColumnDescription = "版本号")]
    public string Version { get; set; } = string.Empty;

    [SugarColumn(ColumnDataType = "longtext", IsNullable = true, ColumnDescription = "流程配置JSON")]
    public string? FlowConfig { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "发布时间")]
    public DateTime? PublishTime { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "发布人ID")]
    public Guid? PublisherId { get; set; }

    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "发布人姓名")]
    public string? PublisherName { get; set; }

    [SugarColumn(ColumnDescription = "版本状态")]
    public int Status { get; set; } = 2;

    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "版本备注")]
    public string? Remark { get; set; }
}