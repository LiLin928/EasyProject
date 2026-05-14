using SqlSugar;

namespace EasyWeChatModels.Entitys;

[SugarTable("RoleWidgetConfig", "角色组件配置表")]
public class RoleWidgetConfig
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(ColumnDescription = "角色ID")]
    public Guid RoleId { get; set; }

    [SugarColumn(ColumnDescription = "组件ID")]
    public Guid WidgetId { get; set; }

    [SugarColumn(ColumnDescription = "排序序号")]
    public int SortOrder { get; set; } = 0;

    [SugarColumn(ColumnDescription = "是否启用")]
    public bool IsEnabled { get; set; } = true;

    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}