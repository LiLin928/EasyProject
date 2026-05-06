namespace EasyWeChatModels.Entitys;

using SqlSugar;

/// <summary>
/// CAP 消息日志实体（用于查询展示）
/// </summary>
[SugarTable("CapMessageLog", "CAP消息日志表")]
public class CapMessageLog
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 消息类型（0:发布, 1:订阅消费）
    /// </summary>
    [SugarColumn(ColumnDescription = "消息类型")]
    public int MessageType { get; set; }

    /// <summary>
    /// 主题名称
    /// </summary>
    [SugarColumn(ColumnDescription = "主题名称", Length = 200)]
    public string Topic { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容（JSON）
    /// </summary>
    [SugarColumn(ColumnDescription = "消息内容", ColumnDataType = "text", IsNullable = true)]
    public string? Content { get; set; }

    /// <summary>
    /// 消息状态（0:待处理, 1:成功, 2:失败, 3:重试中）
    /// </summary>
    [SugarColumn(ColumnDescription = "消息状态")]
    public int Status { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    [SugarColumn(ColumnDescription = "重试次数")]
    public int Retries { get; set; }

    /// <summary>
    /// 消费者组 ID
    /// </summary>
    [SugarColumn(ColumnDescription = "消费者组ID", Length = 100, IsNullable = true)]
    public string? GroupId { get; set; }

    /// <summary>
    /// 异常信息
    /// </summary>
    [SugarColumn(ColumnDescription = "异常信息", ColumnDataType = "text", IsNullable = true)]
    public string? ExceptionMessage { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 处理时间
    /// </summary>
    [SugarColumn(ColumnDescription = "处理时间", IsNullable = true)]
    public DateTime? ProcessTime { get; set; }
}