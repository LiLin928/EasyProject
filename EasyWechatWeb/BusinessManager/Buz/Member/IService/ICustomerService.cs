using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 客户服务接口
/// </summary>
/// <remarks>
/// 提供客户管理、积分调整、等级调整等功能
/// </remarks>
public interface ICustomerService
{
    /// <summary>
    /// 获取客户分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页客户列表</returns>
    Task<PageResponse<CustomerDto>> GetPageListAsync(QueryCustomerDto query);

    /// <summary>
    /// 根据ID获取客户详情
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <returns>客户详情</returns>
    Task<CustomerDetailDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 添加客户
    /// </summary>
    /// <param name="dto">添加参数</param>
    /// <returns>新创建的客户ID</returns>
    Task<Guid> AddAsync(AddCustomerDto dto);

    /// <summary>
    /// 更新客户
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateAsync(UpdateCustomerDto dto);

    /// <summary>
    /// 删除客户
    /// </summary>
    /// <param name="ids">客户ID列表</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteAsync(List<Guid> ids);

    /// <summary>
    /// 调整客户积分
    /// </summary>
    /// <param name="dto">调整参数</param>
    /// <returns>影响的行数</returns>
    Task<int> AdjustPointsAsync(AdjustPointsDto dto);

    /// <summary>
    /// 调整客户等级
    /// </summary>
    /// <param name="dto">调整参数</param>
    /// <returns>影响的行数</returns>
    Task<int> AdjustLevelAsync(AdjustLevelDto dto);

    /// <summary>
    /// 获取客户地址列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>地址列表</returns>
    Task<List<AddressDto>> GetAddressesAsync(Guid userId);

    /// <summary>
    /// 获取客户购物车列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>购物车列表</returns>
    Task<List<CartDto>> GetCartsAsync(Guid userId);

    /// <summary>
    /// 获取客户收藏列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="groupId">分组ID（可选）</param>
    /// <returns>收藏列表</returns>
    Task<List<UserFavoriteDto>> GetFavoritesAsync(Guid userId, Guid? groupId = null);

    /// <summary>
    /// 获取客户收藏分组列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>分组列表</returns>
    Task<List<FavoriteGroupDto>> GetFavoriteGroupsAsync(Guid userId);

    /// <summary>
    /// 更新客户状态
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <param name="status">状态</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateStatusAsync(Guid id, int status);
}

/// <summary>
/// 会员等级服务接口
/// </summary>
/// <remarks>
/// 提供会员等级管理功能
/// </remarks>
public interface IMemberLevelService
{
    /// <summary>
    /// 获取所有会员等级列表
    /// </summary>
    /// <returns>会员等级列表</returns>
    Task<List<MemberLevelDto>> GetAllAsync();

    /// <summary>
    /// 根据ID获取会员等级详情
    /// </summary>
    /// <param name="id">等级ID</param>
    /// <returns>会员等级详情</returns>
    Task<MemberLevelDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 添加会员等级
    /// </summary>
    /// <param name="dto">添加参数</param>
    /// <returns>新创建的等级ID</returns>
    Task<Guid> AddAsync(AddMemberLevelDto dto);

    /// <summary>
    /// 更新会员等级
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateAsync(UpdateMemberLevelDto dto);

    /// <summary>
    /// 删除会员等级
    /// </summary>
    /// <param name="id">等级ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 批量删除会员等级
    /// </summary>
    /// <param name="ids">等级ID列表</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteBatchAsync(List<Guid> ids);
}

/// <summary>
/// 积分服务接口
/// </summary>
/// <remarks>
/// 提供积分记录查询功能
/// </remarks>
public interface IPointsService
{
    /// <summary>
    /// 获取积分记录分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页积分记录列表</returns>
    Task<PageResponse<PointsRecordDto>> GetPageListAsync(QueryPointsRecordDto query);
}