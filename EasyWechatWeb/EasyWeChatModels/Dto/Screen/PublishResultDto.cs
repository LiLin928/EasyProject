namespace EasyWeChatModels.Dto;

/// <summary>
/// 发布结果 DTO
/// </summary>
/// <remarks>
/// 用于返回发布操作的结果
/// </remarks>
public class PublishResultDto
{
    /// <summary>
    /// 发布ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    public Guid PublishId { get; set; }

    /// <summary>
    /// 发布访问URL
    /// </summary>
    /// <example>https://example.com/screen/abc123</example>
    public string PublishUrl { get; set; } = string.Empty;
}