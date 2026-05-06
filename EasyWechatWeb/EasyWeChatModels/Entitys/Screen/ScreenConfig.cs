using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 大屏配置实体类
/// </summary>
/// <remarks>
/// 用于存储大屏的可视化配置信息，包括布局、样式、权限等
/// </remarks>
[SugarTable("ScreenConfig", "大屏配置表")]
public class ScreenConfig
{
    /// <summary>
    /// 大屏ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 大屏名称
    /// </summary>
    /// <remarks>
    /// 大屏的显示名称，长度限制100字符
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "大屏名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    /// <remarks>
    /// 大屏的详细描述信息，长度限制500字符，可为空
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 缩略图
    /// </summary>
    /// <remarks>
    /// 大屏预览缩略图URL地址，长度限制500字符，可为空
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "缩略图")]
    public string? Thumbnail { get; set; }

    /// <summary>
    /// 样式配置
    /// </summary>
    /// <remarks>
    /// 大屏的样式配置，JSON格式存储，默认为空对象
    /// </remarks>
    [SugarColumn(ColumnDataType = "longtext", ColumnDescription = "样式配置")]
    public string Style { get; set; } = "{}";

    /// <summary>
    /// 权限配置
    /// </summary>
    /// <remarks>
    /// 大屏的访问权限配置，JSON格式存储，可为空
    /// </remarks>
    [SugarColumn(ColumnDataType = "longtext", IsNullable = true, ColumnDescription = "权限配置")]
    public string? Permissions { get; set; }

    /// <summary>
    /// 是否公开
    /// </summary>
    /// <remarks>
    /// 是否公开访问：1-公开，0-私有。默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否公开：1-公开，0-私有")]
    public int IsPublic { get; set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 大屏状态：1-正常，0-禁用。默认值为1
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态：1-正常，0-禁用")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 创建人ID
    /// </summary>
    /// <remarks>
    /// 创建该大屏的用户ID
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建人ID")]
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 大屏记录创建时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <remarks>
    /// 大屏信息最后更新时间，可为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}