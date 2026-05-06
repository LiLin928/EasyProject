using SqlSugar;

namespace EasyWeChatModels.Entitys;

[SugarTable("AntWorkflowCurrentTask", "Ant当前待处理任务表")]
public class AntWorkflowCurrentTask
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(ColumnDescription = "实例ID")]
    public Guid InstanceId { get; set; }

    [SugarColumn(ColumnDescription = "实例节点ID")]
    public Guid InstanceNodeId { get; set; }

    [SugarColumn(Length = 100, ColumnDescription = "节点ID（字符串）")]
    public string NodeId { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "节点类型")]
    public int NodeType { get; set; }

    [SugarColumn(ColumnDescription = "处理人ID")]
    public Guid HandlerId { get; set; }

    [SugarColumn(ColumnDescription = "处理人类型：1用户/2角色")]
    public int HandlerType { get; set; } = 1;

    [SugarColumn(IsNullable = true, ColumnDescription = "进入时间")]
    public DateTime? EntryTime { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "截止时间")]
    public DateTime? DueTime { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "来源节点ID（字符串）")]
    public string? SourceNodeId { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "来源用户ID")]
    public Guid? SourceUserId { get; set; }

    [SugarColumn(ColumnDescription = "处理顺序")]
    public int NodeOrder { get; set; } = 1;

    [SugarColumn(ColumnDescription = "是否激活：0否/1是")]
    public int ActiveStatus { get; set; } = 1;

    [SugarColumn(ColumnDescription = "任务类型：1审批/2抄送/3服务/4通知")]
    public int TaskType { get; set; } = 1;

    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}