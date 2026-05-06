using CommonManager.Base;
using CommonManager.Dto;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;

namespace BusinessManager.Basic.IService;

/// <summary>
/// 用户服务接口
/// </summary>
/// <remarks>
/// 提供用户的登录、注册、查询、更新和删除等功能。
/// 支持JWT Token认证机制，包含AccessToken和RefreshToken的双Token机制。
/// </remarks>
public interface IUserService
{
    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="dto">登录请求参数，包含用户名和密码</param>
    /// <returns>
    /// 登录成功返回<see cref="LoginResultDto"/>，包含AccessToken和RefreshToken；
    /// 用户不存在或密码错误返回null
    /// </returns>
    Task<LoginResultDto?> LoginAsync(LoginDto dto);

    /// <summary>
    /// 刷新Token
    /// </summary>
    /// <param name="refreshToken">刷新令牌，在登录时获取</param>
    /// <returns>
    /// 刷新成功返回新的<see cref="LoginResultDto"/>，包含新的AccessToken和RefreshToken；
    /// Token无效或用户状态异常返回null
    /// </returns>
    Task<LoginResultDto?> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    /// <param name="pageIndex">页码索引，从1开始</param>
    /// <param name="pageSize">每页显示数量</param>
    /// <param name="keyword">搜索关键字，可选参数。用于匹配用户名或真实姓名</param>
    /// <returns>分页用户列表，包含用户基本信息和角色列表</returns>
    Task<PageResponse<UserDto>> GetPageListAsync(int pageIndex, int pageSize, string? keyword = null);

    /// <summary>
    /// 获取用户列表（分页，支持多条件查询）
    /// </summary>
    /// <param name="dto">查询参数，包含用户名、昵称、状态、排序等</param>
    /// <returns>分页用户列表，包含用户基本信息和角色列表</returns>
    Task<PageResponse<UserDto>> GetPageListAsync(QueryUserDto dto);

    /// <summary>
    /// 获取用户详情
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户详细信息，包含角色列表；用户不存在返回null</returns>
    Task<UserDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="dto">添加用户参数，包含用户名、密码、真实姓名、角色ID列表等</param>
    /// <returns>新创建用户的ID</returns>
    Task<Guid> AddAsync(AddUserDto dto);

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="dto">更新用户参数，包含用户ID和需要更新的字段</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateAsync(UpdateUserDto dto);

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">要删除的用户ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 批量删除用户
    /// </summary>
    /// <param name="ids">要删除的用户ID列表</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteBatchAsync(List<Guid> ids);

    /// <summary>
    /// 验证用户是否有效
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户存在且状态为有效返回true，否则返回false</returns>
    Task<bool> ValidateUserAsync(Guid userId);

    /// <summary>
    /// 更新用户状态
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="status">状态值：1-启用，0-禁用</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateStatusAsync(Guid id, int status);

    /// <summary>
    /// 重置用户密码
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="newPassword">新密码（明文，将自动加密）</param>
    /// <returns>影响的行数</returns>
    Task<int> ResetPasswordAsync(Guid id, string newPassword);
}