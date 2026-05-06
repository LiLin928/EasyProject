namespace EasyWeChatModels.Dto;

/// <summary>
/// 商品SKU查询参数
/// </summary>
public class QueryProductSkuDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}