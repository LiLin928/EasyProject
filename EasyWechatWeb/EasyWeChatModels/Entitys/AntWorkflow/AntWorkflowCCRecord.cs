using SqlSugar;

namespace EasyWeChatModels.Entitys;

[SugarTable("AntWorkflowCCRecord", "Ant抄送记录表")]
public class AntWorkflowCCRecord
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(ColumnDescription = "实例ID")]
    public Guid InstanceId { get; set; }

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "节点ID（字符串）")]
    public string? NodeId { get; set; }

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "节点名称")]
    public string? NodeName { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "发送人ID")]
    public Guid? FromUserId { get; set; }

    [SugarColumn(ColumnDescription = "接收人ID")]
    public Guid ToUserId { get; set; }

    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "接收人姓名")]
    public string? ToUserName { get; set; }

    [SugarColumn(ColumnDescription = "是否已读：0否/1是")]
    public int IsRead { get; set; } = 0;

    [SugarColumn(IsNullable = true, ColumnDescription = "阅读时间")]
    public DateTime? ReadTime { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "发送时间")]
    public DateTime? SendTime { get; set; }
}