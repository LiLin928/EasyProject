using SqlSugar;

namespace EasyWeChatModels.Entitys;

[SugarTable("AntWorkflowApproveRecord", "Ant审批记录表")]
public class AntWorkflowApproveRecord
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(ColumnDescription = "实例ID")]
    public Guid InstanceId { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "实例节点ID")]
    public Guid? InstanceNodeId { get; set; }

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "节点ID（字符串）")]
    public string? NodeId { get; set; }

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "节点名称")]
    public string? NodeName { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "节点类型")]
    public int? NodeType { get; set; }

    [SugarColumn(ColumnDescription = "审批人ID")]
    public Guid HandlerId { get; set; }

    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "审批人姓名")]
    public string? HandlerName { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "审批状态：1通过/2驳回/3转交/4撤回")]
    public int? ApproveStatus { get; set; }

    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "审批意见")]
    public string? ApproveDesc { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "审批时间")]
    public DateTime? ApproveTime { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "进入节点时间")]
    public DateTime? EntryTime { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "处理时长（秒）")]
    public int? Duration { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "转交给用户ID")]
    public Guid? TransferToId { get; set; }

    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "转交给用户姓名")]
    public string? TransferToName { get; set; }
}