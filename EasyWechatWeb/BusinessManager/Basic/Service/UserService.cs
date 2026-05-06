using BusinessManager.Basic.IService;
using CommonManager.Base;
using CommonManager.Dto;
using CommonManager.Error;
using CommonManager.Helper;
using EasyWeChatModels.Options;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlSugar;

namespace BusinessManager.Basic.Service;

/// <summary>
/// 用户服务实现类
/// </summary>
/// <remarks>
/// 实现用户相关的业务逻辑，包括登录认证、用户管理等功能。
/// 继承自<see cref="BaseService{User}"/>，使用SqlSugar进行数据库操作。
/// 使用JWT双Token机制实现认证。
/// </remarks>
public class UserService : BaseService<User>, IUserService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<UserService> _logger { get; set; } = null!;
    /// <summary>
    /// JWT Token 配置选项
    /// </summary>
    public IOptions<JWTTokenOptions> _jwtOptions { get; set; } = null!;

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="dto">登录请求参数，包含用户名和密码</param>
    /// <returns>
    /// 登录成功返回<see cref="LoginResultDto"/>，包含AccessToken和RefreshToken；
    /// 用户不存在或密码错误返回null
    /// </returns>
    /// <remarks>
    /// 登录流程：
    /// <list type="number">
    ///     <item>根据用户名查询状态有效的用户</item>
    ///     <item>使用MD5加密验证密码</item>
    ///     <item>生成双Token（AccessToken用于接口访问，RefreshToken用于刷新Token）</item>
    ///     <item>更新用户最后登录时间</item>
    /// </list>
    /// </remarks>
    public async Task<LoginResultDto?> LoginAsync(LoginDto dto)
    {
        var user = await GetFirstAsync(u => u.UserName == dto.UserName && u.Status == 1);
        if (user == null)
        {
            return null;
        }

        // 验证密码（MD5加密）
        var encryptedPassword = SecurityHelper.Md5(dto.Password);
        if (user.Password != encryptedPassword)
        {
            return null;
        }

        // 查询用户角色
        var roles = await _db.Queryable<UserRole, Role>((ur, r) => new JoinQueryInfos(JoinType.Left, ur.RoleId == r.Id))
            .Where((ur, r) => ur.UserId == user.Id)
            .Select((ur, r) => r.RoleCode)
            .ToListAsync();

        // 生成 Token（双 Token 机制），包含角色信息
        var result = CustomerJWTHelper.GetLoginToken(user.Id.ToString(), user.UserName, user.RealName ?? "", _jwtOptions.Value, roles);

        // 更新最后登录时间
        user.LastLoginTime = DateTime.Now;
        await UpdateAsync(user);

        return result;
    }

    /// <summary>
    /// 刷新Token
    /// </summary>
    /// <param name="refreshToken">刷新令牌，在登录时获取</param>
    /// <returns>
    /// 刷新成功返回新的<see cref="LoginResultDto"/>；
    /// Token无效、类型错误或用户状态异常返回null
    /// </returns>
    /// <remarks>
    /// 刷新流程：
    /// <list type="number">
    ///     <item>验证RefreshToken的有效性和签名</item>
    ///     <item>检查Token类型是否为Refresh</item>
    ///     <item>从Token中提取用户信息</item>
    ///     <item>验证用户状态是否仍然有效</item>
    ///     <item>生成新的双Token并返回</item>
    /// </list>
    /// </remarks>
    public async Task<LoginResultDto?> RefreshTokenAsync(string refreshToken)
    {
        // 先验证 RefreshToken 并获取用户信息
        var principal = CustomerJWTHelper.ValidateToken(refreshToken, _jwtOptions.Value);
        if (principal == null)
        {
            return null;
        }

        var tokenType = principal.FindFirst("TokenType")?.Value;
        if (tokenType != "Refresh")
        {
            return null;
        }

        var userId = CustomerJWTHelper.GetUserId(principal);
        var userName = CustomerJWTHelper.GetUserName(principal);

        // 验证用户是否有效
        var userIdGuid = Guid.Parse(userId);
        var user = await GetByIdAsync(userIdGuid);
        if (user == null || user.Status != 1)
        {
            return null;
        }

        // 查询用户角色
        var roles = await _db.Queryable<UserRole, Role>((ur, r) => new JoinQueryInfos(JoinType.Left, ur.RoleId == r.Id))
            .Where((ur, r) => ur.UserId == user.Id)
            .Select((ur, r) => r.RoleCode)
            .ToListAsync();

        // 生成新的 Token，包含角色信息
        return CustomerJWTHelper.GetLoginToken(userId, userName, user.RealName ?? "", _jwtOptions.Value, roles);
    }

    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    /// <param name="pageIndex">页码索引，从1开始</param>
    /// <param name="pageSize">每页显示数量</param>
    /// <param name="keyword">搜索关键字，可选。用于匹配用户名或真实姓名</param>
    /// <returns>分页用户列表，包含用户信息和角色列表</returns>
    /// <remarks>
    /// 查询结果按创建时间倒序排列。
    /// 每个用户记录包含其关联的角色名称列表和角色ID列表，通过UserRole和Role表关联查询获取。
    /// </remarks>
    public async Task<PageResponse<UserDto>> GetPageListAsync(int pageIndex, int pageSize, string? keyword = null)
    {
        var total = new RefAsync<int>();
        var list = await _db.Queryable<User>()
            .WhereIF(!string.IsNullOrEmpty(keyword), u => u.UserName.Contains(keyword!) || (u.RealName != null && u.RealName.Contains(keyword!)))
            .OrderByDescending(u => u.CreateTime)
            .ToPageListAsync(pageIndex, pageSize, total);

        var userDtos = list.Adapt<List<UserDto>>();

        // 获取用户角色
        foreach (var dto in userDtos)
        {
            var roleData = await _db.Queryable<UserRole, Role>((ur, r) => new JoinQueryInfos(JoinType.Left, ur.RoleId == r.Id))
                .Where((ur, r) => ur.UserId == dto.Id)
                .Select((ur, r) => new { RoleName = r.RoleName, RoleId = r.Id })
                .ToListAsync();
            dto.Roles = roleData.Select(x => x.RoleName).ToList();
            dto.RoleIds = roleData.Select(x => x.RoleId).ToList();
        }

        return PageResponse<UserDto>.Create(userDtos, total.Value, pageIndex, pageSize);
    }

    /// <summary>
    /// 获取用户列表（分页，支持多条件查询和排序）
    /// </summary>
    /// <param name="dto">查询参数，包含用户名、昵称、状态、排序等</param>
    /// <returns>分页用户列表，包含用户信息和角色列表</returns>
    /// <remarks>
    /// 支持按用户名、昵称模糊查询，按状态精确查询。
    /// 支持按指定字段排序，默认按创建时间倒序。
    /// </remarks>
    public async Task<PageResponse<UserDto>> GetPageListAsync(QueryUserDto dto)
    {
        var total = new RefAsync<int>();
        var query = _db.Queryable<User>()
            .WhereIF(!string.IsNullOrEmpty(dto.UserName), u => u.UserName.Contains(dto.UserName!))
            .WhereIF(!string.IsNullOrEmpty(dto.RealName), u => u.RealName != null && u.RealName.Contains(dto.RealName!))
            .WhereIF(dto.Status.HasValue, u => u.Status == dto.Status!.Value);

        // 处理排序
        if (!string.IsNullOrEmpty(dto.SortField) && !string.IsNullOrEmpty(dto.SortOrder))
        {
            var isAsc = dto.SortOrder == "ascending";
            query = dto.SortField switch
            {
                "userName" => isAsc ? query.OrderBy(u => u.UserName) : query.OrderByDescending(u => u.UserName),
                "realName" => isAsc ? query.OrderBy(u => u.RealName) : query.OrderByDescending(u => u.RealName),
                "email" => isAsc ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
                "phone" => isAsc ? query.OrderBy(u => u.Phone) : query.OrderByDescending(u => u.Phone),
                "createTime" => isAsc ? query.OrderBy(u => u.CreateTime) : query.OrderByDescending(u => u.CreateTime),
                _ => query.OrderByDescending(u => u.CreateTime)
            };
        }
        else
        {
            query = query.OrderByDescending(u => u.CreateTime);
        }

        var list = await query.ToPageListAsync(dto.PageIndex, dto.PageSize, total);

        var userDtos = list.Adapt<List<UserDto>>();

        // 获取用户角色
        foreach (var userDto in userDtos)
        {
            var roleData = await _db.Queryable<UserRole, Role>((ur, r) => new JoinQueryInfos(JoinType.Left, ur.RoleId == r.Id))
                .Where((ur, r) => ur.UserId == userDto.Id)
                .Select((ur, r) => new { RoleName = r.RoleName, RoleId = r.Id })
                .ToListAsync();
            userDto.Roles = roleData.Select(x => x.RoleName).ToList();
            userDto.RoleIds = roleData.Select(x => x.RoleId).ToList();
        }

        return PageResponse<UserDto>.Create(userDtos, total.Value, dto.PageIndex, dto.PageSize);
    }

    /// <summary>
    /// 获取用户详情
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户详细信息DTO，包含角色列表；用户不存在返回null</returns>
    /// <remarks>
    /// 返回用户的完整信息，包括基本信息和关联的角色名称列表、角色ID列表。
    /// 使用Mapster进行实体到DTO的映射。
    /// </remarks>
    public new async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await base.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        var dto = user.Adapt<UserDto>();

        var roleData = await _db.Queryable<UserRole, Role>((ur, r) => new JoinQueryInfos(JoinType.Left, ur.RoleId == r.Id))
            .Where((ur, r) => ur.UserId == id)
            .Select((ur, r) => new { RoleName = r.RoleName, RoleId = r.Id })
            .ToListAsync();
        dto.Roles = roleData.Select(x => x.RoleName).ToList();
        dto.RoleIds = roleData.Select(x => x.RoleId).ToList();

        return dto;
    }

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="dto">添加用户参数DTO</param>
    /// <returns>新创建用户的ID</returns>
    /// <exception cref="BusinessException">
    /// 用户名已存在时抛出BadRequest异常
    /// </exception>
    /// <remarks>
    /// 添加流程：
    /// <list type="number">
    ///     <item>检查用户名唯一性</item>
    ///     <item>密码MD5加密</item>
    ///     <item>设置创建时间和默认状态</item>
    ///     <item>插入用户记录并获取ID</item>
    ///     <item>如果指定了角色列表，创建用户角色关联记录</item>
    /// </list>
    /// </remarks>
    public async Task<Guid> AddAsync(AddUserDto dto)
    {
        // 检查用户名是否已存在
        var exists = await GetFirstAsync(u => u.UserName == dto.UserName);
        if (exists != null)
        {
            throw BusinessException.BadRequest("用户名已存在");
        }

        var user = dto.Adapt<User>();
        user.Password = SecurityHelper.Md5(dto.Password);
        user.CreateTime = DateTime.Now;
        user.Status = 1;

        var id = await InsertAsync(user);

        // 分配角色
        if (dto.RoleIds != null && dto.RoleIds.Count > 0)
        {
            var userRoles = dto.RoleIds.Select(roleId => new UserRole
            {
                UserId = id,
                RoleId = roleId,
                CreateTime = DateTime.Now
            }).ToList();

            await _db.Insertable(userRoles).ExecuteCommandAsync();
        }

        return id;
    }

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="dto">更新用户参数DTO</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="BusinessException">
    /// 用户不存在时抛出NotFound异常
    /// </exception>
    /// <remarks>
    /// 支持部分更新，只更新DTO中非空的字段。
    /// 自动更新UpdateTime为当前时间。
    /// 不支持通过此方法更新用户名和密码。
    /// 如果指定了角色列表，会更新用户角色关联。
    /// </remarks>
    public async Task<int> UpdateAsync(UpdateUserDto dto)
    {
        var user = await base.GetByIdAsync(dto.Id);
        if (user == null)
        {
            throw BusinessException.NotFound("用户不存在");
        }

        if (dto.RealName != null) user.RealName = dto.RealName;
        if (dto.Phone != null) user.Phone = dto.Phone;
        if (dto.Email != null) user.Email = dto.Email;
        if (dto.Status.HasValue) user.Status = dto.Status.Value;
        user.UpdateTime = DateTime.Now;

        var result = await base.UpdateAsync(user);

        // 更新角色关联
        if (dto.RoleIds != null)
        {
            // 删除原有角色关联
            await _db.Deleteable<UserRole>().Where(ur => ur.UserId == dto.Id).ExecuteCommandAsync();

            // 添加新角色关联
            if (dto.RoleIds.Count > 0)
            {
                var userRoles = dto.RoleIds.Select(roleId => new UserRole
                {
                    UserId = dto.Id,
                    RoleId = roleId,
                    CreateTime = DateTime.Now
                }).ToList();

                await _db.Insertable(userRoles).ExecuteCommandAsync();
            }
        }

        return result;
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">要删除的用户ID</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="BusinessException">
    /// 用户不存在时抛出NotFound异常
    /// </exception>
    /// <remarks>
    /// 删除用户时会级联删除用户角色关联表(UserRole)中的相关记录。
    /// 这是物理删除，数据将永久移除。
    /// </remarks>
    public new async Task<int> DeleteAsync(Guid id)
    {
        var user = await base.GetByIdAsync(id);
        if (user == null)
        {
            throw BusinessException.NotFound("用户不存在");
        }

        // 删除用户角色关联
        await _db.Deleteable<UserRole>().Where(ur => ur.UserId == id).ExecuteCommandAsync();

        return await base.DeleteAsync(id);
    }

    /// <summary>
    /// 批量删除用户
    /// </summary>
    /// <param name="ids">要删除的用户ID列表</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 批量删除用户时会级联删除用户角色关联表(UserRole)中的相关记录。
    /// 不允许删除admin用户。
    /// </remarks>
    public async Task<int> DeleteBatchAsync(List<Guid> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return 0;
        }

        // 查询要删除的用户，排除admin用户
        var users = await _db.Queryable<User>()
            .Where(u => ids.Contains(u.Id) && u.UserName != "admin")
            .ToListAsync();

        if (users.Count == 0)
        {
            return 0;
        }

        var userIdsToDelete = users.Select(u => u.Id).ToList();

        // 批量删除用户角色关联
        await _db.Deleteable<UserRole>()
            .Where(ur => userIdsToDelete.Contains(ur.UserId))
            .ExecuteCommandAsync();

        // 批量删除用户
        return await _db.Deleteable<User>()
            .Where(u => userIdsToDelete.Contains(u.Id))
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 验证用户是否有效
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户存在且状态为1返回true，否则返回false</returns>
    /// <remarks>
    /// 用于验证用户是否可正常使用系统。
    /// 常用于Token刷新验证、权限检查等场景。
    /// </remarks>
    public async Task<bool> ValidateUserAsync(Guid userId)
    {
        var user = await base.GetByIdAsync(userId);
        return user != null && user.Status == 1;
    }

    /// <summary>
    /// 更新用户状态
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="status">状态值：1-启用，0-禁用</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="BusinessException">
    /// 用户不存在时抛出NotFound异常；尝试禁用admin用户时抛出BadRequest异常
    /// </exception>
    public async Task<int> UpdateStatusAsync(Guid id, int status)
    {
        var user = await base.GetByIdAsync(id);
        if (user == null)
        {
            throw BusinessException.NotFound("用户不存在");
        }

        // 不允许禁用 admin 用户
        if (user.UserName == "admin" && status == 0)
        {
            throw BusinessException.BadRequest("不能禁用管理员账号");
        }

        user.Status = status;
        user.UpdateTime = DateTime.Now;

        return await base.UpdateAsync(user);
    }

    /// <summary>
    /// 重置用户密码
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="newPassword">新密码（明文，将自动MD5加密）</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="BusinessException">
    /// 用户不存在时抛出NotFound异常
    /// </exception>
    public async Task<int> ResetPasswordAsync(Guid id, string newPassword)
    {
        var user = await base.GetByIdAsync(id);
        if (user == null)
        {
            throw BusinessException.NotFound("用户不存在");
        }

        user.Password = SecurityHelper.Md5(newPassword);
        user.UpdateTime = DateTime.Now;

        return await base.UpdateAsync(user);
    }
}