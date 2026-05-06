using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 收藏分组实体类
/// </summary>
/// <remarks>
/// 用于存储用户收藏商品的分组信息
/// </remarks>
[SugarTable("FavoriteGroup", "收藏分组表")]
public class FavoriteGroup
{
    /// <summary>
    /// 分组ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户ID")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 分组名称
    /// </summary>
    [SugarColumn(Length = 50, ColumnDescription = "分组名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}