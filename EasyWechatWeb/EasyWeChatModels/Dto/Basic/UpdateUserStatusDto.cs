namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新用户状态请求 DTO
/// </summary>
/// <remarks>
/// 用于启用或禁用用户账号
/// </remarks>
public class UpdateUserStatusDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 状态值
    /// </summary>
    /// <remarks>
    /// 1-启用，0-禁用
    /// </remarks>
    /// <example>1</example>
    public int Status { get; set; }
}