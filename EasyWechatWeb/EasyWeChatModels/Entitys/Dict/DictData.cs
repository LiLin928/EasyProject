using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 字典数据实体类
/// </summary>
/// <remarks>
/// 用于存储字典数据项，如订单状态下的"待付款"、"已完成"等
/// </remarks>
[SugarTable("DictData", "字典数据表")]
public class DictData
{
    /// <summary>
    /// 字典数据ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 字典类型编码
    /// </summary>
    /// <remarks>
    /// 关联字典类型表，如 order_status、product_type 等
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "字典类型编码")]
    public string TypeCode { get; set; } = string.Empty;

    /// <summary>
    /// 标签
    /// </summary>
    /// <remarks>
    /// 字典项的显示文本，如"待付款"、"已完成"
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "标签")]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// 中文标签
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "中文标签")]
    public string LabelZh { get; set; } = string.Empty;

    /// <summary>
    /// 英文标签
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "英文标签")]
    public string? LabelEn { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    /// <remarks>
    /// 字典项的值，如"pending"、"completed"
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "值")]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>
    /// 字典项的显示顺序，数值越小越靠前
    /// </remarks>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 字典数据状态：1-正常，0-禁用。默认值为1
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态：1-正常，0-禁用")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}