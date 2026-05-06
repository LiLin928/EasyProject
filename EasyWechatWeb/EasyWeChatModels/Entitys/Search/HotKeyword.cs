using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 热门关键词实体
/// </summary>
[SugarTable("HotKeyword", "热门关键词表")]
public class HotKeyword
{
    /// <summary>
    /// ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 关键词
    /// </summary>
    [SugarColumn(Length = 100, ColumnDescription = "关键词")]
    public string Keyword { get; set; } = string.Empty;

    /// <summary>
    /// 搜索次数
    /// </summary>
    [SugarColumn(ColumnDescription = "搜索次数")]
    public int SearchCount { get; set; } = 0;

    /// <summary>
    /// 排序（数字越小越靠前）
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态：1-启用，0-禁用
    /// </summary>
    [SugarColumn(ColumnDescription = "状态：1-启用，0-禁用")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 是否推荐
    /// </summary>
    [SugarColumn(ColumnDescription = "是否推荐")]
    public bool IsRecommend { get; set; } = false;

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