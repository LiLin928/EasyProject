using SqlSugar;

namespace EasyWeChatModels.Entitys;

[SugarTable("AntWorkflowServiceLog", "Ant服务任务执行日志表")]
public class AntWorkflowServiceLog
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(ColumnDescription = "实例ID")]
    public Guid InstanceId { get; set; }

    [SugarColumn(Length = 100, ColumnDescription = "节点ID（字符串）")]
    public string NodeId { get; set; } = string.Empty;

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "节点名称")]
    public string? NodeName { get; set; }

    [SugarColumn(Length = 20, ColumnDescription = "任务类型")]
    public string TaskType { get; set; } = "api";

    [SugarColumn(ColumnDescription = "执行状态")]
    public int ExecuteStatus { get; set; } = 0;

    [SugarColumn(IsNullable = true, ColumnDescription = "执行时间")]
    public DateTime? ExecuteTime { get; set; }

    [SugarColumn(IsNullable = true, ColumnDescription = "执行人ID")]
    public Guid? ExecutorId { get; set; }

    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "执行人姓名")]
    public string? ExecutorName { get; set; }

    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "请求数据")]
    public string? RequestData { get; set; }

    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "响应数据")]
    public string? ResponseData { get; set; }

    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "错误信息")]
    public string? ErrorMessage { get; set; }

    [SugarColumn(ColumnDescription = "重试次数")]
    public int RetryCount { get; set; } = 0;
}