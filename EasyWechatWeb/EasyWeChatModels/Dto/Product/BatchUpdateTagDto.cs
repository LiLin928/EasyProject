namespace EasyWeChatModels.Dto;

/// <summary>
/// 批量更新标签参数
/// </summary>
public class BatchUpdateTagDto
{
    /// <summary>
    /// 商品ID列表
    /// </summary>
    public List<Guid> Ids { get; set; } = new();

    /// <summary>
    /// 是否热销
    /// </summary>
    public bool? IsHot { get; set; }

    /// <summary>
    /// 是否新品
    /// </summary>
    public bool? IsNew { get; set; }
}