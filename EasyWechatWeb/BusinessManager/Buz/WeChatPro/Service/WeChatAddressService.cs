using BusinessManager.Buz.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 微信地址服务实现类
/// </summary>
/// <remarks>
/// 实现收货地址相关的业务逻辑，包括地址查询、新增、更新、删除等功能。
/// 继承自<see cref="BaseService{Address}"/>，使用SqlSugar进行数据库操作。
/// </remarks>
public class WeChatAddressService : BaseService<Address>, IWeChatAddressService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatAddressService> _logger { get; set; } = null!;

    /// <summary>
    /// 获取地址列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户地址列表</returns>
    /// <remarks>
    /// 只返回状态正常的地址（Status=1）。
    /// 默认地址排在首位，其他按创建时间倒序排列。
    /// </remarks>
    public async Task<List<AddressDto>> GetAddressListAsync(Guid userId)
    {
        var addresses = await _db.Queryable<Address>()
            .Where(a => a.UserId == userId && a.Status == 1)
            .OrderByDescending(a => a.IsDefault)
            .OrderByDescending(a => a.CreateTime)
            .ToListAsync();

        return addresses.Adapt<List<AddressDto>>();
    }

    /// <summary>
    /// 获取地址详情
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="id">地址ID</param>
    /// <returns>地址详情；地址不存在或已删除返回null</returns>
    public async Task<AddressDto?> GetAddressByIdAsync(Guid userId, Guid id)
    {
        var address = await GetFirstAsync(a => a.Id == id && a.UserId == userId && a.Status == 1);
        if (address == null)
        {
            return null;
        }

        return address.Adapt<AddressDto>();
    }

    /// <summary>
    /// 保存地址（新增或更新）
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="dto">地址信息</param>
    /// <returns>地址ID</returns>
    /// <exception cref="BusinessException">
    /// 更新时地址不存在抛出NotFound异常
    /// </exception>
    /// <remarks>
    /// 如果dto.Id有值则更新，否则新增。
    /// 设置默认地址时，会自动取消其他地址的默认状态。
    /// </remarks>
    public async Task<Guid> SaveAddressAsync(Guid userId, SaveAddressDto dto)
    {
        Address? existingAddress = null;

        if (dto.Id.HasValue && dto.Id.Value != Guid.Empty)
        {
            // 更新地址
            existingAddress = await GetFirstAsync(a => a.Id == dto.Id.Value && a.UserId == userId && a.Status == 1);
            if (existingAddress == null)
            {
                throw BusinessException.NotFound("地址不存在");
            }

            existingAddress.Name = dto.Name;
            existingAddress.Phone = dto.Phone;
            existingAddress.Province = dto.Province;
            existingAddress.City = dto.City;
            existingAddress.District = dto.District;
            existingAddress.Detail = dto.Detail;
            existingAddress.IsDefault = dto.IsDefault;
            existingAddress.UpdateTime = DateTime.Now;
        }

        // 处理默认地址
        if (dto.IsDefault)
        {
            await ClearDefaultAddressAsync(userId);
        }

        if (existingAddress != null)
        {
            await UpdateAsync(existingAddress);
            return existingAddress.Id;
        }
        else
        {
            // 新增地址
            var newAddress = new Address
            {
                UserId = userId,
                Name = dto.Name,
                Phone = dto.Phone,
                Province = dto.Province,
                City = dto.City,
                District = dto.District,
                Detail = dto.Detail,
                IsDefault = dto.IsDefault,
                Status = 1,
                CreateTime = DateTime.Now
            };

            return await InsertAsync(newAddress);
        }
    }

    /// <summary>
    /// 删除地址
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="id">地址ID</param>
    /// <returns>是否成功</returns>
    /// <exception cref="BusinessException">
    /// 地址不存在时抛出NotFound异常
    /// </exception>
    /// <remarks>
    /// 使用软删除，将Status设置为0。
    /// </remarks>
    public async Task<bool> DeleteAddressAsync(Guid userId, Guid id)
    {
        var address = await GetFirstAsync(a => a.Id == id && a.UserId == userId && a.Status == 1);
        if (address == null)
        {
            throw BusinessException.NotFound("地址不存在");
        }

        address.Status = 0;
        address.UpdateTime = DateTime.Now;
        await UpdateAsync(address);

        return true;
    }

    /// <summary>
    /// 清除用户的其他默认地址
    /// </summary>
    /// <param name="userId">用户ID</param>
    private async Task ClearDefaultAddressAsync(Guid userId)
    {
        await _db.Updateable<Address>()
            .Where(a => a.UserId == userId && a.IsDefault == true && a.Status == 1)
            .SetColumns(a => a.IsDefault == false)
            .SetColumns(a => a.UpdateTime == DateTime.Now)
            .ExecuteCommandAsync();
    }
}