namespace EasyWeChatModels.Dto;

/// <summary>
/// 批量更新分类参数
/// </summary>
public class BatchUpdateCategoryDto
{
    /// <summary>
    /// 商品ID列表
    /// </summary>
    public List<Guid> Ids { get; set; } = new();

    /// <summary>
    /// 目标分类ID
    /// </summary>
    public Guid CategoryId { get; set; }
}