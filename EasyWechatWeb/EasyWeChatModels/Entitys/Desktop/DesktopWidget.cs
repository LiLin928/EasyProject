using SqlSugar;
using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Entitys;

[SugarTable("DesktopWidget", "桌面组件表")]
public class DesktopWidget
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(ColumnDescription = "组件名称", Length = 50)]
    public string Name { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "组件类型")]
    public WidgetType Type { get; set; } = WidgetType.Card;

    [SugarColumn(ColumnDescription = "图标名称", Length = 50, IsNullable = true)]
    public string? Icon { get; set; }

    [SugarColumn(ColumnDescription = "默认宽度(栅格数1-12)")]
    public int DefaultWidth { get; set; } = 3;

    [SugarColumn(ColumnDescription = "默认高度(像素)")]
    public int DefaultHeight { get; set; } = 120;

    [SugarColumn(ColumnDescription = "数据源类型")]
    public DataSourceType DataSourceType { get; set; } = DataSourceType.Api;

    [SugarColumn(ColumnDescription = "数据源配置(JSON)", ColumnDataType = "text", IsNullable = true)]
    public string? DataSourceConfig { get; set; }

    [SugarColumn(ColumnDescription = "交互配置(JSON)", ColumnDataType = "text", IsNullable = true)]
    public string? InteractionConfig { get; set; }

    [SugarColumn(ColumnDescription = "状态: 0禁用 1启用")]
    public int Status { get; set; } = 1;

    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    [SugarColumn(ColumnDescription = "更新时间", IsNullable = true)]
    public DateTime? UpdateTime { get; set; }
}