using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 商品分类实体类
/// </summary>
/// <remarks>
/// 用于存储商品分类信息，支持分类层级管理
/// </remarks>
[SugarTable("Category", "商品分类表")]
public class Category
{
    /// <summary>
    /// 分类ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 分类名称
    /// </summary>
    /// <remarks>
    /// 分类的显示名称，长度限制50字符
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "分类名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 分类图标
    /// </summary>
    /// <remarks>
    /// 分类图标URL地址，长度限制255字符，可为空
    /// </remarks>
    [SugarColumn(Length = 255, IsNullable = true, ColumnDescription = "分类图标")]
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>
    /// 分类显示排序，数值越小越靠前
    /// </remarks>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 父分类ID
    /// </summary>
    /// <remarks>
    /// 支持多级分类，为空表示顶级分类
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "父分类ID")]
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// 分类的详细描述信息，长度限制200字符，可为空
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 分类状态：1-正常，0-禁用。默认值为1
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态：1-正常，0-禁用")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 分类记录创建时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}