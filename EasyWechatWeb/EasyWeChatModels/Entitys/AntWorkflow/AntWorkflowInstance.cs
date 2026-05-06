using SqlSugar;

namespace EasyWeChatModels.Entitys;

[SugarTable("AntWorkflowInstance", "Ant流程实例表")]
public class AntWorkflowInstance
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(ColumnDescription = "流程定义ID")]
    public Guid WorkflowId { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "流程版本ID")]
    public Guid? WorkflowVersionId { get; set; }

    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "流程标题")]
    public string? Title { get; set; }

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "业务单据ID")]
    public string? BusinessId { get; set; }

    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "业务类型编码")]
    public string? BusinessType { get; set; }

    [SugarColumn(ColumnDataType = "longtext", IsNullable = true, ColumnDescription = "业务数据JSON")]
    public string? BusinessData { get; set; }

    [SugarColumn(ColumnDataType = "longtext", IsNullable = true, ColumnDescription = "表单数据JSON")]
    public string? FormData { get; set; }

    [SugarColumn(ColumnDescription = "状态：0待提交/1审批中/2通过/3驳回/4撤回/5终止")]
    public int Status { get; set; } = 0;

    [SugarColumn(ColumnDataType = "longtext", IsNullable = true, ColumnDescription = "实例节点配置JSON")]
    public string? FlowConfig { get; set; }

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "开始节点ID（字符串）")]
    public string? StartNodeId { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "开始时间")]
    public DateTime? StartTime { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "完成时间")]
    public DateTime? FinishTime { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "发起人ID")]
    public Guid? InitiatorId { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "父流程实例ID（子流程用）")]
    public Guid? ParentInstanceId { get; set; }

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "父流程节点ID（字符串）")]
    public string? ParentNodeId { get; set; }

    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}