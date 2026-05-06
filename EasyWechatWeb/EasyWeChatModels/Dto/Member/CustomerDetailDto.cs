namespace EasyWeChatModels.Dto;

/// <summary>
/// 客户详情DTO
/// </summary>
public class CustomerDetailDto : CustomerDto
{
    /// <summary>
    /// 地址数量
    /// </summary>
    public int AddressCount { get; set; }

    /// <summary>
    /// 购物车数量
    /// </summary>
    public int CartCount { get; set; }

    /// <summary>
    /// 收藏数量
    /// </summary>
    public int FavoriteCount { get; set; }

    /// <summary>
    /// 订单数量
    /// </summary>
    public int OrderCount { get; set; }
}