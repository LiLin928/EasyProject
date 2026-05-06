namespace EasyWeChatModels.Dto;

/// <summary>
/// 购物车状态 DTO
/// </summary>
public class CartStateDto
{
    /// <summary>
    /// 购物车项列表
    /// </summary>
    public List<CartDto> Items { get; set; } = new();

    /// <summary>
    /// 总数量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 总金额
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// 选中数量
    /// </summary>
    public int SelectedCount { get; set; }

    /// <summary>
    /// 选中金额
    /// </summary>
    public decimal SelectedPrice { get; set; }
}