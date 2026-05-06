namespace EasyWeChatModels.Dto;

/// <summary>
/// 重置用户密码请求 DTO
/// </summary>
/// <remarks>
/// 用于重置用户密码，密码将自动进行MD5加密存储
/// </remarks>
public class ResetPasswordDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 新密码（明文）
    /// </summary>
    /// <remarks>
    /// 密码将自动进行MD5加密存储
    /// </remarks>
    /// <example>123456</example>
    public string NewPassword { get; set; } = string.Empty;
}