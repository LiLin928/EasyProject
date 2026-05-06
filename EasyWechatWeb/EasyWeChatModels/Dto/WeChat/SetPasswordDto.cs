namespace EasyWeChatModels.Dto;

/// <summary>
/// 设置密码请求 DTO
/// </summary>
public class SetPasswordDto
{
    /// <summary>
    /// 新密码
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 确认密码
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}