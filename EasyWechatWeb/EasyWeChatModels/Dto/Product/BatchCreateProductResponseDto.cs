namespace EasyWeChatModels.Dto;

/// <summary>
/// 批量创建商品响应
/// </summary>
public class BatchCreateProductResponseDto
{
    /// <summary>
    /// 创建数量
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 新创建的商品ID列表
    /// </summary>
    public List<Guid> Ids { get; set; } = new();
}