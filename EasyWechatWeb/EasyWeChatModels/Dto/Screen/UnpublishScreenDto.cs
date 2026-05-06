namespace EasyWeChatModels.Dto;

/// <summary>
/// 下架大屏请求 DTO
/// </summary>
/// <remarks>
/// 用于下架已发布的大屏
/// </remarks>
public class UnpublishScreenDto
{
    /// <summary>
    /// 大屏ID（与PublishId二选一）
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    public Guid? ScreenId { get; set; }

    /// <summary>
    /// 发布ID（与ScreenId二选一）
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    public Guid? PublishId { get; set; }
}