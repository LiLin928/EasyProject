using SqlSugar;

namespace EasyWeChatModels.Entitys;

[SugarTable("AntWorkflowInstanceNode", "Ant流程实例节点表")]
public class AntWorkflowInstanceNode
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(ColumnDescription = "实例ID")]
    public Guid InstanceId { get; set; }

    [SugarColumn(Length = 100, ColumnDescription = "节点ID（字符串，如node_start）")]
    public string NodeId { get; set; } = string.Empty;

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "节点名称")]
    public string? NodeName { get; set; }

    [SugarColumn(ColumnDescription = "节点类型")]
    public int NodeType { get; set; }

    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "节点配置JSON")]
    public string? NodeConfig { get; set; }

    [SugarColumn(ColumnDescription = "审批状态：0待处理/1处理中/2已完成/3已跳过")]
    public int ApproveStatus { get; set; } = 0;

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "父节点ID（字符串）")]
    public string? ParentNodeId { get; set; }

    [SugarColumn(ColumnDescription = "分支索引")]
    public int BranchIndex { get; set; } = 0;

    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "审批意见")]
    public string? ApproveDesc { get; set; }

    [SugarColumn(ColumnDescription = "审批顺序（依次审批时使用）")]
    public int? ApproveOrder { get; set; }
}