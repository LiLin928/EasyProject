using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 微信购物车服务接口
/// </summary>
public interface IWeChatCartService
{
    /// <summary>
    /// 获取购物车列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>购物车状态</returns>
    Task<CartStateDto> GetCartListAsync(Guid userId);

    /// <summary>
    /// 添加商品到购物车
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="dto">添加信息</param>
    /// <returns>购物车项ID</returns>
    Task<Guid> AddToCartAsync(Guid userId, AddCartDto dto);

    /// <summary>
    /// 更新购物车项
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="cartId">购物车项ID</param>
    /// <param name="dto">更新信息</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateCartItemAsync(Guid userId, Guid cartId, UpdateCartDto dto);

    /// <summary>
    /// 删除购物车项
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="cartId">购物车项ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteCartItemAsync(Guid userId, Guid cartId);

    /// <summary>
    /// 清空购物车
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>是否成功</returns>
    Task<bool> ClearCartAsync(Guid userId);
}