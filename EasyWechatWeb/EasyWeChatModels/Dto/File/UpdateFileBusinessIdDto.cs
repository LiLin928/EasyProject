namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新文件业务ID请求 DTO
/// </summary>
/// <remarks>
/// 用于批量更新文件的 BusinessId，将上传的文件关联到具体的业务记录。
/// </remarks>
public class UpdateFileBusinessIdDto
{
    /// <summary>
    /// 文件ID列表
    /// </summary>
    /// <example>["3fa85f64-5717-4562-b3fc-2c963f66afa6", "another-guid"]</example>
    public List<Guid> FileIds { get; set; } = new();

    /// <summary>
    /// 业务ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid BusinessId { get; set; }
}