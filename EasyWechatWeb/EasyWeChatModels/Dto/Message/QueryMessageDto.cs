namespace EasyWeChatModels.Dto;

/// <summary>
/// 消息查询参数 DTO
/// </summary>
public class QueryMessageDto
{
    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    /// <example>1</example>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    /// <example>10</example>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 消息类型筛选（1-系统消息 2-通知 3-提醒）
    /// </summary>
    public int? Type { get; set; }

    /// <summary>
    /// 阅读状态筛选（0-未读 1-已读）
    /// </summary>
    public int? IsRead { get; set; }
}