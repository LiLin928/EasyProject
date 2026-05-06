using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 微信地址服务接口
/// </summary>
public interface IWeChatAddressService
{
    /// <summary>
    /// 获取地址列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>地址列表</returns>
    Task<List<AddressDto>> GetAddressListAsync(Guid userId);

    /// <summary>
    /// 获取地址详情
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="id">地址ID</param>
    /// <returns>地址详情</returns>
    Task<AddressDto?> GetAddressByIdAsync(Guid userId, Guid id);

    /// <summary>
    /// 保存地址（新增或更新）
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="dto">地址信息</param>
    /// <returns>地址ID</returns>
    Task<Guid> SaveAddressAsync(Guid userId, SaveAddressDto dto);

    /// <summary>
    /// 删除地址
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="id">地址ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAddressAsync(Guid userId, Guid id);
}