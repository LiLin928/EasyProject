namespace EasyWeChatModels.Dto;

/// <summary>
/// 添加商品参数
/// </summary>
public class AddProductDto
{
    /// <summary>
    /// SKU码
    /// </summary>
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 商品描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 销售价格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 原价
    /// </summary>
    public decimal? OriginalPrice { get; set; }

    /// <summary>
    /// 主图
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// 商品图片列表
    /// </summary>
    public List<string>? Images { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// 库存数量
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// 库存预警阈值
    /// </summary>
    public int AlertThreshold { get; set; } = 10;

    /// <summary>
    /// 是否热销
    /// </summary>
    public bool IsHot { get; set; } = false;

    /// <summary>
    /// 是否新品
    /// </summary>
    public bool IsNew { get; set; } = false;

    /// <summary>
    /// 商品详情
    /// </summary>
    public string? Detail { get; set; }
}