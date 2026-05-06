namespace EasyWeChatModels.Dto;

/// <summary>
/// 公告阅读详情 DTO
/// </summary>
/// <remarks>
/// 用于返回用户的阅读状态详情
/// </remarks>
public class ReadDetailDto
{
    /// <summary>
    /// 记录ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 公告ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid AnnouncementId { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    /// <example>zhangsan</example>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 用户真实姓名
    /// </summary>
    /// <example>张三</example>
    public string? RealName { get; set; }

    /// <summary>
    /// 用户角色
    /// </summary>
    /// <example>管理员</example>
    public string? RoleName { get; set; }

    /// <summary>
    /// 是否已读：0未读 1已读
    /// </summary>
    /// <example>1</example>
    public int IsRead { get; set; }

    /// <summary>
    /// 阅读时间
    /// </summary>
    /// <example>2024-01-01T10:00:00</example>
    public DateTime? ReadTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 记录创建时间，即公告发布时间
    /// </remarks>
    /// <example>2024-01-01T08:00:00</example>
    public DateTime CreateTime { get; set; }
}