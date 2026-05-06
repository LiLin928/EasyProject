using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 轮播图实体类
/// </summary>
/// <remarks>
/// 用于存储首页轮播图信息，包括图片、跳转链接、排序等
/// </remarks>
[SugarTable("Banner", "轮播图表")]
public class Banner
{
    /// <summary>
    /// 轮播图ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 图片URL
    /// </summary>
    /// <remarks>
    /// 轮播图图片地址，长度限制500字符
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "图片URL")]
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// 跳转类型
    /// </summary>
    /// <remarks>
    /// 跳转类型：none-无跳转，product-商品详情，category-分类页面，page-指定页面
    /// </remarks>
    [SugarColumn(ColumnDescription = "跳转类型：none-无，product-商品，category-分类，page-页面")]
    public string LinkType { get; set; } = "none";

    /// <summary>
    /// 跳转目标
    /// </summary>
    /// <remarks>
    /// 跳转目标值，根据LinkType不同而不同：
    /// - none: 无值
    /// - product: 商品ID
    /// - category: 分类ID
    /// - page: 页面路径
    /// </remarks>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "跳转目标")]
    public string? LinkValue { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>
    /// 轮播图显示顺序，数字越小越靠前，默认为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 轮播图状态：1-启用，0-禁用。默认值为1
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态：1-启用，0-禁用")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 轮播图记录创建时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <remarks>
    /// 轮播图信息最后更新时间，可为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}