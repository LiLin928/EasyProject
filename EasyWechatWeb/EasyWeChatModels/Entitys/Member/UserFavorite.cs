using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 用户收藏实体类
/// </summary>
/// <remarks>
/// 用于存储用户收藏的商品信息
/// </remarks>
[SugarTable("UserFavorite", "用户收藏表")]
public class UserFavorite
{
    /// <summary>
    /// 收藏ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户ID")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    [SugarColumn(ColumnDescription = "商品ID")]
    public Guid ProductId { get; set; }

    /// <summary>
    /// 分组ID
    /// </summary>
    /// <remarks>
    /// 收藏所属分组，可为空表示默认分组
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "分组ID")]
    public Guid? GroupId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}