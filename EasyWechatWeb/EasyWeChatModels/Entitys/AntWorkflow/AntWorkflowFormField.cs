using SqlSugar;

namespace EasyWeChatModels.Entitys;

[SugarTable("AntWorkflowFormField", "Ant表单字段表")]
public class AntWorkflowFormField
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(Length = 50, ColumnDescription = "业务类型编码")]
    public string BusinessType { get; set; } = string.Empty;

    [SugarColumn(Length = 50, ColumnDescription = "字段ID")]
    public string FieldId { get; set; } = string.Empty;

    [SugarColumn(Length = 50, ColumnDescription = "字段名称")]
    public string FieldName { get; set; } = string.Empty;

    [SugarColumn(Length = 100, ColumnDescription = "字段标签")]
    public string FieldLabel { get; set; } = string.Empty;

    [SugarColumn(Length = 20, ColumnDescription = "字段类型")]
    public string FieldType { get; set; } = "text";

    [SugarColumn(ColumnDescription = "是否必填")]
    public int Required { get; set; } = 0;

    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "默认值JSON")]
    public string? DefaultValue { get; set; }

    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "选项配置JSON")]
    public string? Options { get; set; }

    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "占位符")]
    public string? Placeholder { get; set; }

    [SugarColumn(ColumnDescription = "字段顺序")]
    public int Order { get; set; } = 0;

    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}