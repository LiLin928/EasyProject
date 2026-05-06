using Microsoft.AspNetCore.Mvc;

namespace CommonManager.Base;

/// <summary>
/// 基础控制器，提供通用的辅助方法和响应封装
/// </summary>
/// <remarks>
/// 所有 API 控制器应继承此基类，以获得统一的用户信息获取方法和响应封装。
/// 包含获取当前用户信息、判断权限和返回标准响应的方法。
/// </remarks>
/// <example>
/// <code>
/// [ApiController]
/// [Route("api/[controller]/[action]")]
/// public class UserController : BaseController
/// {
///     [HttpGet("profile")]
///     public ApiResponse&lt;User&gt; GetProfile()
///     {
///         var userId = GetCurrentUserId();
///         // ...
///         return Success(user);
///     }
/// }
/// </code>
/// </example>
[ApiController]
[Route("api/[controller]/[action]")]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// 获取当前登录用户的 ID
    /// </summary>
    /// <returns>用户 ID，未登录时返回 Guid.Empty</returns>
    /// <remarks>
    /// 从 JWT Token 的 UserId 声明中获取。
    /// 需要接口添加 [Authorize] 特性才能获取到有效值。
    /// </remarks>
    /// <example>
    /// <code>
    /// [HttpGet("my-profile")]
    /// [Authorize]
    /// public async Task&lt;ApiResponse&lt;User&gt;&gt; GetMyProfile()
    /// {
    ///     var userId = GetCurrentUserId();
    ///     if (userId == Guid.Empty)
    ///         return Error&lt;User&gt;("请先登录", 401);
    ///
    ///     var user = await _userService.GetByIdAsync(userId);
    ///     return Success(user);
    /// }
    /// </code>
    /// </example>
    protected Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("UserId");
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
    }

    /// <summary>
    /// 获取当前登录用户的用户名
    /// </summary>
    /// <returns>用户名，未登录时返回空字符串</returns>
    /// <remarks>
    /// 从 JWT Token 的 UserName 声明中获取。
    /// </remarks>
    /// <example>
    /// <code>
    /// var userName = GetCurrentUserName();
    /// Console.WriteLine($"当前用户: {userName}");
    /// </code>
    /// </example>
    protected string GetCurrentUserName()
    {
        return User.FindFirst("UserName")?.Value ?? string.Empty;
    }

    /// <summary>
    /// 获取当前登录用户的真实姓名
    /// </summary>
    /// <returns>真实姓名，未登录时返回空字符串</returns>
    /// <remarks>
    /// 从 JWT Token 的 RealName 声明中获取。
    /// </remarks>
    /// <example>
    /// <code>
    /// var realName = GetCurrentRealName();
    /// Console.WriteLine($"欢迎: {realName}");
    /// </code>
    /// </example>
    protected string GetCurrentRealName()
    {
        return User.FindFirst("RealName")?.Value ?? string.Empty;
    }

    /// <summary>
    /// 判断当前用户是否为管理员
    /// </summary>
    /// <returns>是管理员返回 true，否则返回 false</returns>
    /// <remarks>
    /// 通过检查用户是否属于 "Admin" 角色来判断。
    /// 需要在 JWT Token 中包含角色声明。
    /// </remarks>
    /// <example>
    /// <code>
    /// [HttpDelete("{id}")]
    /// [Authorize]
    /// public async Task&lt;ApiResponse&lt;object&gt;&gt; DeleteUser(int id)
    /// {
    ///     if (!IsAdmin())
    ///         return Error("只有管理员才能删除用户", 403);
    ///
    ///     await _userService.DeleteAsync(id);
    ///     return Success("删除成功");
    /// }
    /// </code>
    /// </example>
    protected bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }

    /// <summary>
    /// 返回带数据的成功响应
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="data">响应数据</param>
    /// <param name="message">响应消息，默认为"操作成功"</param>
    /// <returns>成功的 API 响应对象</returns>
    /// <remarks>
    /// 封装 ApiResponse.Success 方法，提供更简洁的调用方式。
    /// </remarks>
    /// <example>
    /// <code>
    /// [HttpGet("{id}")]
    /// public async Task&lt;ApiResponse&lt;User&gt;&gt; GetUser(int id)
    /// {
    ///     var user = await _userService.GetByIdAsync(id);
    ///     return Success(user, "获取用户成功");
    /// }
    /// </code>
    /// </example>
    protected ApiResponse<T> Success<T>(T? data, string message = "操作成功")
    {
        return ApiResponse<T>.Success(data, message);
    }

    /// <summary>
    /// 返回无数据的成功响应
    /// </summary>
    /// <param name="message">响应消息，默认为"操作成功"</param>
    /// <returns>成功的 API 响应对象，Data 为 null</returns>
    /// <remarks>
    /// 适用于不需要返回数据的操作，如删除、更新、确认等。
    /// </remarks>
    /// <example>
    /// <code>
    /// [HttpPost]
    /// public async Task&lt;ApiResponse&lt;object&gt;&gt; CreateUser(CreateUserDto dto)
    /// {
    ///     await _userService.InsertAsync(new User { ... });
    ///     return Success("创建成功");
    /// }
    /// </code>
    /// </example>
    protected ApiResponse<object> Success(string message = "操作成功")
    {
        return ApiResponse<object>.Success(null, message);
    }

    /// <summary>
    /// 返回失败响应
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="message">错误消息</param>
    /// <param name="code">错误码，默认为 500</param>
    /// <returns>失败的 API 响应对象</returns>
    /// <remarks>
    /// 封装 ApiResponse.Error 方法，提供更简洁的调用方式。
    /// </remarks>
    /// <example>
    /// <code>
    /// [HttpGet("{id}")]
    /// public async Task&lt;ApiResponse&lt;User&gt;&gt; GetUser(int id)
    /// {
    ///     var user = await _userService.GetByIdAsync(id);
    ///     if (user == null)
    ///         return Error&lt;User&gt;("用户不存在", 404);
    ///
    ///     return Success(user);
    /// }
    /// </code>
    /// </example>
    protected ApiResponse<T> Error<T>(string message, int code = 500)
    {
        return ApiResponse<T>.Error(message, code);
    }
}