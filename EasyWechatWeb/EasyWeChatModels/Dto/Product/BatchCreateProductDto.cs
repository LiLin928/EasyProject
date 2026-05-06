namespace EasyWeChatModels.Dto;

/// <summary>
/// 批量创建商品参数
/// </summary>
public class BatchCreateProductDto
{
    /// <summary>
    /// 商品列表
    /// </summary>
    public List<AddProductDto> Products { get; set; } = new();
}