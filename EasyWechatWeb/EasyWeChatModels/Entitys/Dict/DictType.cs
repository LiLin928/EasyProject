using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 字典类型实体类
/// </summary>
/// <remarks>
/// 用于存储字典类型信息，如订单状态、商品类型等
/// </remarks>
[SugarTable("DictType", "字典类型表")]
public class DictType
{
    /// <summary>
    /// 字典类型ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 字典编码
    /// </summary>
    /// <remarks>
    /// 字典的唯一标识编码，如 order_status、product_type 等
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "字典编码")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 字典名称
    /// </summary>
    /// <remarks>
    /// 字典的显示名称，如"订单状态"、"商品类型"等
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "字典名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// 字典的详细描述信息
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 字典类型状态：1-正常，0-禁用。默认值为1
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态：1-正常，0-禁用")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 版本号
    /// </summary>
    /// <remarks>
    /// 每次字典数据变更时递增，初始值为1
    /// </remarks>
    [SugarColumn(ColumnDescription = "版本号")]
    public int Version { get; set; } = 1;

    /// <summary>
    /// 中文名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "中文名称")]
    public string LabelZh { get; set; } = string.Empty;

    /// <summary>
    /// 英文名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "英文名称")]
    public string? LabelEn { get; set; }

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