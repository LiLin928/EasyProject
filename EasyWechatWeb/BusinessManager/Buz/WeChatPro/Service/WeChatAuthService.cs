using BusinessManager.Buz.IService;
using CommonManager.Base;
using CommonManager.Helper;
using EasyWeChatModels.Options;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlSugar;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 微信认证服务实现类
/// </summary>
/// <remarks>
/// 实现微信登录相关的业务逻辑，包括登录认证、用户信息获取和更新等功能。
/// 当 UseMock=true 时使用 Mock 实现，UseMock=false 时调用真实微信 API。
/// </remarks>
public class WeChatAuthService : IWeChatAuthService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatAuthService> _logger { get; set; } = null!;
    /// <summary>
    /// JWT Token 配置选项
    /// </summary>
    public IOptions<JWTTokenOptions> _jwtOptions { get; set; } = null!;
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public ISqlSugarClient _db { get; set; } = null!;
    /// <summary>
    /// 微信 API 服务
    /// </summary>
    public WeChatApiService _wxApiService { get; set; } = null!;

    /// <summary>
    /// 微信登录
    /// </summary>
    /// <param name="code">微信登录 code</param>
    /// <returns>登录结果，包含Token和用户信息</returns>
    /// <remarks>
    /// 流程：
    /// <list type="number">
    ///     <item>调用微信 API 获取 OpenId（Mock 或真实）</item>
    ///     <item>根据OpenId查找或创建用户</item>
    ///     <item>生成JWT Token</item>
    ///     <item>返回登录结果</item>
    /// </list>
    /// </remarks>
    public async Task<WxLoginResultDto?> LoginAsync(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return null;
        }

        // 调用微信 API 获取 OpenId（支持 Mock 和真实 API）
        var wxSession = await _wxApiService.Code2SessionAsync(code);
        if (wxSession == null || string.IsNullOrEmpty(wxSession.OpenId))
        {
            _logger.LogWarning("微信登录失败: 无法获取 OpenId");
            return null;
        }

        var openId = wxSession.OpenId;

        // 查找或创建用户
        var user = await _db.Queryable<WeChatUser>()
            .Where(u => u.OpenId == openId)
            .FirstAsync();

        if (user == null)
        {
            // 创建新用户
            user = new WeChatUser
            {
                OpenId = openId,
                UnionId = wxSession.UnionId,
                UserName = $"wx_{openId.Substring(0, 8)}",
                Nickname = "微信用户",
                AvatarUrl = "/static/images/default-avatar.png",
                Gender = 0,
                Status = 1,
                CreateTime = DateTime.Now,
                WxLoginTime = DateTime.Now
            };

            await _db.Insertable(user).ExecuteCommandAsync();
            _logger.LogInformation("创建新微信用户: {OpenId}", openId);
        }
        else
        {
            // 更新登录时间和 UnionId
            user.WxLoginTime = DateTime.Now;
            user.LastLoginTime = DateTime.Now;
            if (!string.IsNullOrEmpty(wxSession.UnionId) && user.UnionId != wxSession.UnionId)
            {
                user.UnionId = wxSession.UnionId;
            }
            await _db.Updateable(user).ExecuteCommandAsync();
        }

        // 生成 Token
        var loginResult = CustomerJWTHelper.GetLoginToken(
            user.Id.ToString(),
            user.UserName,
            user.Nickname ?? "",
            _jwtOptions.Value
        );

        return new WxLoginResultDto
        {
            Id = user.Id,
            Token = loginResult.AccessToken,
            UserInfo = await GetUserInfoDtoAsync(user)
        };
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户信息；用户不存在返回null</returns>
    public async Task<WxUserInfoDto?> GetUserInfoAsync(Guid userId)
    {
        var user = await _db.Queryable<WeChatUser>()
            .Where(u => u.Id == userId && u.Status == 1)
            .FirstAsync();

        if (user == null)
        {
            return null;
        }

        return await GetUserInfoDtoAsync(user);
    }

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="dto">更新信息</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 支持更新昵称、头像、性别、手机号。
    /// </remarks>
    public async Task<bool> UpdateUserInfoAsync(Guid userId, UpdateWxUserDto dto)
    {
        var user = await _db.Queryable<WeChatUser>()
            .Where(u => u.Id == userId && u.Status == 1)
            .FirstAsync();

        if (user == null)
        {
            return false;
        }

        if (dto.Nickname != null)
        {
            user.Nickname = dto.Nickname;
        }

        if (dto.AvatarUrl != null)
        {
            user.AvatarUrl = dto.AvatarUrl;
        }

        if (dto.Gender.HasValue)
        {
            user.Gender = dto.Gender.Value;
        }

        if (dto.Phone != null)
        {
            user.Phone = dto.Phone;
        }

        user.UpdateTime = DateTime.Now;

        await _db.Updateable(user).ExecuteCommandAsync();

        _logger.LogInformation("更新用户信息: {UserId}", userId);

        return true;
    }

    /// <summary>
    /// 绑定手机号
    /// </summary>
    public async Task<BindPhoneResultDto> BindPhoneAsync(Guid userId, BindPhoneDto dto)
    {
        var user = await _db.Queryable<WeChatUser>()
            .Where(u => u.Id == userId)
            .FirstAsync();

        if (user == null)
        {
            return new BindPhoneResultDto
            {
                Success = false,
                Message = "用户不存在"
            };
        }

        // 检查手机号是否已被其他用户绑定
        var existsPhone = await _db.Queryable<WeChatUser>()
            .Where(u => u.Phone == dto.Phone && u.Id != userId)
            .FirstAsync();

        if (existsPhone != null)
        {
            return new BindPhoneResultDto
            {
                Success = false,
                Message = "该手机号已被其他用户绑定"
            };
        }

        // 更新手机号
        user.Phone = dto.Phone;
        user.BindPhoneTime = DateTime.Now;
        user.UpdateTime = DateTime.Now;

        await _db.Updateable(user).ExecuteCommandAsync();

        _logger.LogInformation("用户绑定手机号: {UserId}, {Phone}", userId, dto.Phone);

        return new BindPhoneResultDto
        {
            Success = true,
            Message = "绑定成功",
            UserInfo = await GetUserInfoDtoAsync(user)
        };
    }

    /// <summary>
    /// 设置密码
    /// </summary>
    public async Task<bool> SetPasswordAsync(Guid userId, SetPasswordDto dto)
    {
        if (dto.Password != dto.ConfirmPassword)
        {
            throw new CommonManager.Error.BusinessException("两次密码输入不一致");
        }

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
        {
            throw new CommonManager.Error.BusinessException("密码长度不能少于6位");
        }

        var user = await _db.Queryable<WeChatUser>()
            .Where(u => u.Id == userId)
            .FirstAsync();

        if (user == null)
        {
            return false;
        }

        // 密码加密
        user.Password = PasswordHelper.HashPassword(dto.Password);
        user.UpdateTime = DateTime.Now;

        await _db.Updateable(user).ExecuteCommandAsync();

        _logger.LogInformation("用户设置密码: {UserId}", userId);

        return true;
    }

    /// <summary>
    /// 手机号登录
    /// </summary>
    public async Task<WxLoginResultDto?> PhoneLoginAsync(PhoneLoginDto dto)
    {
        var user = await _db.Queryable<WeChatUser>()
            .Where(u => u.Phone == dto.Phone)
            .FirstAsync();

        if (user == null || string.IsNullOrEmpty(user.Password))
        {
            return null;
        }

        // 验证密码 - 支持 BCrypt 和 MD5 两种格式
        bool passwordValid = false;

        // 先尝试 BCrypt 验证（微信小程序设置的密码）
        if (user.Password.StartsWith("$2"))
        {
            passwordValid = PasswordHelper.VerifyPassword(dto.Password, user.Password);
        }
        else
        {
            // MD5 验证（PC端管理后台设置的密码）
            var md5Password = CommonManager.Helper.SecurityHelper.Md5(dto.Password);
            passwordValid = user.Password == md5Password;
        }

        if (!passwordValid)
        {
            return null;
        }

        // 更新最后登录时间
        user.LastLoginTime = DateTime.Now;
        user.UpdateTime = DateTime.Now;
        await _db.Updateable(user).ExecuteCommandAsync();

        // 生成 Token
        var loginResult = CustomerJWTHelper.GetLoginToken(
            user.Id.ToString(),
            user.UserName,
            user.Nickname ?? "",
            _jwtOptions.Value
        );

        _logger.LogInformation("用户手机号登录: {UserId}, {Phone}", user.Id, dto.Phone);

        return new WxLoginResultDto
        {
            Id = user.Id,
            Token = loginResult.AccessToken,
            UserInfo = await GetUserInfoDtoAsync(user)
        };
    }

    /// <summary>
    /// 微信手机号快速验证登录
    /// </summary>
    /// <param name="dto">微信手机号登录请求参数</param>
    /// <returns>登录结果；失败返回 null</returns>
    public async Task<WxLoginResultDto?> WxPhoneLoginAsync(WxPhoneLoginDto dto)
    {
        // 1. code -> sessionKey
        var session = await _wxApiService.Code2SessionAsync(dto.Code);
        if (session == null || string.IsNullOrEmpty(session.SessionKey))
        {
            _logger.LogWarning("微信手机号登录失败: 无法获取 SessionKey");
            return null;
        }

        // 2. 解密手机号
        var phoneInfo = _wxApiService.DecryptPhoneNumber(session.SessionKey, dto.EncryptedData, dto.Iv);
        if (phoneInfo == null || string.IsNullOrEmpty(phoneInfo.PurePhoneNumber))
        {
            _logger.LogWarning("微信手机号登录失败: 手机号解密失败");
            return null;
        }

        // 3. 根据手机号查找或创建用户
        var user = await _db.Queryable<WeChatUser>()
            .Where(u => u.Phone == phoneInfo.PurePhoneNumber)
            .FirstAsync();

        if (user == null)
        {
            // 创建新用户
            user = new WeChatUser
            {
                OpenId = session.OpenId,
                UnionId = session.UnionId,
                Phone = phoneInfo.PurePhoneNumber,
                UserName = $"user_{phoneInfo.PurePhoneNumber}",
                Nickname = "微信用户",
                AvatarUrl = "/static/images/default-avatar.png",
                Gender = 0,
                Status = 1,
                CreateTime = DateTime.Now,
                BindPhoneTime = DateTime.Now,
                LastLoginTime = DateTime.Now
            };
            await _db.Insertable(user).ExecuteCommandAsync();
            _logger.LogInformation("创建新用户: {Phone}", phoneInfo.PurePhoneNumber);
        }
        else
        {
            // 更新 OpenId 和登录时间
            user.OpenId = session.OpenId;
            user.LastLoginTime = DateTime.Now;
            user.UpdateTime = DateTime.Now;
            if (!string.IsNullOrEmpty(session.UnionId))
            {
                user.UnionId = session.UnionId;
            }
            await _db.Updateable(user).ExecuteCommandAsync();
            _logger.LogInformation("用户登录: {UserId}, {Phone}", user.Id, phoneInfo.PurePhoneNumber);
        }

        // 4. 生成 Token
        var loginResult = CustomerJWTHelper.GetLoginToken(
            user.Id.ToString(),
            user.UserName,
            user.Nickname ?? "",
            _jwtOptions.Value);

        return new WxLoginResultDto
        {
            Id = user.Id,
            Token = loginResult.AccessToken,
            UserInfo = await GetUserInfoDtoAsync(user)
        };
    }

    /// <summary>
    /// 获取用户信息 DTO
    /// </summary>
    private async Task<WxUserInfoDto> GetUserInfoDtoAsync(WeChatUser user)
    {
        string? levelName = null;
        if (user.LevelId.HasValue)
        {
            var level = await _db.Queryable<MemberLevel>()
                .Where(l => l.Id == user.LevelId.Value)
                .FirstAsync();
            levelName = level?.Name;
        }

        return new WxUserInfoDto
        {
            Id = user.Id,
            OpenId = user.OpenId,
            Nickname = user.Nickname,
            AvatarUrl = user.AvatarUrl,
            Phone = user.Phone,
            Gender = user.Gender,
            Points = user.Points,
            LevelName = levelName
        };
    }
}