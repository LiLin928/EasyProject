namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新商品参数
/// </summary>
public class UpdateProductDto : AddProductDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid Id { get; set; }
}