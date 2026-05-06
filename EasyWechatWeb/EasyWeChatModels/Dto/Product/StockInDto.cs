namespace EasyWeChatModels.Dto;

/// <summary>
/// 库存入库参数
/// </summary>
public class StockInDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 入库数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 批次号
    /// </summary>
    public string? BatchNo { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}