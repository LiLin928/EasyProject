using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 供应商实体类
/// </summary>
/// <remarks>
/// 用于存储供应商信息，包括名称、联系方式、地址等
/// </remarks>
[SugarTable("Supplier", "供应商表")]
public class Supplier
{
    /// <summary>
    /// 供应商ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 供应商名称
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "供应商名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 三证合一码
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "三证合一码")]
    public string? UnifiedCode { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "联系人")]
    public string? Contact { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true, ColumnDescription = "联系电话")]
    public string? Phone { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "地址")]
    public string? Address { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    public string? Remark { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 1-启用，0-禁用
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态：1-启用，0-禁用")]
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