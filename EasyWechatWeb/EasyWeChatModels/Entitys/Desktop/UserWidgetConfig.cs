using SqlSugar;

namespace EasyWeChatModels.Entitys;

[SugarTable("UserWidgetConfig", "用户组件配置表")]
public class UserWidgetConfig
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(ColumnDescription = "用户ID")]
    public Guid UserId { get; set; }

    [SugarColumn(ColumnDescription = "组件ID")]
    public Guid WidgetId { get; set; }

    [SugarColumn(ColumnDescription = "用户自定义宽度(栅格数)")]
    public int Width { get; set; } = 3;

    [SugarColumn(ColumnDescription = "是否启用")]
    public bool IsEnabled { get; set; } = true;

    [SugarColumn(ColumnDescription = "排序序号")]
    public int SortOrder { get; set; } = 0;

    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    [SugarColumn(ColumnDescription = "更新时间", IsNullable = true)]
    public DateTime? UpdateTime { get; set; }
}